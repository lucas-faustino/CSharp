using Microsoft.Xna.Framework;

namespace NamelessEngine.UI
{
    public readonly struct FontText
    {
        public readonly char[] Characters;
        public readonly Rectangle Rect;
        public readonly byte Width;
        public readonly byte Height;

        public FontText(char[] characters, Rectangle rect, byte width, byte height)
        {
            Characters = characters;
            Rect = rect;
            Width = width;
            Height = height;
        }

        public int GetIndexChar(char c)
        {
            var count = Characters.Length;;
            for (int i = 0; i < count; i++)
            {
		        if (Characters[i] == c)
			        return i;
            }
            // instead of crashing, just return whichever character is first
            return 0;
        }
    }
}
