using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Adfectus.Common;
using Adfectus.ImGuiNet;
using Adfectus.Scenography;
using ImGuiNET;

namespace Solution12.Scenes
{
    public class SceneScene : Scene
    {
        private List<Type> _scenes;

        public override void Load()
        {
            // Get all : Scene classes in ~package~ namespace.
            _scenes = Assembly.GetExecutingAssembly().GetTypes()
                .Where(t => t.Namespace == "Solution12.Scenes")
                .Where(type => typeof(Scene).IsAssignableFrom(type))
                .ToList();
        }

        public override void Update()
        {
            /* do noting */
        }

        public override void Draw()
        {
            ImGui.NewFrame();

            foreach (var scene in _scenes)
            {
                if (ImGui.Button(scene.Name))
                {
                    Engine.SceneManager.SetScene((Scene) Activator.CreateInstance(scene));
                    break;
                }
            }

            Engine.Renderer.RenderGui();
        }

        public override void Unload()
        {
            /* do noting */
        }
    }
}