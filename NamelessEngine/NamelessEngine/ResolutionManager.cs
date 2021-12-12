using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NamelessEngine
{
    public enum PixelMode : byte
    {
        Perfect,
        PreserveAspect,
        StretchToFit
    }

    public sealed class ResolutionManager
    {
        NativeScreen _nativeScreen;
        RenderTarget2D _virtualScreen;
        Rectangle _rect;
        Point _offset;
        float _scaleX;
        float _scaleY;

        public PixelMode PixelMode { get; internal set; } = PixelMode.Perfect;

        public Point Offset => _offset;
        public Vector2 Scale => new Vector2(_scaleX, _scaleY);

        internal ResolutionManager(NativeScreen nativeScreen)
        {
            _nativeScreen = nativeScreen;
        }

        internal void Initialize(GraphicsDevice gd)
        {
            _virtualScreen = new RenderTarget2D(gd, _nativeScreen.Width, _nativeScreen.Height, false, SurfaceFormat.Color, DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
        }

        internal void DrawGame(SpriteBatch sb, GraphicsDevice gd, SceneManager sceneManager, EngineContext context, IDebug debug)
        {
            // clear to black so black bars will appear for resolutions that don't scale perfectly
            gd.Clear(Color.Black);
            // draw game to render target
            gd.SetRenderTarget(_virtualScreen);
            {
                // draw game
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, context.Camera.ViewPort());
                {
                    sceneManager.DrawGame(sb, context);
                    debug.Draw(sb, context);
                }
                sb.End();
                // draw UI
                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
                {
                    sceneManager.DrawGameUI(sb, context);
                    debug.DrawUI(sb, context);
                }
                sb.End();
            }
            // null out here to send image to backbuffer
            gd.SetRenderTarget(null);
            // draw render texture and make sure it is point filtered
            sb.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone);
            sb.Draw(_virtualScreen, _rect, Color.White);
            sb.End();
        }

        internal void SetScale(GameWindow window)
        {
            const int half = 2;

            _rect = default;
            var scaleX = (float)window.ClientBounds.Width / _nativeScreen.Width;
            var scaleY = (float)window.ClientBounds.Height / _nativeScreen.Height;
            // scale rectangle based on display mode
            switch (PixelMode)
            {
                // give maximum int value that height divides into to
                // preserve pixel perfect ratio. Black bars on sides or top if resolution doesn't fit screen
                case PixelMode.Perfect:
                    {
                        // scale by height as the goal is to only have black bars on the sides. Not always
                        // possible but that's the goal.
                        var finalWidth = _nativeScreen.Width * (int)scaleY;
                        var finalHeight = _nativeScreen.Height * (int)scaleY;
                        // determine how many black pixels will be drawn and divide by 2 to
                        // center screen (anything not taken up by the game will render as black)
                        var pointX = (window.ClientBounds.Width - finalWidth) / half;
                        var pointY = (window.ClientBounds.Height - finalHeight) / half;
                        _rect = new Rectangle(pointX, pointY, finalWidth, finalHeight);
                        _offset.X = pointX;
                        _offset.Y = pointY;
                        // height is in pixel scale so both values will match scaleY and must be
                        // whole number to keep pixel perfection
                        _scaleX = (int)scaleY;
                        _scaleY = (int)scaleY;
                    }
                    break;
                // stretch image to preserve aspect. This can result in losing
                // some viewport area and can be stretched by floating point numbers
                case PixelMode.PreserveAspect:
                    {
                        // return larger scale so image maintains aspect ratio
                        var finalScale = scaleX > scaleY ? scaleX : scaleY;
                        var finalWidth = (int)Math.Round(_nativeScreen.Width * finalScale);
                        var finalHeight = (int)Math.Round(_nativeScreen.Height * finalScale);
                        // we need to center the image to to the screen so we'll
                        // find the draw point here
                        var pointX = (finalWidth - window.ClientBounds.Width) / half;
                        var pointY = (finalHeight - window.ClientBounds.Height) / half;
                        _rect = new Rectangle(-pointX, -pointY, finalWidth, finalHeight);
                        _offset.X = pointX;
                        _offset.Y = pointY;
                        // height is in pixel scale so both values will match the larger, final scale
                        _scaleX = finalScale;
                        _scaleY = finalScale;
                    }
                    break;
                // this mode will simply stretch x and y to fit screen but does not preserve aspect ratio
                // no viewable area is lost
                case PixelMode.StretchToFit:
                    {
                        // stretch image size to nearest int
                        var finalWidth = (int)Math.Round(_nativeScreen.Width * scaleX);
                        var finalHeight = (int)Math.Round(_nativeScreen.Height * scaleY);
                        // image is being stretched to fit perfectly so no offsets are needed
                        _rect = new Rectangle(0, 0, finalWidth, finalHeight);
                        _offset.X = 0;
                        _offset.Y = 0;
                        _scaleX = scaleX;
                        _scaleY = scaleY;
                    }
                    break;
            }
        }
    }
}
