using Microsoft.Xna.Framework.Graphics;

namespace NamelessEngine
{
    public interface IIndexer<T>
    {
        int GetIndex(T key);
    }

    public interface IDebug
    {
        void LoadContent(EngineContext context);
        void Update(EngineContext context, float dt);
        void Draw(SpriteBatch sb, EngineContext context);
        void DrawUI(SpriteBatch sb, EngineContext context);
    }

    public sealed class EmptyDebug : IDebug
    {
        public void LoadContent(EngineContext context) { }
        public void Update(EngineContext context, float dt) { }
        public void Draw(SpriteBatch sb, EngineContext context) { }
        public void DrawUI(SpriteBatch sb, EngineContext context) { }
    }
}
