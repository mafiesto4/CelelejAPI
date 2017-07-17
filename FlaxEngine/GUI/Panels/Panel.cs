﻿// Flax Engine scripting API

using System;

namespace FlaxEngine.GUI
{
    /// <summary>
    /// Panel UI control.
    /// </summary>
    /// <seealso cref="FlaxEngine.GUI.ContainerControl" />
    public class Panel : ContainerControl
    {
        /// <summary>
        /// The scroll right corner. Used to scroll contents of the panel control.
        /// </summary>
        protected Vector2 _scrollRightCorner;

        /// <summary>
        /// The vertical scroll bar.
        /// </summary>
        public readonly VScrollBar VScrollBar;

        /// <summary>
        /// The horizontal scroll bar.
        /// </summary>
        public readonly HScrollBar HScrollBar;

        /// <summary>
        /// Gets the scrolling right corner.
        /// </summary>
        /// <value>
        /// The scrolling right corner.
        /// </value>
        public Vector2 ScrollRightCorner
        {
            get { return _scrollRightCorner; }
            internal set { _scrollRightCorner = value; }
        }
        
        /// <summary>
        /// Gets the view bottom.
        /// </summary>
        /// <value>
        /// The view bottom.
        /// </value>
        public Vector2 ViewBottom => Size + _viewOffset;

        /// <summary>
        /// Initializes a new instance of the <see cref="Panel"/> class.
        /// </summary>
        /// <param name="scrollBars">The scroll bars.</param>
        /// <param name="canFocus">True if control can accept user focus</param>
        public Panel(ScrollBars scrollBars, bool canFocus = false)
            : base(canFocus)
        {
            // Create scroll bars
            if ((scrollBars & ScrollBars.Vertical) == ScrollBars.Vertical)
            {
                // Create vertical sroll bar
                VScrollBar = new VScrollBar(Width - ScrollBar.DefaultSize, Height);
                VScrollBar.Parent = this;
            }
            if ((scrollBars & ScrollBars.Horizontal) == ScrollBars.Horizontal)
            {
                // Create vertical sroll bar
                HScrollBar = new HScrollBar(Height - ScrollBar.DefaultSize, Width);
                HScrollBar.Parent = this;
            }
        }

        /// <summary>
        /// Scrolls the view to the given control area.
        /// </summary>
        /// <param name="c">The control.</param>
        public void ScrollViewTo(Control c)
        {
            if (c == null)
                throw new ArgumentNullException();

            Vector2 location = c.Location;
            Vector2 size = c.Size;
            while (c.HasParent && c.Parent != this)
            {
                c = c.Parent;
                location = c.PointToParent(location);
            }

            if (c.HasParent)
            {
                ScrollViewTo(new Rectangle(location, size));
            }
        }

        /// <summary>
        /// Scrolls the view to the given location.
        /// </summary>
        /// <param name="location">The location.</param>
        public void ScrollViewTo(Vector2 location)
        {
            ScrollViewTo(new Rectangle(location, Vector2.Zero));
        }

        /// <summary>
        /// Scrolls the view to the given area.
        /// </summary>
        /// <param name="bounds">The bounds.</param>
        public void ScrollViewTo(Rectangle bounds)
        {
            bool wasLocked = IsLayoutLocked;
            IsLayoutLocked = true;

            if (HScrollBar != null && HScrollBar.Visible)
                HScrollBar.ScrollViewTo(bounds.Left, bounds.Right);
            if (VScrollBar != null && VScrollBar.Visible)
                VScrollBar.ScrollViewTo(bounds.Top, bounds.Bottom);

            IsLayoutLocked = wasLocked;
            PerformLayout();
        }

        internal void setViewOffset(Orientation orientation, float value)
        {
            if (orientation == Orientation.Vertical)
                _viewOffset.Y = -value;
            else
                _viewOffset.X = -value;
        }

        /// <inheritdoc />
        public override bool OnMouseWheel(Vector2 location, int delta)
        {
            // Base
            if (base.OnMouseWheel(location, delta))
                return true;

            // Roll back to scroll bars
            if (VScrollBar != null && VScrollBar.Visible && VScrollBar.OnMouseWheel(VScrollBar.PointFromParent(location), delta))
                return true;
            if (HScrollBar != null && HScrollBar.Visible && HScrollBar.OnMouseWheel(HScrollBar.PointFromParent(location), delta))
                return true;

            // No event handled
            return false;
        }

