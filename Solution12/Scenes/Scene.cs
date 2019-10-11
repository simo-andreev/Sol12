using Emotion.Common;

namespace Solution12.Scenes
{
    public abstract class Scene : IPlugin
    {
        public void Initialize()
        {
            Load();
        }

        public void Update()
        {
            UpdateInternal();
            Draw();
        }

        protected abstract void Load();
        protected abstract void UpdateInternal();
        protected abstract void Draw();
    }
}