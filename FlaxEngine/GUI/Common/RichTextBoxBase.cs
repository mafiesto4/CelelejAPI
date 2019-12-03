// Copyright (c) 2012-2019 Wojciech Figat. All rights reserved.

using System;
using System.Collections.Generic;

namespace FlaxEngine.GUI
{
    /// <summary>
    /// Base class for all rich text box controls which can gather text input from the user and present text in highly formatted and stylized way.
    /// </summary>
    public abstract class RichTextBoxBase : TextBoxBase
    {
        /// <summary>
        /// The delegate for text blocks processing.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="textBlocks">The output text blocks. Given list is not-nul and cleared before.</param>
        public delegate void ParseTextBlocksDelegate(string text, List<TextBlock> textBlocks);

        /// <summary>
        /// The text blocks.
        /// </summary>
        protected List<TextBlock> _textBlocks = new List<TextBlock>();

        /// <summary>
        /// The custom callback for parsing text blocks.
        /// </summary>
        [HideInEditor]
        public ParseTextBlocksDelegate ParseTextBlocks;

        /// <summary>
        /// Initializes a new instance of the <see cref="RichTextBoxBase"/> class.
        /// </summary>
        protected RichTextBoxBase()
        {
            IsMultiline = true;
        }

        /// <summary>
        /// Updates the text blocks.
        /// </summary>
        protected void UpdateTextBlocks()
        {
            Profiler.BeginEvent("RichTextBoxBase.UpdateTextBlocks");

            _textBlocks.Clear();
            if (_text.Length != 0)
            {
                OnParseTextBlocks();
            }

            Profiler.EndEvent();
        }

        /// <summary>
        /// Called when text blocks needs to be updated from the current text.
        /// </summary>
        protected virtual void OnParseTextBlocks()
        {
            ParseTextBlocks?.Invoke(_text, _textBlocks);
        }

        /// <inheritdoc />
        protected override void OnTextChanged()
        {
            UpdateTextBlocks();

            base.OnTextChanged();
        }

        /// <inheritdoc />
        public override Vector2 GetTextSize()
        {
            var count = _textBlocks.Count;
            var textBlocks = Utils.ExtractArrayFromList(_textBlocks);
            var max = Vector2.Zero;
            for (int i = 0; i < count; i++)
            {
                ref TextBlock textBlock = ref textBlocks[i];
                max = Vector2.Max(max, textBlock.Bounds.BottomRight);
            }
            return max;
        }

        /// <inheritdoc />
        public override Vector2 GetCharPosition(int index, out float height)
        {
            var count = _textBlocks.Count;
            var textBlocks = Utils.ExtractArrayFromList(_textBlocks);

            if (count == 0)
            {
                height = 0;
                return Vector2.Zero;
            }
            if (index == 0)
            {
                ref TextBlock textBlock = ref textBlocks[0];
                var font = textBlock.Style.Font.GetFont();
                if (font)
                {
                    height = font.Height;
                    return textBlock.Bounds.UpperLeft;
                }
            }

            for (int i = 0; i < count; i++)
            {
                ref TextBlock textBlock = ref textBlocks[i];

                if (textBlock.Range.Contains(index))
                {
                    var font = textBlock.Style.Font.GetFont();
                    if (!font)
                        break;
                    height = font.Height;
                    return textBlock.Bounds.Location + font.GetCharPosition(_text, ref textBlock.Range, index - textBlock.Range.StartIndex);
                }
            }

            if (count != 0)
            {
                ref TextBlock textBlock = ref textBlocks[count - 1];
                var font = textBlock.Style.Font.GetFont();
                if (font)
                {
                    height = font.Height;
                    return textBlock.Bounds.UpperLeft;
                }
            }

            height = 0;
            return Vector2.Zero;
        }

        /// <inheritdoc />
        public override int HitTestText(Vector2 location)
        {
            location = Vector2.Clamp(location, Vector2.Zero, _textSize);

            var textBlocks = Utils.ExtractArrayFromList(_textBlocks);
            var count = _textBlocks.Count;
            for (int i = 0; i < count; i++)
            {
                ref TextBlock textBlock = ref textBlocks[i];

                var containsX = location.X >= textBlock.Bounds.Location.X && location.X < textBlock.Bounds.Location.X + textBlock.Bounds.Size.X;
                var containsY = location.Y >= textBlock.Bounds.Location.Y && location.Y < textBlock.Bounds.Location.Y + textBlock.Bounds.Size.Y;

                if (containsY && (containsX || (i + 1 < count && textBlocks[i + 1].Bounds.Location.Y > textBlock.Bounds.Location.Y + 1.0f)))
                {
                    var font = textBlock.Style.Font.GetFont();
                    if (!font)
                        break;
                    return font.HitTestText(_text, ref textBlock.Range, location - textBlock.Bounds.Location) + textBlock.Range.StartIndex;
                }
            }

            return _text.Length;
        }

