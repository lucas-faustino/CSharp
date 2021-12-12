using Microsoft.Xna.Framework.Graphics;

namespace NamelessEngine
{
    public sealed class TextureManager
    {
        // this isn't read only because you have to load textures in the engine Load() method
        Texture2D[] _textures;

        public Texture2D Get(int index)
        {
            return _textures[index];
        }

        internal void LoadContent(Texture2D[] textures)
        {
            _textures = textures;
        }

        internal void Unload()
        {
            foreach (var t in _textures)
                t.Dispose();
        }
    }
}
