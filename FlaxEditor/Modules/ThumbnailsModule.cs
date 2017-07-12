////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.IO;
using FlaxEditor.Content;
using FlaxEngine;
using FlaxEngine.Assertions;
using FlaxEngine.GUI;
using FlaxEngine.Rendering;
using Object = FlaxEngine.Object;

namespace FlaxEditor.Modules
{
    /// <summary>
    /// Manages asset thumbnails rendering and presentation.
    /// </summary>
    /// <seealso cref="FlaxEditor.Modules.EditorModule" />
    public sealed class ThumbnailsModule : EditorModule, IContentItemOwner
    {
        private readonly List<PreviewsCache> _cache = new List<PreviewsCache>(4);
        private string _cacheFolder;

        private readonly List<ContentItem> _requests = new List<ContentItem>(128);
        private readonly PreviewRoot _guiRoot = new PreviewRoot();
        private EmptyActor _sceneRoot;
        private SceneRenderTask _task;
        private RenderTarget _output;

        internal ThumbnailsModule(Editor editor)
            : base(editor)
        {
            _cacheFolder = Path.Combine(Globals.ProjectCacheFolder, "Thumbnails");
        }

        /// <summary>
        /// Requests the item preview.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void RequestPreview(ContentItem item)
        {
            if (item == null)
                throw new ArgumentNullException();

            // Check if use default icon
            var defaultThumbnail = item.DefaultThumbnailName;
            if (!string.IsNullOrEmpty(defaultThumbnail))
            {
                item.Thumbnail = Editor.Instance.UI.GetIcon(defaultThumbnail);
                return;
            }

            lock (_requests)
            {
                // Check if element hasn't been already processed for generating preview
                if (!_requests.Contains(item))
                {
                    // Check each cache atlas
                    // Note: we cache previews only for items with 'ID', for now we support only AssetItems
                    if (item is AssetItem assetItem)
                    {
                        Sprite sprite;
                        for (int i = 0; i < _cache.Count; i++)
                        {
                            if (_cache[i].FindPreview(assetItem.ID, out sprite))
                            {
                                // Found!
                                item.Thumbnail = sprite;
                                return;
                            }
                        }
                    }

                    // Add request
                    item.AddReference(this);
                    _requests.Add(item);
                }
            }
        }

        /// <summary>
        /// Deletes the item preview from the cache.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <exception cref="System.ArgumentNullException"></exception>
        public void DeletePreview(ContentItem item)
        {
            if (item == null)
                throw new ArgumentNullException();

            lock (_requests)
            {
                // Cancel loading
                _requests.Remove(item);
                item.RemoveReference(this);

                // Find atlas with preview and remove it
                // Note: we cache previews only for items with 'ID', for now we support only AssetItems
                if (item is AssetItem assetItem)
                {
                    for (int i = 0; i < _cache.Count; i++)
                    {
                        if (_cache[i].ReleaseSlot(assetItem.ID))
                        {
                            break;
                        }
                    }
                }
            }
        }

        #region IContentItemOwner

        /// <inheritdoc />
        void IContentItemOwner.OnItemDeleted(ContentItem item)
        {
            lock (_requests)
            {
                _requests.Remove(item);
            }
        }

        /// <inheritdoc />
        void IContentItemOwner.OnItemRenamed(ContentItem item)
        {
        }

        /// <inheritdoc />
        void IContentItemOwner.OnItemDispose(ContentItem item)
        {
            lock (_requests)
            {
                _requests.Remove(item);
            }
        }

        #endregion

        /// <inheritdoc />
        public override void OnInit()
        {
            // Create cache folder
            if (!Directory.Exists(_cacheFolder))
            {
                Directory.CreateDirectory(_cacheFolder);
            }

            // Find atlases in a Editor cache directory
            var files = Directory.GetFiles(_cacheFolder, "cache_*.flax", SearchOption.TopDirectoryOnly);
            int atlases = 0;
            for (int i = 0; i < files.Length; i++)
            {
                // Load asset
                var asset = FlaxEngine.Content.LoadAsync(files[i]);
                if (asset == null)
                    continue;

                // Validate type
                if (asset is PreviewsCache atlas)
                {
                    // Cache atlas
                    atlases++;
                    _cache.Add(atlas);
                }
                else
                {
                    // Skip asset
                    Debug.LogWarning(string.Format("Asset \'{0}\' is inside Editor\'s private directory for Assets Thumbnails Cache. Please move it.", asset.Name));
                }
            }
            Debug.Log(string.Format("Previews cache count: {0} (capacity for {1} icons)", atlases, atlases * PreviewsCache.AssetIconsPerAtlas));

            // Create render task but disabled for now
            _output = RenderTarget.New();
            _output.Init(PreviewsCache.AssetIconsAtlasFormat, PreviewsCache.AssetIconSize, PreviewsCache.AssetIconSize);
            _task = RenderTask.Create<SceneRenderTask>();
            _task.Enabled = false;
            _task.Output = _output;
            _task.OnBegin += OnBegin;
            _task.OnEnd += OnEnd;
        }

