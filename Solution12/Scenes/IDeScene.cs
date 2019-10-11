using System.Data;
using Emotion.Common;
using Emotion.Graphics;

namespace Solution12.Scenes
{
    public abstract class UnScene
    {
        protected abstract void Load();

        protected abstract void Update();

        protected abstract void Draw(RenderComposer composer);

        public void Attach()
        {
            if (Engine.DebugUpdateAction != null || Engine.DebugDrawAction != null) throw new SyntaxErrorException();

            Load();

            Engine.DebugUpdateAction = Update;
            Engine.DebugDrawAction = Draw;
        }
    }
}