        /// <inheritdoc />
        public override void Draw()
        {
            // Cache data
            var rect = new Rectangle(Vector2.Zero, Size);
            bool enabled = EnabledInHierarchy;

            // Background
            Color backColor = BackgroundColor;
            if (IsMouseOver)
                backColor = BackgroundSelectedColor;
            Render2D.FillRectangle(rect, backColor);
            Render2D.DrawRectangle(rect, IsFocused ? BorderSelectedColor : BorderColor);

            // Apply view offset and clip mask
            Render2D.PushClip(TextClipRectangle);
            bool useViewOffset = !_viewOffset.IsZero;
            if (useViewOffset)
                Render2D.PushTransform(Matrix3x3.Translation2D(-_viewOffset));

            // Text Blocks
            var textBlocks = Utils.ExtractArrayFromList(_textBlocks);
            var hasSelection = HasSelection;
            var selection = new TextRange(SelectionLeft, SelectionRight);
            var viewRect = new Rectangle(_viewOffset, Size);
            for (int i = 0; i < _textBlocks.Count; i++)
            {
                ref TextBlock textBlock = ref textBlocks[i];

                // Skip blocks not in the view
                if (!textBlock.Bounds.Intersects(ref viewRect))
                    continue;

                // Pick font
                var font = textBlock.Style.Font.GetFont();
                if (!font)
                    continue;

                // Selection
                if (hasSelection && textBlock.Style.BackgroundSelectedBrush != null && textBlock.Range.Intersect(ref selection))
                {
                    var selectionLeft = Math.Max(selection.StartIndex, textBlock.Range.StartIndex);
                    var selectionEnd = Math.Min(selection.EndIndex, textBlock.Range.EndIndex);
                    Vector2 leftEdge = font.GetCharPosition(_text, selectionLeft);
                    Vector2 rightEdge = font.GetCharPosition(_text, selectionEnd);
                    float alpha = Mathf.Min(1.0f, Mathf.Cos(_animateTime * 6.0f) * 0.5f + 1.3f);
                    alpha = alpha * alpha;
                    if (!IsFocused)
                        alpha = 0.1f;
                    Color selectionColor = Color.White * alpha;
                    Rectangle selectionRect = new Rectangle(leftEdge.X, leftEdge.Y, rightEdge.X - leftEdge.X, font.Height);
                    textBlock.Style.BackgroundSelectedBrush.Draw(selectionRect, selectionColor);
                }

                // Shadow
                Color color;
                if (!textBlock.Style.ShadowOffset.IsZero && textBlock.Style.ShadowColor != Color.Transparent)
                {
                    color = textBlock.Style.ShadowColor;
                    if (!enabled)
                        color *= 0.6f;
                    Render2D.DrawText(font, _text, ref textBlock.Range, color, textBlock.Bounds.Location + textBlock.Style.ShadowOffset, textBlock.Style.CustomMaterial);
                }

                // Text
                color = textBlock.Style.Color;
                if (!enabled)
                    color *= 0.6f;
                Render2D.DrawText(font, _text, ref textBlock.Range, color, textBlock.Bounds.Location, textBlock.Style.CustomMaterial);

                // Underline
                if (textBlock.Style.UnderlineBrush != null)
                {
                    var underLineHeight = 2.0f;
                    var underlineRect = new Rectangle(textBlock.Bounds.Location.X, textBlock.Bounds.Location.Y + font.Height - underLineHeight * 0.5f, textBlock.Bounds.Width, underLineHeight);
                    textBlock.Style.UnderlineBrush.Draw(underlineRect, Color.White);
                }
            }

            // Caret
            if (IsFocused && CaretPosition > -1)
            {
                float alpha = Mathf.Saturate(Mathf.Cos(_animateTime * Mathf.TwoPi) * 0.5f + 0.7f);
                alpha = alpha * alpha * alpha * alpha * alpha * alpha;
                Render2D.FillRectangle(CaretBounds, CaretColor * alpha);
            }

            // Restore rendering state
            if (useViewOffset)
                Render2D.PopTransform();
            Render2D.PopClip();
        }

        /// <inheritdoc />
        public override void OnDestroy()
        {
            _textBlocks.Clear();
            _textBlocks = null;

            base.OnDestroy();
        }
    }
}
