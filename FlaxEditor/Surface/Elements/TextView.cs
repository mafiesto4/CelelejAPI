// Copyright (c) 2012-2020 Wojciech Figat. All rights reserved.

using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.Surface.Elements
{
    /// <summary>
    /// Text drawing element.
    /// </summary>
    /// <seealso cref="FlaxEditor.Surface.SurfaceNodeElementControl" />
    public sealed class TextView : SurfaceNodeElementControl
    {
        /// <inheritdoc />
        public TextView(SurfaceNode parentNode, NodeElementArchetype archetype)
        : base(parentNode, archetype, archetype.ActualPosition, archetype.Size, false)
        {
        }

        /// <inheritdoc />
        public override void Draw()
        {
            base.Draw();

            var style = Style.Current;
            var color = Enabled ? style.Foreground : style.ForegroundDisabled;
            Render2D.DrawText(style.FontSmall, Archetype.Text, new Rectangle(Vector2.Zero, Size), color, TextAlignment.Near, TextAlignment.Center);
        }
    }
}
