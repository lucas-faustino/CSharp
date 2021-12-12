using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using NamelessEngine.Sound;
using NamelessEngine.Input;
using NamelessEngine.UI;
using Microsoft.Xna.Framework.Graphics;

namespace NamelessEngine
{
    public sealed class EngineOnConstructParameters
    {
        public readonly SceneBase[] Scenes;
        public readonly SoundEffect[] BGMs;
        public readonly SoundEffect[] Clips;
        public readonly ModuleManager ModuleManager;
        public readonly NativeScreen NativeScreen;
        public readonly Point TileSize;
        public readonly int MaxControllers;

        public EngineOnConstructParameters(
            SceneBase[] scenes, 
            SoundEffect[] bgms, 
            SoundEffect[] clips, 
            ModuleManager moduleManager,
            NativeScreen nativeScreen, 
            Point tileSize, 
            int maxControllers)
        {
            Scenes = scenes;
            BGMs = bgms;
            Clips = clips;
            ModuleManager = moduleManager;
            NativeScreen = nativeScreen;
            TileSize = tileSize;
            MaxControllers = maxControllers;
        }
    }

    public sealed class EngineOnLoadParameters
    {
        public readonly Func<GraphicsDevice, Texture2D[]> LoadTextures;
        public readonly Action<EngineContext> SetInitialScene;
        public readonly bool IsInitFullScreen;

        public EngineOnLoadParameters(Func<GraphicsDevice, Texture2D[]> loadTextures, Action<EngineContext> setInitialScene, bool isInitFullScreen)
        {
            LoadTextures = loadTextures;
            SetInitialScene = setInitialScene;
            IsInitFullScreen = isInitFullScreen;
        }
    }

    public sealed class EngineContext
    {
        public readonly Camera Camera;
        public readonly BGMManager BGM;
        public readonly SoundClipManager SoundClip;
        public readonly Controller Controller;
        public readonly KBM KBM;
        public readonly SpriteManager UISprites;
        public readonly TextManager UITexts;
        public readonly TextureManager TextureManager;
        public readonly ISceneChanger SceneChanger;
        public readonly IModuleManager ModuleManager;
        public readonly Action ExitGame;
        public readonly Action<PixelMode> SetPixelMode;
        public readonly Action<bool> ToggleFullscreenMode;
        public readonly NativeScreen NativeScreen;
        public readonly Point TileSize;

        public EngineContext(
            Camera camera,
            BGMManager bgm,
            SoundClipManager soundClip,
            Controller controller,
            KBM kbm,
            SpriteManager sprites,
            TextManager texts,
            TextureManager textureManager,
            ISceneChanger sceneManager, 
            IModuleManager moduleManager,
            NativeScreen nativeScreen,
            Point tileSize,
            Action exitGame, 
            Action<PixelMode> setPixelMode,
            Action<bool> toggleFullscreenMode)
        {
            Camera = camera;
            BGM = bgm;
            SoundClip = soundClip;
            Controller = controller;
            KBM = kbm;
            UISprites = sprites;
            UITexts = texts;
            SceneChanger = sceneManager;
            ModuleManager = moduleManager;
            NativeScreen = nativeScreen;
            TileSize = tileSize;
            TextureManager = textureManager;
            ExitGame = exitGame;
            SetPixelMode = setPixelMode;
            ToggleFullscreenMode = toggleFullscreenMode;
        }
    }
}
