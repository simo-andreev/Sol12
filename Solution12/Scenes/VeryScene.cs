using System;
using System.Collections.Generic;
using System.Numerics;
using Adfectus.Common;
using Adfectus.Graphics.Text;
using Adfectus.ImGuiNet;
using Adfectus.Primitives;
using Adfectus.Scenography;
using ImGuiNET;

namespace Solution12.Scenes
{
    public class VeryScene : Scene
    {
        private Random _rand = new Random();

        private int _vertexCount = 1;

        private List<Vector3> _vertices = new List<Vector3>();
        private List<Color> _colors = new List<Color>();
        private Atlas _atlasUbuntuMonoSmall;

        public VeryScene()
        {
            RegenVertices();
        }


        public override void Load()
        {
            _atlasUbuntuMonoSmall = Engine.AssetLoader.Get<Font>("font/ubuntumono-r.ttf").GetFontAtlas(8);
        }

        public override void Update()
        {
            /* do noting */
        }

        public override void Draw()
        {
            DrawGui();

            DrawVertices();

            LabelVertices();
        }

        private void LabelVertices()
        {
            for (int i = 0; i < _vertices.Count; i++)
            {
                var vertex = _vertices[i];
                
                var labelPosition = new Vector3(vertex.X, vertex.Y, 300);
                Engine.Renderer.RenderString(_atlasUbuntuMonoSmall, $"{i}:{vertex.Z}", labelPosition, Color.Green);
            }
        }

        private void DrawVertices()
        {
            Engine.Renderer.RenderVertices(_vertices.ToArray(), _colors.ToArray());
        }

        private void DrawGui()
        {
            ImGui.NewFrame();
            ImGui.Begin("", ImGuiWindowFlags.NoResize);
            if (ImGui.Button("reset"))
            {
                RegenVertices();
            }

            ImGui.InputInt("Vertex Count:", ref _vertexCount);

            Engine.Renderer.RenderGui();
        }

        public override void Unload()
        {
            /* do noting */
        }


        private void RegenVertices()
        {
            _vertices = new List<Vector3>(_vertexCount);
            _colors = new List<Color>(_vertexCount);


            for (var i = 0; i < _vertexCount; i++)
            {
                var vertex = new Vector3(
                    _rand.Next((int) Engine.Renderer.BaseTarget.Size.X),
                    _rand.Next((int) Engine.Renderer.BaseTarget.Size.Y),
                    _rand.Next(256)
                );

                _vertices.Add(vertex);
                _colors.Add(new Color((int) vertex.Z, 100, 100));
            }
        }
    }
}