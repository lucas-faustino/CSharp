using Microsoft.Xna.Framework.Graphics;

namespace NamelessEngine
{
    /// <summary>
    /// override methods that need to be implemented. You don't have to worry about the base.Method()
    /// </summary>
    public abstract class SceneBase
    {
        public virtual void Initialize(EngineContext context) { }
        public virtual void Cleanup(EngineContext context) { }
        public virtual void Update(float dt, EngineContext context) { }
        public virtual void DrawGame(SpriteBatch sb, EngineContext context) { }
        public virtual void DrawGameUI(SpriteBatch sb, EngineContext context) { }
    }

    public sealed class EmptyScene : SceneBase { }

    public interface ISceneChanger
    {
        void ChangeScene<T>(EngineContext context) where T : SceneBase;
    }

    public sealed class SceneManager : ISceneChanger
    {
        readonly SceneBase[] _scenes;
        // empty scene that we change out of later
        SceneBase _currentScene = new EmptyScene();

        internal SceneManager(SceneBase[] scenes)
        {
            _scenes = scenes;
        }

        internal void Update(float dt, EngineContext context)
        {
            _currentScene.Update(dt, context);
        }

        internal void DrawGame(SpriteBatch sb, EngineContext context)
        {
            _currentScene.DrawGame(sb, context);
        }

        internal void DrawGameUI(SpriteBatch sb, EngineContext context)
        {
            _currentScene.DrawGameUI(sb, context);
        }

        public void ChangeScene<T>(EngineContext context) where T : SceneBase
        {
            foreach(var s in _scenes)
            {
                if(s.GetType() == typeof(T))
                {
                    _currentScene.Cleanup(context);
                    _currentScene = s;
                    _currentScene.Initialize(context);
                    break;
                }
            }
        }
    }
}
