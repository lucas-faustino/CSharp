using Microsoft.Xna.Framework;

namespace NamelessEngine
{
    public readonly struct NativeScreen
    {
        public readonly int Width;
        public readonly int Height;

        public int CenterX => Width / 2;
        public int CenterY => Height / 2;
        public Point Size => new Point(Width, Height);
        public Rectangle Bounds => new Rectangle(0, 0, Width, Height);
        public Vector2 CenterScreen => new Vector2(CenterX, CenterY);

        public NativeScreen(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