        private void OnBegin(SceneRenderTask task)
        {
            lock (_requests)
            {
                // Check if has no requests (maybe removed in async)
                if (_requests.Count == 0)
                    return;

                // Get asset to refresh
                var item = _requests[0];

                // Get proxy for that element
                var proxy = Editor.ContentDatabase.GetProxy(item);
                if (proxy == null)
                    return;

                // Setup
                _guiRoot.RemoveChildren();
                _guiRoot.AccentColor = proxy.AccentColor;
                Assert.IsFalse(_sceneRoot.HasChildren);

                // Call proxy to prepare for thumbnail rendering
                // It can setup preview scene and additional GUI
                proxy.OnThumbnailDrawBegin(item, _sceneRoot, _guiRoot);
            }
        }

        private void OnEnd(SceneRenderTask task)
        {
            // Check if has no requests (maybe removed in async)
            if (_requests.Count == 0)
                return;

            // Get asset to flush with the cache atlas
            var item = _requests[0];
            _requests.RemoveAt(0);

            // Get proxy for that element
            var proxy = Editor.ContentDatabase.GetProxy(item);

            // Call proxy and cleanup UI (delete create controls, shared controls should be unlinked during OnPreviewRenderEnd event)
            proxy.OnThumbnailDrawEnd(item, _sceneRoot, _guiRoot);
            _guiRoot.DisposeChildren();
            _sceneRoot.DeleteChilden();

            // Find atlas with an free slot
            var atlas = getValidAtlas();
            if (atlas == null)
            {
                // Error
                _task.Enabled = false;
                _requests.Clear();
                return;
            }

            // Copy backbuffer with rendered preview into atlas
            Sprite icon;
            int result = atlas->OccupySlot(_output->GetBackBuffer(), el->GetID(), out icon);
            if (result)
            {
                // Error
                _task.Enabled = false;
                _requests.Clear();
                LOG_EDITOR(Fatal, 24, result);
                return;
            }

            // Assign new preview icon
            item.OnLoadPreview(icon);

            // Check if the is next 
            // Disable task
            _task.Enabled = false;
        }

        private bool GetReadyItem(int maxChecks)
        {
            for (int i = 0; i < maxChecks; i++)
            {
                // Check if first item is ready
                var item = _requests[i];
                var proxy = Editor.ContentDatabase.GetProxy(item);
                if (proxy != null && proxy.CanDrawThumbnail(item))
                {
                    // For non frst elements do the swap with keeping order
                    if (i != 0)
                    {
                        _requests.RemoveAt(i);
                        _requests.Insert(0, item);
                    }

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc />
        public override void OnUpdate()
        {
            // Wait some frames before start generating previews (late init feature)
            if (CEngine->FrameCount < 10 || hasAllAtlasesLoaded() == false)
            {
                // Back
                return;
            }

            lock (_requests)
            {
                // Check if has any request pending
                if (_requests.Count > 0)
                {
                    // Check if has no rendering task enabled
                    if (_task.Enabled == false)
                    {
                        if (GetReadyItem(_requests.Count))
                        {
                            // Start generating preview
                            startPreviewsQueue();
                        }
                    }
                }
                else
                {
                    // Flush data
                    flush();
                }
            }
        }

        /// <inheritdoc />
        public override void OnExit()
        {
            _task.Enabled = false;

            lock (_requests)
            {
                // Clear data
                for (int i = 0; i < _requests.Count; i++)
                {
                    _requests[i].RemoveReference(this);
                }
                _requests.Clear();
                _cache.Clear();
            }

            _guiRoot.Dispose();
            _task.Dispose();
            Object.Destroy(ref _output);
        }

        private class PreviewRoot : ContainerControl
        {
            /// <summary>
            /// The item accent color to draw.
            /// </summary>
            public Color AccentColor;

            /// <inheritdoc />
            public PreviewRoot()
                : base(false, 0, 0, PreviewsCache.AssetIconSize, PreviewsCache.AssetIconSize)
            {
                AccentColor = Color.Pink;
                IsLayoutLocked = false;
            }

            /// <inheritdoc />
            public override void Draw()
            {
                base.Draw();

                // Draw accent
                const float accentHeight = 2;
                Render2D.FillRectangle(new Rectangle(0, Height - accentHeight, Width, accentHeight), AccentColor);
            }

            /// <inheritdoc />
            protected override void SetSizeInternal(Vector2 size)
            {
                // Cannot change default preview size
            }
        }
    }
}
