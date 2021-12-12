using System;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;

namespace NamelessEngine
{
    public static class Helpers
    {
        public static TEnum[] GetEnumValues<TEnum>() where TEnum : Enum
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }

        public static Point ToTile(this Vector2 v, Point tileSize)
        {
            var x = (int)(v.X / tileSize.X);
            var y = (int)(v.Y / tileSize.Y);

            return new Point(x, y);
        }

        // FNA doesn't have this but monogame does so adding it into the engine to support FNA
        public static Point ToPoint(this Vector2 v) => new Point((int)v.X, (int)v.Y);

        public static Texture2D LoadTextureFromFile(GraphicsDevice gd, string filePath)
        {
            var t = default(Texture2D);
            // opens up texture and premultiplies alphas so it works properly
            using (var stream = TitleContainer.OpenStream(filePath))
            {
                t = Texture2D.FromStream(gd, stream);

                var buffer = new Color[t.Width * t.Height];
                var count = buffer.Length;
                t.GetData(buffer);
                for (int i = 0; i < count; i++)
                    buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);

                t.SetData(buffer);
            }

            return t;
        }

        public static Texture2D LoadTextureFromMemory(GraphicsDevice gd, byte[] textureMemory, string name)
        {
            var t = default(Texture2D);
            // opens up texture and premultiplies alphas so it works properly
            using (var stream = new MemoryStream(textureMemory))
            {
                t = Texture2D.FromStream(gd, stream);
                t.Name = name;

                var buffer = new Color[t.Width * t.Height];
                var count = buffer.Length;
                t.GetData(buffer);
                for (int i = 0; i < count; i++)
                    buffer[i] = Color.FromNonPremultiplied(buffer[i].R, buffer[i].G, buffer[i].B, buffer[i].A);

                t.SetData(buffer);
            }

            return t;
        }

        public static SoundEffect LoadAudioFromFile(string path)
        {
            var sound = default(SoundEffect);
            using (var stream = TitleContainer.OpenStream(path))
            {
                sound = SoundEffect.FromStream(stream);
                sound.Name = path;
            }

            return sound;
        }

        // gets all files names in specified path. This makes it easy to load all of your textures, audio files, etc. if you put them into folders
        // (BGM, soundclips, sprites, etc.)
        public static FileInfo[] GetFileNamesFromFolder(string path)
        {
            var dir = new DirectoryInfo(path);
            return !dir.Exists ? new FileInfo[] { } : dir.GetFiles("*.*");
        }

        public static SoundEffect[] LoadWavFilesFromFolder(FileInfo[] fileNames)
        {
            var effects = new SoundEffect[fileNames.Length];

            for (int i = 0; i < fileNames.Length; i++)
            {
                effects[i] = LoadAudioFromFile(fileNames[i].FullName);
                effects[i].Name = fileNames[i].Name.Replace(".wav", string.Empty);
            }

            return effects;
        }

        public static Texture2D[] LoadTexturesFromFolder(GraphicsDevice gd, FileInfo[] fileNames)
        {
            var textures = new Texture2D[fileNames.Length];

            for (int i = 0; i < fileNames.Length; i++)
            {
                textures[i] = LoadTextureFromFile(gd, fileNames[i].FullName);
                textures[i].Name = fileNames[i].Name.Replace(".png", string.Empty);
            }

            return textures;
        }
    }
}
