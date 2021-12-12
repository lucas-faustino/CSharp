using Microsoft.Xna.Framework;

namespace NamelessEngine
{
    public struct Timer
    {
        public readonly float Delay;

        public float NextReset;

        public bool IsElapsed => NextReset <= 0f;

        public Timer(float delay, float nextReset = 0f, bool isReset = true)
        {
            Delay = delay;
            NextReset = 0;

            if (isReset)
                Reset(ref this);
        }

        public static void Tick(ref Timer t, float dt) => t.NextReset -= dt;
        public static void Reset(ref Timer t) => t.NextReset = t.Delay;
    }

    public struct Animation
    {
        /// <summary>
        /// The rect area of the texture (or entire one if one texture is the sprite)
        /// </summary>
        public Rectangle Rect;
        public int FrameWidth;

        public Rectangle GetFrameAt(int frame) => new Rectangle(x: Rect.X + (FrameWidth * frame), y: Rect.Y, width: FrameWidth == 0 ? Rect.Width : FrameWidth, height: Rect.Height);
        public int GetTotalFrames() => FrameWidth == 0 ? 1 : (Rect.Width / FrameWidth);
        public int GetFrameHeight() => Rect.Height;
    }

    public struct SpriteLoop
    {
        public Animation Anim;
        public Timer Timer;
        public int FrameIndex;

        public bool IsLastFrame() => FrameIndex == Anim.GetTotalFrames() - 1;
        public Rectangle GetCurrentFrame() => Anim.GetFrameAt(FrameIndex);

        public static void Update(ref SpriteLoop loop, float dt)
        {
            Timer.Tick(ref loop.Timer, dt);

            if (loop.Timer.IsElapsed)
            {
                if (++loop.FrameIndex >= loop.Anim.GetTotalFrames())
                    loop.FrameIndex = 0;

                Timer.Reset(ref loop.Timer);
            }
        }
    }
}
