using System;
using System.Numerics;
using Adfectus.Common;
using Adfectus.ImGuiNet;
using Adfectus.Primitives;
using Adfectus.Scenography;
using ImGuiNET;

namespace Solution12.Scenes
{
    /// <summary>
    /// WARNING: Carcinogenic material ahead.
    /// </summary>
    public class ReVertexScene : Scene
    {
        private readonly Random _rand = new Random();
        private Vector3[,] _mapMatrix = new Vector3[50, 20];

        public override void Load()
        {
            /* do noting */
        }

        public override void Update()
        {
            /* do noting */
        }

        public override void Draw()
        {
            DrawGui();
            DrawMatrix();
        }

        public override void Unload()
        {
            /* do noting */
        }

        private void DrawMatrix()
        {
            for (var i = 0; i < _mapMatrix.GetLength(0) - 1; i++)
            {
                for (var j = 0; j < _mapMatrix.GetLength(1) - 1; j++)
                {
                    var vertices = new[] {_mapMatrix[i, j], _mapMatrix[i + 1, j], _mapMatrix[i + 1, j + 1], _mapMatrix[i, j + 1]};
                    var colors = new Color[4];
                    for (var k = 0; k < vertices.Length; k++)
                        colors[k] = new Color(20, (int) vertices[k].Z, 20);
                    Engine.Renderer.RenderVertices(vertices, colors);
                }
            }

            foreach (var vector3 in _mapMatrix)
            {
                Engine.Renderer.RenderOutline(vector3, new Vector2(10f), Color.Black, 0.1f);
            }
        }

        private void DrawGui()
        {
            ImGui.NewFrame();
            ImGui.Begin("", ImGuiWindowFlags.NoResize);
            if (ImGui.Button("reset"))
                RegenMapMatrix();

            Engine.Renderer.RenderGui();
        }


        private void RegenMapMatrix()
        {
            for (var i = 0; i < _mapMatrix.GetLength(0); i++)
            {
                for (var j = 0; j < _mapMatrix.GetLength(1); j++)
                {
                    _mapMatrix[i, j] = new Vector3(i * 10, j * 10, _rand.Next(256));
                }
            }
        }
    }
}