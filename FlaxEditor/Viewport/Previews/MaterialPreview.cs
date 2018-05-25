// Copyright (c) 2012-2018 Wojciech Figat. All rights reserved.

using FlaxEngine;
using Object = FlaxEngine.Object;

namespace FlaxEditor.Viewport.Previews
{
    /// <summary>
    /// Material or Material Instance asset preview editor viewport.
    /// </summary>
    /// <seealso cref="AssetPreview" />
    public class MaterialPreview : AssetPreview
    {
        private string[] Models =
        {
            "Sphere",
            "Cube",
            "Plane",
            "Cylinder",
            "Cone"
        };


        private ModelActor _previewModel;
        private Decal _decal;
        private MaterialBase _material;

        /// <summary>
        /// Gets or sets the material asset to preview. It can be <see cref="FlaxEngine.Material"/> or <see cref="FlaxEngine.MaterialInstance"/>.
        /// </summary>
        public MaterialBase Material
        {
            get => _material;
            set
            {
                if (_material != value)
                {
                    _material = value;
                    UpdateMaterial();
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MaterialPreview"/> class.
        /// </summary>
        /// <param name="useWidgets">if set to <c>true</c> use widgets.</param>
        public MaterialPreview(bool useWidgets)
            : base(useWidgets)
        {
            // Setup preview scene
            _previewModel = ModelActor.New();
            _previewModel.Transform = new Transform(Vector3.Zero, Quaternion.Identity, new Vector3(0.45f));
            _previewModel.Model = FlaxEngine.Content.LoadAsyncInternal<Model>("Editor/Primitives/Sphere");

            // Link actors for rendering
            Task.CustomActors.Add(_previewModel);

            // TODO: don't wait for model but assign material in async on task begin or sth?
            // do it like in c++ editor
            _previewModel.Model?.WaitForLoaded();

            {
                var modelSelect = ViewWidgetButtonMenu.AddChildMenu("Model").ContextMenu;
                for (int i = 0; i < Models.Length; i++)
                {
                    var v = Models[i];
                    var button = modelSelect.AddButton(v);
                    button.Tag = v;
                }

                modelSelect.ButtonClicked += (button) => _previewModel.Model = FlaxEngine.Content.LoadAsyncInternal<Model>("Editor/Primitives/" + button.Tag);
            }
        }

        /// <inheritdoc />
        public override bool HasLoadedAssets => base.HasLoadedAssets && _previewModel.Model.IsLoaded;

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            UpdateMaterial();
        }

        private void UpdateMaterial()
        {
            // If material is a surface link it to the preview model.
            // Otherwise use postFx volume to render custom postFx material.
            MaterialBase surfaceMaterial = null;
            MaterialBase postFxMaterial = null;
            MaterialBase decalMaterial = null;
            if (_material != null)
            {
                if (_material is MaterialInstance materialInstance && materialInstance.BaseMaterial == null)
                {
                    // Material instance without a base material should not be used
                }
                else
                {
                    if (_material.IsPostFx)
                        postFxMaterial = _material;
                    else if (_material.IsDecal)
                        decalMaterial = _material;
                    else
                        surfaceMaterial = _material;
                }
            }

            var entries = _previewModel.Entries;
            if (entries.Length == 1)
                entries[0].Material = surfaceMaterial;
            PostFxVolume.Settings.PostFxMaterials = new[] { postFxMaterial };
            if (decalMaterial && _decal == null)
            {
                _decal = Decal.New();
                _decal.Size = new Vector3(120.0f);
                Task.CustomActors.Add(_decal);
            }
            if (_decal)
                _decal.Material = decalMaterial;
        }

        /// <inheritdoc />
        public override void OnDestroy()
        {
            _material = null;

            // Ensure to cleanup created actor objects
            Object.Destroy(ref _previewModel);
            Object.Destroy(ref _decal);

            base.OnDestroy();
        }
    }
}