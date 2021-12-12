using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NamelessEngine.UI
{
    public sealed class TextManager
    {
        const int STARTING_SIZE = 32;

        readonly struct CharSpan
        {
            public readonly ushort Start;
            public readonly ushort Total;

            public CharSpan(ushort start, ushort count)
            {
                Start = start;
                Total = count;
            }
        }

        readonly SparseSet _ss = new SparseSet();

        StringBuilder[] _texts = new StringBuilder[STARTING_SIZE];
        Vector2[] _positions = new Vector2[STARTING_SIZE];
        Color[] _colors = new Color[STARTING_SIZE];
        Texture2D[] _textures = new Texture2D[STARTING_SIZE];
        FontText[] _fonts = new FontText[STARTING_SIZE];
        ushort[] _maxLineWidths = new ushort[STARTING_SIZE];
        byte[] _textScales = new byte[STARTING_SIZE];
        List<CharSpan>[] _spans = new List<CharSpan>[STARTING_SIZE];

        private int GetTextLength(int index, int fontWidth)
        {
            var length = 0;
            foreach (var span in _spans[index])
                length += span.Total;

            return length * fontWidth * _textScales[index];
        }

        public TextManager()
        {
            for (int i = 0; i < STARTING_SIZE; i++)
            {
                _texts[i] = new StringBuilder();
                _spans[i] = new List<CharSpan>();
            }
        }

        public GenerationalIndex? Create(
            string text, 
            Vector2 position, 
            Color color, 
            FontText font, 
            Texture2D texture, 
            ushort maxLineWidth, 
            byte scale)
        {
            if(_ss.Insert() is GenerationalIndex handle)
            {
                // We are working with mutable structs and Lists return copies but arrays do not. We also want our arrays to grow automatically
                // so we use the resize method whenever the index is larger than the array sizes. STARTING_SIZE is the default size so no resizing will happen
                // if the size limit is never exceeded
                // any array is fine for resizing
                if (_positions.Length <= handle.Index)
                {
                    Array.Resize(ref _texts, handle.Index);
                    Array.Resize(ref _positions, handle.Index);
                    Array.Resize(ref _colors, handle.Index);
                    Array.Resize(ref _textures, handle.Index);
                    Array.Resize(ref _fonts, handle.Index);
                    Array.Resize(ref _maxLineWidths, handle.Index);
                    Array.Resize(ref _textScales, handle.Index);
                    Array.Resize(ref _spans, handle.Index);
                    // allocate memory for nullable types
                    for (int i = 0; i < handle.Index; i++)
                    {
                        if (_texts[i] == null)
                            _texts[i] = new StringBuilder();

                        if (_spans[i] == null)
                            _spans[i] = new List<CharSpan>();
                    }
                }

                _positions[handle.Index] = position;
                _colors[handle.Index] = color;
                _fonts[handle.Index] = font;
                _textures[handle.Index] = texture;
                _maxLineWidths[handle.Index] = maxLineWidth;
                _textScales[handle.Index] = scale;
                _spans[handle.Index].Clear();
                SetText(handle, text);
                return handle;
            }

            return null;
        }

        // once the item is removed, it won't be looped over so clearing the data is useless
        public GenerationalIndex? RemoveAt(GenerationalIndex handle)
        {
            if (_ss.RemoveAt(handle) is GenerationalIndex id)
                return id;
            else
                return null;
        }

        public void SetColor(GenerationalIndex handle, Color color)
        {
            if (_ss.IsInSet(handle))
                _colors[handle.Index] = color;
        }

        public void SetFont(GenerationalIndex handle, FontText font)
        {
            if (_ss.IsInSet(handle))
                _fonts[handle.Index] = font;
        }

        public void SetTexture(GenerationalIndex handle, Texture2D texture)
        {
            if (_ss.IsInSet(handle))
                _textures[handle.Index] = texture;
        }

        public void SetMaxLineWidth(GenerationalIndex handle, ushort maxLineWidth)
        {
            if (_ss.IsInSet(handle))
                _maxLineWidths[handle.Index] = maxLineWidth;
        }

        public void SetTextObject(
            GenerationalIndex handle,
            string text,
            Vector2 position,
            Color color,
            FontText font,
            Texture2D texture,
            ushort maxLineWidth,
            byte scale)
        {
            if(_ss.IsInSet(handle))
            {
                _positions[handle.Index] = position;
                _colors[handle.Index] = color;
                _fonts[handle.Index] = font;
                _textures[handle.Index] = texture;
                _maxLineWidths[handle.Index] = maxLineWidth;
                _textScales[handle.Index] = scale;
                _spans[handle.Index].Clear();
                SetText(handle, text);
            }
        }

        public void SetText(GenerationalIndex handle, string text)
        {
            if(_ss.IsInSet(handle))
            {
                const char space = ' ';

                _texts[handle.Index].Clear();
                _texts[handle.Index].Append(text);

                var wordLength = 0;
                var index = 0;

                for (int i = 0; i < text.Length; i++)
                {
                    wordLength++;

                    if (text[i] == space || i == (text.Length - 1))
                    {
                        _spans[handle.Index].Add(new CharSpan(
                            start: (ushort)index, 
                            count: (ushort)wordLength
                        ));
                        // increase index before resetting word length
                        index += wordLength;
                        wordLength = 0;
                    }
                }
            }
        }

        public void CenterText(GenerationalIndex handle, Vector2 center, Point font)
        {
            if(_ss.IsInSet(handle))
            {
                _positions[handle.Index] = new Vector2(
                    x: center.X - (GetTextLength(handle.Index, font.X) / 2),
                    y: center.Y - (font.Y * _textScales[handle.Index] / 2)
                );
            }
        }

        public void CenterX(GenerationalIndex handle, int center, Point font)
        {
            if (_ss.IsInSet(handle))
                _positions[handle.Index].X = center - (GetTextLength(handle.Index, font.X) / 2);
        }

        public void SetAndPlaceAt(GenerationalIndex handle, string text, Vector2 position)
        {
            if (_ss.IsInSet(handle))
            {
                SetText(handle, text);
                _positions[handle.Index] = position;
            }
        }

        public void SetAndCenterAt(GenerationalIndex handle, string text, Vector2 center, Point font)
        {
            if (_ss.IsInSet(handle))
            {
                SetText(handle, text);
                CenterText(handle, center, font);
            }
        }

        public void SetAndCenterAtX(GenerationalIndex handle, string text, int center, Point font)
        {
            if (_ss.IsInSet(handle))
            {
                SetText(handle, text);
                CenterX(handle, center, font);
            }
        }

        public void ToggleVisibility(GenerationalIndex handle, bool isInvisible)
        {
            if (_ss.IsInSet(handle))
                _colors[handle.Index].A = isInvisible ? byte.MinValue : byte.MaxValue;
        }

        // this clears the sparseset so data doesn't need to be cleared
        public void Reset()
        {
            _ss.Clear();
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < _ss.Count; i++)
            {
                var font = _fonts[i];
                var maxLettersPerLine = _maxLineWidths[i] / font.Width;
                var scale = _textScales[i];
                var position = _positions[i];
                var color = _colors[i];
                var texture = _textures[i];
                var text = _texts[i];
                var spans = _spans[i];
                var lineHeight = 0;
                var wordIndex = 0;

                foreach (var span in spans)
                {
                    // make a new line once the line limit is reached
                    if ((wordIndex + span.Total) > maxLettersPerLine)
                    {
                        wordIndex = 0;
                        lineHeight++;
                    }

                    for (int j = 0; j < span.Total; j++)
                    {
                        // draw out each letter of the word word
                        var c = text[span.Start + j];
                        var xOffset = font.Width * wordIndex * scale;
                        var yOffset = (font.Height + (font.Height / 2)) * lineHeight * scale;
                        var pos = new Vector2(x: position.X + xOffset, y: position.Y + yOffset);
                        var drawRect = new Rectangle(
                            x: font.Rect.X + (font.Width * font.GetIndexChar(c)), 
                            y: font.Rect.Y, 
                            width: font.Width, 
                            height: font.Height
                        );

                        sb.Draw(texture, pos, drawRect, color);
                        wordIndex++;
                    }
                }
            }
        }
    }
}
