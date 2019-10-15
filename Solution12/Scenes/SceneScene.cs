using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.Plugins.ImGuiNet;
using Emotion.Scenography;
using ImGuiNET;

namespace Solution12.Scenes
{
    public class SceneScene : IScene
    {
        private List<Type> _scenes;
        
        public void Load()
        {
            // Get all : Scene classes in ~package~ namespace.
            _scenes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "Solution12.Scenes")
                .Where(type => typeof(IScene).IsAssignableFrom(type))
                .ToList();
        }

        public void Update()
        {
            /* do noting */
        }

        public void Draw(RenderComposer composer)
        {
            ImGui.NewFrame();

            foreach (var scene in _scenes)
            {
                if (ImGui.Button(scene.Name))
                {
                    Engine.SceneManager.SetScene((IScene) Activator.CreateInstance(scene));
                    break;
                }
            }
            
            composer.RenderUI();
        }

        public void Unload()
        {
            /* do noting */
        }
    }
}