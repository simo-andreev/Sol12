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

        private Vector3[,] mapMatrix = new Vector3[50, 20];

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

//            DrawVertices();
            DrawMatrix();
            
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

        private void DrawMatrix()
        {
            for (var i = 0; i < mapMatrix.GetLength(0) - 1; i++)
            {
                for (var j = 0; j < mapMatrix.GetLength(1) - 1; j++)
                {
                    var vertices = new[] {mapMatrix[i, j], mapMatrix[i + 1, j], mapMatrix[i + 1, j + 1], mapMatrix[i, j + 1]};
                    var colors = new Color[4];
                    for (var k = 0; k < vertices.Length; k++)
                        colors[k] = new Color(20, (int) vertices[k].Z, 20);
                    Engine.Renderer.RenderVertices(vertices, colors);
                }
            }

            foreach (var vector3 in mapMatrix)
            {
                Engine.Renderer.RenderOutline(vector3, new Vector2(10f), Color.Black, 0.1f);
            }
        }

        private void DrawGui()
        {
            ImGui.NewFrame();
            ImGui.Begin("", ImGuiWindowFlags.NoResize);
            if (ImGui.Button("reset"))
            {
//                RegenVertices();
                RegenMapMatrix();
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

        private void RegenMapMatrix()
        {
            for (var i = 0; i < mapMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < mapMatrix.GetLength(1); j++)
                {
                    mapMatrix[i, j] = new Vector3(i * 10, j * 10, _rand.Next(256));
                }
            }
        }
    }
}