        /// <inheritdoc />
        public override void RemoveChildren()
        {
            // Keep scroll bars alive
            if (VScrollBar != null)
                _children.Remove(VScrollBar);
            if (HScrollBar != null)
                _children.Remove(HScrollBar);

            base.RemoveChildren();

            // Restore scrollbars
            if (VScrollBar != null)
                _children.Add(VScrollBar);
            if (HScrollBar != null)
                _children.Add(HScrollBar);
            PerformLayout();
        }

        /// <inheritdoc />
        public override void DisposeChildren()
        {
            // Keep scroll bars alive
            if (VScrollBar != null)
                _children.Remove(VScrollBar);
            if (HScrollBar != null)
                _children.Remove(HScrollBar);

            base.DisposeChildren();

            // Restore scrollbars
            if (VScrollBar != null)
                _children.Add(VScrollBar);
            if (HScrollBar != null)
                _children.Add(HScrollBar);
            PerformLayout();
        }

        /// <inheritdoc />
        public override void OnChildResized(Control control)
        {
            base.OnChildResized(control);
            PerformLayout();
        }

        /// <inheritdoc />
        protected override bool IntersectsChildContent(Control child, Vector2 location, out Vector2 childSpaceLocation)
        {
            // For not scroll bars we want to reject any collisions
            if (child != VScrollBar && child != HScrollBar)
            {
                // Check if has v scroll bar to reject points on it
                if (VScrollBar != null && VScrollBar.Visible)
                {
                    Vector2 pos = VScrollBar.PointFromParent(location);
                    if (VScrollBar.ContainsPoint(ref pos))
                    {
                        childSpaceLocation = Vector2.Zero;
                        return false;
                    }
                }

                // Check if has h scroll bar to reject points on it
                if (HScrollBar != null && HScrollBar.Visible)
                {
                    Vector2 pos = HScrollBar.PointFromParent(location);
                    if (HScrollBar.ContainsPoint(ref pos))
                    {
                        childSpaceLocation = Vector2.Zero;
                        return false;
                    }
                }
            }

            return base.IntersectsChildContent(child, location, out childSpaceLocation);
        }

        /// <inheritdoc />
        internal override void AddChildInternal(Control child)
        {
            base.AddChildInternal(child);
            PerformLayout();
        }

        /// <inheritdoc />
        protected override void PerformLayoutSelf()
        {
            const float ScrollSpaceLeft = 0.1f;

            // Arrange controls and get scroll bounds
            ArrageAndGetBounds();

            // Scroll bars
            if (VScrollBar != null)
            {
                float height = Height;
                bool vScrollEnabled = _scrollRightCorner.Y > height + 0.01f && height > ScrollBar.DefaultMinimumSize;

                if (VScrollBar.Visible != vScrollEnabled)
                {
                    // Set scroll bar visibility 
                    VScrollBar.Visible = vScrollEnabled;

                    // Clear scroll state
                    VScrollBar.Reset();
                    _viewOffset.Y = 0;

                    // Update
                    ArrageAndGetBounds();
                }

                if (vScrollEnabled)
                {
                    VScrollBar.Maximum = _scrollRightCorner.Y - height * (1 - ScrollSpaceLeft);
                }
            }
            if (HScrollBar != null)
            {
                float width = Width;
                bool hScrollEnabled = _scrollRightCorner.X > width + 0.01f && width > ScrollBar.DefaultMinimumSize;

                if (HScrollBar.Visible != hScrollEnabled)
                {
                    // Set scroll bar visibility 
                    HScrollBar.Visible = hScrollEnabled;

                    // Clear scroll state
                    HScrollBar.Reset();

                    _viewOffset.X = 0;

                    // Update
                    ArrageAndGetBounds();
                }

                if (hScrollEnabled)
                {
                    HScrollBar.Maximum = _scrollRightCorner.X - width * (1 - ScrollSpaceLeft);
                }
            }
        }

        /// <summary>
        /// Arrages the child controls and gets their bounds.
        /// </summary>
        protected virtual void ArrageAndGetBounds()
        {
            Arrage();

            // Calculate scroll area bounds
            Vector2 rigthBottom = Vector2.Zero;
            for (int i = 0; i < _children.Count; i++)
            {
                var c = _children[i];
                if (c.Visible && c.IsScrollable)
                {
                    rigthBottom = Vector2.Max(rigthBottom, c.BottomRight);
                }
            }

            // Cache result
            _scrollRightCorner = rigthBottom;
        }

        /// <summary>
        /// Arrages the child controls.
        /// </summary>
        protected virtual void Arrage()
        {
            base.PerformLayoutSelf();
        }
    }
}
