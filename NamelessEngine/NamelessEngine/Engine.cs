using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NamelessEngine
{
    public sealed class Engine : Game
    {
        readonly ResolutionManager _resolution;
        readonly GraphicsDeviceManager _graphics;
        readonly SceneManager _sceneManager;
        readonly EngineContext _context;
        readonly EngineOnLoadParameters _onLoad;
        readonly IDebug _debug;

        SpriteBatch _spriteBatch;

        public Engine(string gameTitle, EngineOnConstructParameters parameters, EngineOnLoadParameters onLoad, IDebug debug)
        {
            // keeping this in XNA order to prevent any potential issues
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            _graphics.DeviceReset += OnDeviceReset;
            InactiveSleepTime = TimeSpan.Zero;
            _resolution = new ResolutionManager(parameters.NativeScreen);
            _sceneManager = new SceneManager(parameters.Scenes);
            _context = new EngineContext(
                new Camera(parameters.NativeScreen, parameters.TileSize), 
                new Sound.BGMManager(parameters.BGMs),
                new Sound.SoundClipManager(parameters.Clips),
                new Input.Controller(parameters.MaxControllers),
                new Input.KBM(),
                new UI.SpriteManager(),
                new UI.TextManager(),
                new TextureManager(),
                _sceneManager, 
                parameters.ModuleManager,
                parameters.NativeScreen,
                parameters.TileSize,
                Exit, 
                setPixelMode: x => {
                    _resolution.PixelMode = x;
                    _resolution.SetScale(Window);
                },
                ToggleFullScreen
            );

            _onLoad = onLoad;
            _debug = debug;
            Window.Title = gameTitle;
        }

        private void ToggleFullScreen(bool isFullScreen)
        {
            if(isFullScreen)
            {
                _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                Window.IsBorderlessEXT = true;
            }
            else
            {
                _graphics.PreferredBackBufferWidth = _context.NativeScreen.Width;
                _graphics.PreferredBackBufferHeight = _context.NativeScreen.Height;
                Window.IsBorderlessEXT = false;
            }

            Window.AllowUserResizing = false;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();
        }

        private void OnDeviceReset(object sender, EventArgs e)
        {
            _graphics.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = true;
            _graphics.PreferMultiSampling = false;
            _graphics.PreferredDepthStencilFormat = DepthFormat.None;
            _resolution.SetScale(Window);
        }

        protected override void Initialize()
        {
            base.Initialize();
            ToggleFullScreen(_onLoad.IsInitFullScreen);
            _resolution.Initialize(GraphicsDevice);
            _resolution.SetScale(Window);
            _onLoad.SetInitialScene(_context);
        }

        protected override void LoadContent()
        {
            base.LoadContent();
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _context.TextureManager.LoadContent(_onLoad.LoadTextures(GraphicsDevice));
            _debug.LoadContent(_context);
        }

        protected override void UnloadContent()
        {
            base.UnloadContent();
            _context.BGM.Unload();
            _context.SoundClip.Unload();
            _context.TextureManager.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            _context.Controller.Update();
            _context.KBM.Update();
            var dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _debug.Update(_context, dt);
            _sceneManager.Update(dt, _context);
            _context.SoundClip.PlayClips();
        }

        protected override void Draw(GameTime gameTime)
        {
            _resolution.DrawGame(_spriteBatch, GraphicsDevice, _sceneManager, _context, _debug);
        }
    }
}
