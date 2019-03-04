// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.

using System;
using FlaxEditor.Content.Thumbnails;
using FlaxEditor.Viewport.Previews;
using FlaxEditor.Windows;
using FlaxEngine;
using FlaxEngine.GUI;
using FlaxEngine.Rendering;

namespace FlaxEditor.Content
{
    /// <summary>
    /// A <see cref="ParticleEmitter"/> asset proxy object.
    /// </summary>
    /// <seealso cref="FlaxEditor.Content.BinaryAssetProxy" />
    public class ParticleEmitterProxy : BinaryAssetProxy
    {
        private ParticleEmitterPreview _preview;

        /// <inheritdoc />
        public override string Name => "Particle Emitter";

        /// <inheritdoc />
        public override EditorWindow Open(Editor editor, ContentItem item)
        {
            throw new NotImplementedException();
            //return new ParticleEmitterWindow(editor, item as AssetItem);
        }

        /// <inheritdoc />
        public override Color AccentColor => Color.FromRGB(0xFF79D2B0);

        /// <inheritdoc />
        public override ContentDomain Domain => ContentDomain.Other;

        /// <inheritdoc />
        public override Type AssetType => typeof(ParticleEmitter);

        /// <inheritdoc />
        public override bool CanCreate(ContentFolder targetLocation)
        {
            return targetLocation.CanHaveAssets;
        }

        /// <inheritdoc />
        public override void Create(string outputPath, object arg)
        {
            if (Editor.CreateAsset(Editor.NewAssetType.ParticleEmitter, outputPath))
                throw new Exception("Failed to create new asset.");
        }

        /// <inheritdoc />
        public override void OnThumbnailDrawPrepare(ThumbnailRequest request)
        {
            if (_preview == null)
            {
                _preview = new ParticleEmitterPreview(false);
                _preview.RenderOnlyWithWindow = false;
                _preview.Task.Enabled = false;
                _preview.PostFxVolume.Settings.Eye_Technique = EyeAdaptationTechnique.None;
                _preview.PostFxVolume.Settings.Eye_Exposure = 0.1f;
                _preview.PostFxVolume.Settings.data.Flags4 |= 0b1001;
                _preview.Size = new Vector2(PreviewsCache.AssetIconSize, PreviewsCache.AssetIconSize);
                _preview.SyncBackbufferSize();
            }
        }

        /// <inheritdoc />
        public override bool CanDrawThumbnail(ThumbnailRequest request)
        {
            if (!_preview.HasLoadedAssets)
                return false;

            // Check if asset is streamed enough
            var asset = (ParticleEmitter)request.Asset;
            return asset.IsLoaded;
        }

        /// <inheritdoc />
        public override void OnThumbnailDrawBegin(ThumbnailRequest request, ContainerControl guiRoot, GPUContext context)
        {
            _preview.Emitter = (ParticleEmitter)request.Asset;
            _preview.Parent = guiRoot;

            _preview.Task.Internal_Render(context);
        }

        /// <inheritdoc />
        public override void OnThumbnailDrawEnd(ThumbnailRequest request, ContainerControl guiRoot)
        {
            _preview.Emitter = null;
            _preview.Parent = null;
        }

        /// <inheritdoc />
        public override void Dispose()
        {
            if (_preview != null)
            {
                _preview.Dispose();
                _preview = null;
            }

            base.Dispose();
        }
    }
}