////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using FlaxEngine;
using FlaxEngine.Rendering;

namespace FlaxEditor.Content.Settings
{
    /// <summary>
    /// The graphics rendering settings container. Allows to edit asset via editor. To modify those settings at runtime use <see cref="GraphicsQuality"/>.
    /// </summary>
    /// <seealso cref="GraphicsQuality"/>
    public sealed class GraphicsSettings : SettingsBase
    {
        /// <summary>
        /// Enables rendering synchronization with the refresh rate of the display device to avoid "tearing" artifacts.
        /// </summary>
        [EditorOrder(20), EditorDisplay("General"), Tooltip("Enables rendering synchronization with the refresh rate of the display device to avoid \"tearing\" artifacts.")]
        public bool UseVSync = false;

        /// <summary>
        /// Screen Space Reflections quality.
        /// </summary>
        [EditorOrder(1000), EditorDisplay("Quality", "SSR Quality"), Tooltip("Screen Space Reflections quality.")]
        public Quality SSRQuality = Quality.Medium;

        /// <summary>
        /// Screen Space Ambient Occlusion quality setting.
        /// </summary>
        [EditorOrder(1010), EditorDisplay("Quality", "SSAO Quality"), Tooltip("Screen Space Ambient Occlusion quality setting.")]
        public Quality SSAOQuality = Quality.Medium;

        /// <summary>
        /// The shadows quality.
        /// </summary>
        [EditorOrder(1020), EditorDisplay("Quality", "Shadows Quality"), Tooltip("The shadows quality.")]
        public Quality ShadowsQuality = Quality.Medium;

        /// <summary>
        /// The shadow maps quality (textures resolution).
        /// </summary>
        [EditorOrder(1030), EditorDisplay("Quality", "Shadow Maps Quality"), Tooltip("The shadow maps quality (textures resolution).")]
        public Quality ShadowMapsQuality = Quality.Medium;

        /// <summary>
        /// Enables cascades splits blending for directional light shadows.
        /// </summary>
        [EditorOrder(1040), EditorDisplay("Quality", "Allow CSM Blending"), Tooltip("Enables cascades splits blending for directional light shadows.")]
        public bool AllowCSMBlending = true;
    }
}
