using System;
using Microsoft.Xna.Framework;

namespace NamelessEngine
{
    /// <summary>
    /// Camera class. By default, it will simply show the game view
    /// as if it didn't exist. So games that don't need the viewport to
    /// change can ignore this class
    /// </summary>
    public sealed class Camera
    {
        readonly NativeScreen _nativeScreen;
        readonly Point _tileSize;

        public float Zoom = 1f;
        public float Rotation = 0f;
        public Vector2 Position = Vector2.Zero;

        public Camera(NativeScreen nativeScreen, Point tileSize)
        {
            _nativeScreen = nativeScreen;
            _tileSize = tileSize;
            // sets the camera to (0, 0)
            Position = new Vector2(_nativeScreen.CenterX, _nativeScreen.CenterY);
        }

        public bool IsOnScreen(Rectangle rect)
        {
            var offset = GetPosOffset;
            return new Rectangle((int)offset.X, (int)offset.Y, _nativeScreen.Width, _nativeScreen.Height).Intersects(rect);
        }

        internal Matrix ViewPort()
        {
            return Matrix.CreateTranslation(new Vector3(-Position.X, -Position.Y, 0)) *
                                         Matrix.CreateRotationZ(Rotation) *
                                         Matrix.CreateScale(new Vector3(Zoom, Zoom, 1)) *
                                         Matrix.CreateTranslation(new Vector3(_nativeScreen.CenterX, _nativeScreen.CenterY, 0));
        }

        public Point GetDrawBoundsMin => GetPosTile;

        public Point GetDrawBoundsMax
        {
            get
            {
                var pos = GetPosTile;
                var x = pos.X + (_nativeScreen.Width / _tileSize.X);
                var y = pos.Y + (_nativeScreen.Height / _tileSize.Y);

                return new Point(x, y);
            }
        }

        public Point GetPosTile
        {
            get
            {
                var x = (int)Position.X - _nativeScreen.CenterX;
                var y = (int)Position.Y - _nativeScreen.CenterY;

                return new Vector2(x, y).ToTile(_tileSize);
            }
        }

        public Vector2 GetPosOffset => new Vector2(Position.X - _nativeScreen.CenterX, Position.Y - _nativeScreen.CenterY);

        public void MaxPositionClamp(int maxX, int maxY)
        {
            if (Position.X > (maxX - (_nativeScreen.CenterX / Zoom)))
                Position.X = (maxX - (_nativeScreen.CenterX / Zoom));

            if (Position.Y > (maxY - (_nativeScreen.CenterY / Zoom)))
                Position.Y = (maxY - (_nativeScreen.CenterY / Zoom));
        }

        public void MinPositionClamp(int minX, int minY)
        {
            if (Position.X < (minX + _nativeScreen.CenterX / Zoom))
                Position.X = (minX + _nativeScreen.CenterX / Zoom);

            if (Position.Y < (minY + _nativeScreen.CenterY / Zoom))
                Position.Y = (minY + _nativeScreen.CenterY / Zoom);
        }

        public void ZeroPositionClamp() => MinPositionClamp(0, 0);

        public void RoundToPixel()
        {
            Position.X = (float)Math.Round(Position.X);
            Position.Y = (float)Math.Round(Position.Y);
        }

        /// <summary>
        /// Resets all camera field values to default
        /// </summary>
        public void Reset()
        {
            Zoom = 1f;
            Position = new Vector2(_nativeScreen.CenterX, _nativeScreen.CenterY);
            Rotation = 0f;
        }
    }
}
