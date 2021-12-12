using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NamelessEngine.UI
{
    public sealed class SpriteManager
    {
        const int STARTING_SIZE = 32;

        readonly SparseSet _ss = new SparseSet();

        Vector2[] _positions = new Vector2[STARTING_SIZE];
        Color[] _colors = new Color[STARTING_SIZE];
        SpriteLoop[] _loops = new SpriteLoop[STARTING_SIZE];
        Texture2D[] _textures = new Texture2D[STARTING_SIZE];

        public GenerationalIndex? Create(Texture2D texture, Vector2 position, Color color, SpriteLoop loop)
        {
            if(_ss.Insert() is GenerationalIndex id)
            {
                // We are working with mutable structs and Lists return copies but arrays do not. We also want our arrays to grow automatically
                // so we use the resize method whenever the index is larger than the array sizes. STARTING_SIZE is the default size so no resizing will happen
                // if the size limit is never exceeded
                // any array is fine for resizing
                if (_positions.Length <= id.Index)
                {
                    Array.Resize(ref _positions, id.Index);
                    Array.Resize(ref _colors, id.Index);
                    Array.Resize(ref _loops, id.Index);
                    Array.Resize(ref _textures, id.Index);
                }

                _positions[id.Index] = position;
                _colors[id.Index] = color;
                _loops[id.Index] = loop;
                _textures[id.Index] = texture;
                return id;
            }

            return null;
        }

        public void RemoveAt(GenerationalIndex gen)
        {
            // keep arrays packed with a swap
            if(_ss.RemoveAt(gen) is GenerationalIndex id)
            {
                _positions[id.Index] = _positions[_ss.Count - 1];
                _colors[id.Index] = _colors[_ss.Count - 1];
                _loops[id.Index] = _loops[_ss.Count - 1];
            }
        }

        public void CenterAt(GenerationalIndex gen, Vector2 center, Rectangle bounds)
        {
            if (_ss.IsInSet(gen))
            {
                var p = _positions[gen.Index];
                _positions[gen.Index] = new Vector2(
                    p.X - (bounds.Width / 2),
                    p.Y - (bounds.Height / 2)
                );
            }
        }

        public void SetPosition(GenerationalIndex gen, Vector2 position)
        {
            if (_ss.IsInSet(gen))
                _positions[gen.Index] = position;
        }

        public void SetColor(GenerationalIndex gen, Color color)
        {
            if (_ss.IsInSet(gen))
                _colors[gen.Index] = color;
        }

        public void SetLoop(GenerationalIndex gen, SpriteLoop loop)
        {
            if (_ss.IsInSet(gen))
                _loops[gen.Index] = loop;
        }

        public void SetTexture(GenerationalIndex gen, Texture2D texture)
        {
            if (_ss.IsInSet(gen))
                _textures[gen.Index] = texture;
        }

        public void SetSprite(GenerationalIndex gen, Texture2D texture, Vector2 position, Color color, SpriteLoop loop)
        {
            if(_ss.IsInSet(gen))
            {
                _textures[gen.Index] = texture;
                _positions[gen.Index] = position;
                _colors[gen.Index] = color;
                _loops[gen.Index] = loop;
            }
        }

        public void Clear()
        {
            _ss.Clear();
        }

        public void Update(float dt)
        {
            for (int i = 0; i < _ss.Count; i++)
                SpriteLoop.Update(ref _loops[i], dt);
        }

        public void Draw(SpriteBatch sb)
        {
            for (int i = 0; i < _ss.Count; i++)
                sb.Draw(_textures[i], _positions[i], _loops[i].GetCurrentFrame(), _colors[i]);
        }
    }
}
