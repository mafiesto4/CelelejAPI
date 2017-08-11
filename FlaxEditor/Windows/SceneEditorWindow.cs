﻿////////////////////////////////////////////////////////////////////////////////////
// Copyright (c) 2012-2017 Flax Engine. All rights reserved.
////////////////////////////////////////////////////////////////////////////////////

using FlaxEngine;
using FlaxEngine.GUI;

namespace FlaxEditor.Windows
{
    /// <summary>
    /// Base class for editor windows dedicated to scene editing.
    /// </summary>
    /// <seealso cref="FlaxEditor.Windows.EditorWindow" />
    public abstract class SceneEditorWindow : EditorWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SceneEditorWindow"/> class.
        /// </summary>
        /// <param name="editor">The editor.</param>
        /// <param name="hideOnClose">True if hide window on closing, otherwise it will be destroyed.</param>
        /// <param name="scrollBars">The scroll bars.</param>
        protected SceneEditorWindow(Editor editor, bool hideOnClose, ScrollBars scrollBars)
            : base(editor, hideOnClose, scrollBars)
        {
        }

        /// <summary>
        /// Saves all changes.
        /// </summary>
        public void SaveAll()
        {
            Editor.SaveAll();
        }

        /// <inheritdoc />
        public override bool OnKeyDown(KeyCode key)
        {
            // Base
            bool result = base.OnKeyDown(key);
            if (!result)
            {
                if (ParentWindow.GetKey(KeyCode.CONTROL))
                {
                    switch (key)
                    {
                        case KeyCode.S: Editor.SaveAll(); return true;
                        case KeyCode.Z: Editor.PerformUndo(); return true;
                        case KeyCode.Y: Editor.PerformRedo(); return true;
                    }
                }
            }

            return result;
        }
    }
}
