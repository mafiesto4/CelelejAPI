// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using FlaxEditor.GUI.Drag;
using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.GUI.Timeline
{
    /// <summary>
    /// The Timeline track that contains a header and custom timeline events/media.
    /// </summary>
    /// <seealso cref="FlaxEngine.GUI.ContainerControl" />
    public class Track : ContainerControl
    {
        /// <summary>
        /// The default prefix for drag data used for tracks dragging.
        /// </summary>
        public const string DragPrefix = "TRACK!?";

        private Timeline _timeline;
        private Track _parentTrack;
        internal float _xOffset;
        private float DefaultNodeOffsetY = 1.0f;
        private float DefaultDragInsertPositionMargin = 2.0f;
        private Margin _margin = new Margin(2.0f);
        private readonly List<Media> _media = new List<Media>();
        private readonly List<Track> _subTracks = new List<Track>();
        private bool _opened;
        private bool _isMouseDown;
        private float _mouseDownTime;
        private bool _mouseOverArrow;
        private Vector2 _mouseDownPos;

        private DragNames _dragTracks;
        private DragHandlers _dragHandlers;
        private DragItemPositioning _dragOverMode;
        private bool _isDragOverHeader;

        /// <summary>
        /// Gets the parent timeline.
        /// </summary>
        public Timeline Timeline => _timeline;

        /// <summary>
        /// Gets the parent track (null if this track is one of the root tracks in timeline).
        /// </summary>
        public Track ParentTrack
        {
            get => _parentTrack;
            set
            {
                if (_parentTrack != value)
                {
                    _parentTrack?.RemoveSubTrack(this);
                    value?.AdSubTrack(this);
                }
            }
        }

        /// <summary>
        /// Gets or sets the index of the track (in timeline track list).
        /// </summary>
        public int TrackIndex
        {
            get
            {
                int result = -1;
                for (int i = 0; i < _timeline.Tracks.Count; i++)
                {
                    if (_timeline.Tracks[i] == this)
                    {
                        result = i;
                        break;
                    }
                }
                return result;
            }
            set => _timeline.ChangeTrackIndex(this, value);
        }

        /// <summary>
        /// Gets the collection of the media events added to this track (read-only list).
        /// </summary>
        public IReadOnlyList<Media> Media => _media;

        /// <summary>
        /// Occurs when collection of the media events gets changed.
        /// </summary>
        public event Action<Track> MediaChanged;

        /// <summary>
        /// Gets the collection of the child tracks added to this track (read-only list).
        /// </summary>
        public IReadOnlyList<Track> SubTracks => _subTracks;

        /// <summary>
        /// Occurs when collection of the sub tracks gets changed.
        /// </summary>
        public event Action<Track> SubTracksChanged;

        /// <summary>
        /// The track text.
        /// </summary>
        public string Name;

        /// <summary>
        /// The track icon.
        /// </summary>
        public Sprite Icon;

        /// <summary>
        /// Gets or sets a value indicating whether this track is expanded.
        /// </summary>
        public bool IsExpanded
        {
            get => _opened;
            set
            {
                if (value)
                    Expand();
                else
                    Collapse();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this track is collapsed.
        /// </summary>
        public bool IsCollapsed
        {
            get => !_opened;
            set
            {
                if (value)
                    Collapse();
                else
                    Expand();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Track"/> class.
        /// </summary>
        public Track()
        : base(0, 0, 100, 22.0f)
        {
        }

        /// <summary>
        /// Gets the arrow rectangle.
        /// </summary>
        protected Rectangle ArrowRect => new Rectangle(_xOffset + 2 + _margin.Left, (Height - 12) * 0.5f, 12, 12);

        /// <summary>
        /// Called when parent timeline gets changed.
        /// </summary>
        /// <param name="timeline">The timeline.</param>
        public virtual void OnTimelineChanged(Timeline timeline)
        {
            _timeline = timeline;
        }

        /// <summary>
        /// Called when parent track gets changed.
        /// </summary>
        /// <param name="parent">The parent track.</param>
        public virtual void OnParentTrackChanged(Track parent)
        {
            _parentTrack = parent;
        }

        /// <summary>
        /// Determines whether the specified track contains is contained in this track sub track or any sub track children.
        /// </summary>
        /// <param name="track">The track.</param>
        /// <returns><c>true</c> if this track contains the specified track; otherwise, <c>false</c>.</returns>
        public bool ContainsTrack(Track track)
        {
            return _subTracks.Any(x => x == track || ContainsTrack(x));
        }

        /// <summary>
        /// Adds the media.
        /// </summary>
        /// <param name="media">The media.</param>
        public virtual void AddMedia(Media media)
        {
            _media.Add(media);
            media.OnTimelineChanged(this);

            OnMediaChanged();
        }

        /// <summary>
        /// Removes the media.
        /// </summary>
        /// <param name="media">The media.</param>
        public virtual void RemoveMedia(Media media)
        {
            media.OnTimelineChanged(null);
            _media.Remove(media);

            OnMediaChanged();
        }

        /// <summary>
        /// Adds the sub track.
        /// </summary>
        /// <param name="track">The track.</param>
        public virtual void AdSubTrack(Track track)
        {
            _subTracks.Add(track);
            track.OnParentTrackChanged(this);

            OnSubTracksChanged();
        }

        /// <summary>
        /// Removes the sub track.
        /// </summary>
        /// <param name="track">The track.</param>
        public virtual void RemoveSubTrack(Track track)
        {
            track.OnParentTrackChanged(null);
            _subTracks.Remove(track);

            OnSubTracksChanged();
        }

        /// <summary>
        /// Called when collection of the media items gets changed.
        /// </summary>
        protected virtual void OnMediaChanged()
        {
            MediaChanged?.Invoke(this);
        }

        /// <summary>
        /// Called when collection of the sub tracks gets changed.
        /// </summary>
        protected virtual void OnSubTracksChanged()
        {
            SubTracksChanged?.Invoke(this);
        }

        /// <summary>
        /// Called when drag and drop enters the track header area.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Drag action response.</returns>
        protected virtual DragDropEffect OnDragEnterHeader(DragData data)
        {
            if (_dragHandlers == null)
                _dragHandlers = new DragHandlers();

            // Check if drop tracks
            if (_dragTracks == null)
            {
                _dragTracks = new DragNames(DragPrefix, ValidateTrackDrag);
                _dragHandlers.Add(_dragTracks);
            }
            if (_dragTracks.OnDragEnter(data))
                return _dragTracks.Effect;

            return DragDropEffect.None;
        }

        private bool ValidateTrackDrag(string name)
        {
            // Find track
            var track = _timeline.Tracks.FirstOrDefault(x => x.Name == name);

            // Validate track
            if (track == null || !CanAddChildTrack(track))
                return false;

            // Reject dragging parents and itself
            return track != this && !track.ContainsTrack(this);
        }

        /// <summary>
        /// Called when drag and drop moves over the track header area.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Drag action response.</returns>
        protected virtual DragDropEffect OnDragMoveHeader(DragData data)
        {
            return _dragHandlers.Effect;
        }

        /// <summary>
        /// Called when drag and drop performs over the track header area.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns>Drag action response.</returns>
        protected virtual DragDropEffect OnDragDropHeader(DragData data)
        {
            var result = DragDropEffect.None;

            // Use drag positioning to change target parent and index
            Track newParent;
            int newOrder;
            if (_dragOverMode == DragItemPositioning.Above)
            {
                newParent = _parentTrack;
                newOrder = TrackIndex;
            }
            else if (_dragOverMode == DragItemPositioning.Below)
            {
                newParent = _parentTrack;
                newOrder = TrackIndex + 1;
            }
            else
            {
                newParent = this;
                newOrder = (_subTracks.Count > 0 ? _subTracks.Last().TrackIndex : TrackIndex) + 1;
            }

            // Drag tracks
            if (_dragTracks != null && _dragTracks.HasValidDrag)
            {
                var targetTracks = _dragTracks.Objects.ConvertAll(x => _timeline.SelectedTracks.Find(y => y.Name == x));
                for (int i = 0; i < targetTracks.Count; i++)
                {
                    var targetActor = targetTracks[i];
                    targetActor.ParentTrack = newParent;
                    targetActor.TrackIndex = newOrder;
                }
                _timeline.OnTracksOrderChanged();

                result = DragDropEffect.Move;
            }

            // Clear cache
            _dragHandlers.OnDragDrop(null);

            // Expand if dropped sth
            if (result != DragDropEffect.None)
            {
                Expand();
            }

            return result;
        }

        /// <summary>
        /// Called when drag and drop leaves the track header area.
        /// </summary>
        protected virtual void OnDragLeaveHeader()
        {
            _dragHandlers.OnDragLeave();
        }

        /// <summary>
        /// Begins the drag drop operation.
        /// </summary>
        protected virtual void DoDragDrop()
        {
            DragData data;

            // Check if this node is selected
            if (_timeline.SelectedTracks.Contains(this))
            {
                // Get selected tracks
                var names = new List<string>();
                for (var i = 0; i < _timeline.SelectedTracks.Count; i++)
                {
                    var track = _timeline.SelectedTracks[i];
                    if (track.CanDrag)
                        names.Add(track.Name);
                }
                data = DragNames.GetDragData(DragPrefix, names);
            }
            else
            {
                data = DragNames.GetDragData(DragPrefix, Name);
            }

            // Start drag operation
            DoDragDrop(data);
        }

        /// <summary>
        /// Called when expanded state gets changed.
        /// </summary>
        protected virtual void OnExpandedChanged()
        {
            _timeline.ArrangeTracks();
        }

        /// <summary>
        /// Gets a value indicating whether can drag this track.
        /// </summary>
        protected virtual bool CanDrag => true;

        /// <summary>
        /// Determines whether this track can get the child track.
        /// </summary>
        /// <param name="track">The track.</param>
        /// <returns>True if can add this track, otherwise false.</returns>
        protected virtual bool CanAddChildTrack(Track track)
        {
            return false;
        }

        /// <summary>
        /// Updates the drag over mode based on the given mouse location.
        /// </summary>
        /// <param name="location">The location.</param>
        private void UpdateDrawPositioning(ref Vector2 location)
        {
            if (new Rectangle(0, 0 - DefaultDragInsertPositionMargin - DefaultNodeOffsetY, Width, DefaultDragInsertPositionMargin * 2.0f).Contains(location))
                _dragOverMode = DragItemPositioning.Above;
            else if (IsCollapsed && new Rectangle(0, Height - DefaultDragInsertPositionMargin, Width, DefaultDragInsertPositionMargin * 2.0f).Contains(location))
                _dragOverMode = DragItemPositioning.Below;
            else
                _dragOverMode = DragItemPositioning.At;
        }

        /// <summary>
        /// Tests the header hit.
        /// </summary>
        /// <param name="location">The location.</param>
        /// <returns>True if hits it.</returns>
        protected virtual bool TestHeaderHit(ref Vector2 location)
        {
            return new Rectangle(0, 0, Width, Height).Contains(ref location);
        }

        /// <summary>
        /// Starts the track renaming action.
        /// </summary>
        public void StartRenaming()
        {
            _timeline.Select(this, false);

            // Start renaming the track
            var dialog = RenamePopup.Show(this, new Rectangle(Vector2.Zero, Size), Name, false);
            dialog.Renamed += OnRenamed;
        }

        private void OnRenamed(RenamePopup renamePopup)
        {
            if (!_timeline.IsTrackNameValid(renamePopup.Text))
            {
                MessageBox.Show("Invalid name. It must be unique.", "Invalid track name", MessageBox.Buttons.OK, MessageBox.Icon.Warning);
                return;
            }

            OnRename(renamePopup.Text);
        }

        /// <summary>
        /// Called when track should be renamed.
        /// </summary>
        /// <param name="newName">The new name.</param>
        protected virtual void OnRename(string newName)
        {
            Name = newName;
        }

        /// <summary>
        /// Expand track.
        /// </summary>
        public void Expand()
        {
            ExpandAllParents();

            _opened = true;

            OnExpandedChanged();
        }

        /// <summary>
        /// Collapse track.
        /// </summary>
        public void Collapse()
        {
            _opened = false;

            OnExpandedChanged();
        }

        /// <summary>
        /// Expand track and all the children.
        /// </summary>
        public void ExpandAll()
        {
            bool wasLayoutLocked = IsLayoutLocked;
            IsLayoutLocked = true;

            Expand();

            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] is TreeNode node)
                {
                    node.ExpandAll();
                }
            }

            IsLayoutLocked = wasLayoutLocked;
            PerformLayout();
        }

        /// <summary>
        /// Collapse track and all the children.
        /// </summary>
        public void CollapseAll()
        {
            bool wasLayoutLocked = IsLayoutLocked;
            IsLayoutLocked = true;

            Collapse();

            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] is TreeNode node)
                {
                    node.CollapseAll();
                }
            }

            IsLayoutLocked = wasLayoutLocked;
            PerformLayout();
        }

        /// <summary>
        /// Ensure that all track parents are expanded.
        /// </summary>
        public void ExpandAllParents()
        {
            _parentTrack?.Expand();
        }

        /// <inheritdoc />
        public override void Draw()
        {
            // Cache data
            var style = Style.Current;
            bool isSelected = _timeline.SelectedTracks.Contains(this);
            bool isFocused = _timeline.ContainsFocus;
            var left = _xOffset + 16; // offset + arrow
            var height = Height;
            var bounds = new Rectangle(Vector2.Zero, Size);
            var textRect = new Rectangle(left, 0, Width - left, height);
            _margin.ShrinkRectangle(ref textRect);
            var TextColor = style.Foreground;
            var BackgroundColorSelected = style.BackgroundSelected;
            var BackgroundColorHighlighted = style.BackgroundHighlighted;
            var BackgroundColorSelectedUnfocused = style.LightBackground;
            var TextFont = new FontReference(style.FontSmall);
            var isMouseOver = IsMouseOver;

            // Draw background
            if (isSelected || isMouseOver)
            {
                Render2D.FillRectangle(bounds, (isSelected && isFocused) ? BackgroundColorSelected : (isMouseOver ? BackgroundColorHighlighted : BackgroundColorSelectedUnfocused));
            }

            // Draw arrow
            if (SubTracks.Count > 0)
            {
                Render2D.DrawSprite(_opened ? style.ArrowDown : style.ArrowRight, ArrowRect, isMouseOver ? Color.White : new Color(0.8f, 0.8f, 0.8f, 0.8f));
            }

            // Draw icon
            if (Icon.IsValid)
            {
                Render2D.DrawSprite(Icon, new Rectangle(textRect.Left, (height - 16) * 0.5f, 16, 16));
                textRect.X += 18.0f;
                textRect.Width -= 18.0f;
            }

            // Draw text
            Render2D.DrawText(TextFont.GetFont(), Name, textRect, TextColor, TextAlignment.Near, TextAlignment.Center);

            // Draw drag and drop effect
            if (IsDragOver)
            {
                Color dragOverColor = style.BackgroundSelected * 0.6f;
                Rectangle rect;
                switch (_dragOverMode)
                {
                case DragItemPositioning.At:
                    rect = textRect;
                    break;
                case DragItemPositioning.Above:
                    rect = new Rectangle(textRect.X, textRect.Y - DefaultDragInsertPositionMargin - DefaultNodeOffsetY, textRect.Width, DefaultDragInsertPositionMargin * 2.0f);
                    break;
                case DragItemPositioning.Below:
                    rect = new Rectangle(textRect.X, textRect.Bottom - DefaultDragInsertPositionMargin, textRect.Width, DefaultDragInsertPositionMargin * 2.0f);
                    break;
                default:
                    rect = Rectangle.Empty;
                    break;
                }
                Render2D.FillRectangle(rect, dragOverColor);
            }

            base.Draw();
        }


        /// <inheritdoc />
        public override bool OnMouseDown(Vector2 location, MouseButton buttons)
        {
            // Check if mouse hits bar and track isn't a root
            if (IsMouseOver)
            {
                // Check if left button goes down
                if (buttons == MouseButton.Left)
                {
                    _isMouseDown = true;
                    _mouseDownPos = location;
                    _mouseDownTime = Time.UnscaledGameTime;
                }

                // Handled
                Focus();
                return true;
            }

            // Handled
            Focus();
            return true;
        }

        /// <inheritdoc />
        public override bool OnMouseUp(Vector2 location, MouseButton buttons)
        {
            // Check if mouse hits bar
            if (buttons == MouseButton.Right)
            {
                // TODO: show context menu for this track
            }
            else if (buttons == MouseButton.Left)
            {
                // Clear flag
                _isMouseDown = false;
                _mouseDownTime = -1;
            }

            // Prevent from selecting track when user is just clicking at an arrow
            if (!_mouseOverArrow)
            {
                var window = Root;
                if (window.GetKey(Keys.Control))
                {
                    // Add/Remove
                    if (_timeline.SelectedTracks.Contains(this))
                        _timeline.Deselect(this);
                    else
                        _timeline.Select(this, true);
                }
                else
                {
                    // Select
                    _timeline.Select(this, false);
                }
            }

            // Check if mouse hits arrow
            if (SubTracks.Count > 0 && _mouseOverArrow)
            {
                // Toggle open state
                if (_opened)
                    Collapse();
                else
                    Expand();
            }

            // Handled
            Focus();
            return true;
        }

        /// <inheritdoc />
        public override void OnMouseMove(Vector2 location)
        {
            // Cache flag
            _mouseOverArrow = SubTracks.Count > 0 && ArrowRect.Contains(location);

            // Check if start drag and drop
            if (_isMouseDown && Vector2.Distance(_mouseDownPos, location) > 10.0f)
            {
                // Clear flag
                _isMouseDown = false;
                _mouseDownTime = -1;

                // Start
                DoDragDrop();
                return;
            }

            base.OnMouseMove(location);
        }

        /// <inheritdoc />
        public override void OnMouseLeave()
        {
            // Clear flags
            _mouseOverArrow = false;

            // Check if start drag and drop
            if (_isMouseDown)
            {
                // Clear flag
                _isMouseDown = false;
                _mouseDownTime = -1;

                // Start
                DoDragDrop();
            }

            // Base
            base.OnMouseLeave();
        }

        /// <inheritdoc />
        public override DragDropEffect OnDragEnter(ref Vector2 location, DragData data)
        {
            var result = base.OnDragEnter(ref location, data);

            // Check if no children handled that event
            _dragOverMode = DragItemPositioning.None;
            if (result == DragDropEffect.None)
            {
                UpdateDrawPositioning(ref location);

                // Check if mouse is over header
                _isDragOverHeader = TestHeaderHit(ref location);
                if (_isDragOverHeader)
                {
                    // Check if mouse is over arrow
                    if (_children.Count > 0 && ArrowRect.Contains(location))
                    {
                        // Expand track
                        Expand();
                    }

                    result = OnDragEnterHeader(data);
                }

                if (result == DragDropEffect.None)
                    _dragOverMode = DragItemPositioning.None;
            }

            return result;
        }

        /// <inheritdoc />
        public override DragDropEffect OnDragMove(ref Vector2 location, DragData data)
        {
            var result = base.OnDragMove(ref location, data);

            // Check if no children handled that event
            _dragOverMode = DragItemPositioning.None;
            if (result == DragDropEffect.None)
            {
                UpdateDrawPositioning(ref location);

                // Check if mouse is over header
                bool isDragOverHeader = TestHeaderHit(ref location);
                if (isDragOverHeader)
                {
                    // Check if mouse is over arrow
                    if (_children.Count > 0 && ArrowRect.Contains(location))
                    {
                        // Expand track
                        Expand();
                    }

                    if (!_isDragOverHeader)
                        result = OnDragEnterHeader(data);
                    else
                        result = OnDragMoveHeader(data);
                }
                _isDragOverHeader = isDragOverHeader;

                if (result == DragDropEffect.None)
                    _dragOverMode = DragItemPositioning.None;
            }

            return result;
        }

        /// <inheritdoc />
        public override DragDropEffect OnDragDrop(ref Vector2 location, DragData data)
        {
            var result = base.OnDragDrop(ref location, data);

            // Check if no children handled that event
            if (result == DragDropEffect.None)
            {
                UpdateDrawPositioning(ref location);

                // Check if mouse is over header
                if (TestHeaderHit(ref location))
                {
                    result = OnDragDropHeader(data);
                }
            }

            // Clear cache
            _isDragOverHeader = false;
            _dragOverMode = DragItemPositioning.None;

            return result;
        }

        /// <inheritdoc />
        public override void OnDragLeave()
        {
            // Clear cache
            if (_isDragOverHeader)
            {
                _isDragOverHeader = false;
                OnDragLeaveHeader();
            }
            _dragOverMode = DragItemPositioning.None;

            base.OnDragLeave();
        }

        /// <inheritdoc />
        public override bool OnMouseDoubleClick(Vector2 location, MouseButton buttons)
        {
            if (TestHeaderHit(ref location))
            {
                StartRenaming();
                return true;
            }

            return base.OnMouseDoubleClick(location, buttons);
        }

        /// <inheritdoc />
        public override bool OnKeyDown(Keys key)
        {
            if (IsFocused)
            {
                switch (key)
                {
                case Keys.F2:
                    StartRenaming();
                    return true;
                case Keys.Delete:
                    _timeline.DeleteSelection();
                    return true;
                }
                // TODO: track duplicate with Ctrl+D
            }

            return base.OnKeyDown(key);
        }
    }
}