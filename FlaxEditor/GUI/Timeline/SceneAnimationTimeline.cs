// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.

using System;
using FlaxEngine;

namespace FlaxEditor.GUI.Timeline
{
    /// <summary>
    /// The timeline editor for scene animation asset.
    /// </summary>
    /// <seealso cref="FlaxEditor.GUI.Timeline.Timeline" />
    public sealed class SceneAnimationTimeline : Timeline
    {
        private SceneAnimationPlayer _player;

        /// <summary>
        /// Gets or sets the animation player actor used for the timeline preview.
        /// </summary>
        public SceneAnimationPlayer Player
        {
            get => _player;
            set
            {
                if (_player == value)
                    return;

                _player = value;

                UpdatePlaybackState();
                PlayerChanged?.Invoke();
            }
        }

        /// <summary>
        /// Occurs when the selected player gets changed.
        /// </summary>
        public event Action PlayerChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="SceneAnimationTimeline"/> class.
        /// </summary>
        public SceneAnimationTimeline()
        : base(PlaybackButtons.Play | PlaybackButtons.Stop)
        {
            PlaybackState = PlaybackStates.Disabled;

            // Setup track types
            TrackArchetypes.Add(FolderTrack.GetArchetype());
            TrackArchetypes.Add(PostProcessMaterialTrack.GetArchetype());
        }

        private void UpdatePlaybackState()
        {
            PlaybackStates state;
            if (!_player)
                state = PlaybackStates.Disabled;
            else if (_player.IsPlaying)
                state = PlaybackStates.Playing;
            else if (_player.IsPaused)
                state = PlaybackStates.Paused;
            else if (_player.IsStopped)
                state = PlaybackStates.Stopped;
            else
                state = PlaybackStates.Disabled;
            PlaybackState = state;
            CurrentFrame = _player ? (int)(_player.Time * _player.Animation.DurationFrames) : 0;
        }

        /// <inheritdoc />
        public override void OnPlay()
        {
            _player.Play();

            base.OnPlay();
        }

        /// <inheritdoc />
        public override void OnPause()
        {
            _player.Pause();

            base.OnPause();
        }

        /// <inheritdoc />
        public override void OnStop()
        {
            _player.Stop();

            base.OnStop();
        }

        /// <inheritdoc />
        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            UpdatePlaybackState();
        }

        /// <summary>
        /// Loads the timeline from the specified <see cref="FlaxEngine.SceneAnimation"/> asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        public void Load(SceneAnimation asset)
        {
            var data = asset.LoadTimeline();
            Load(data);
        }

        /// <summary>
        /// Saves the timeline data to the <see cref="FlaxEngine.SceneAnimation"/> asset.
        /// </summary>
        /// <param name="asset">The asset.</param>
        public void Save(SceneAnimation asset)
        {
            var data = Save();
            asset.SaveTimeline(data);
        }
    }
}