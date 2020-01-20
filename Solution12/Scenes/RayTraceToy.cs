using System;
using System.Numerics;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.Plugins.ImGuiNet;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Utility;
using ImGuiNET;
using Solution12.Util.Models;

namespace Solution12.Scenes
{
    public class RayTraceToy : IScene
    {
        private Vector2 _mouseWorldPosition;

        private Vector2 _barrierStart = new Vector2(0, -50);
        private Vector2 _barrierEnd = new Vector2(100, -50);

        private Vector2 _rayStart = new Vector2(50, 0);
        private Vector2 _rayDirection = new Vector2(0, 1);

        private Vector2? _crossPoint = new Vector2?();


        public void Load()
        {
        }

        public void Update()
        {
            _mouseWorldPosition = Engine.Renderer.Camera.ScreenToWorld(Engine.Host.MousePosition);
        }

        public void Draw(RenderComposer composer)
        {
            // Draw X and Y Axes for easier visual orientation
            composer.RenderLine(new Vector2(-800, 0), new Vector2(800, 0), Color.White, 0.5f);
            composer.RenderLine(new Vector2(0, -800), new Vector2(0, 800), Color.White, 0.5f);

            var ray = new Ray2D(_rayStart, _rayDirection, 1000f);
            var barrier = new Line(_barrierStart, _barrierEnd);

            composer.RenderLine(ray.Start, ray.End, Color.Green);

            var color = Util.Util.Intersects(ray, barrier, out _crossPoint) ? Color.Red : Color.CornflowerBlue;
            composer.RenderLine(_barrierStart, _barrierEnd, color);

            if (_crossPoint != null)
                composer.RenderSprite(new Vector3(_crossPoint.Value.X - 1, _crossPoint.Value.Y - 1, 5), new Vector2(2f), Color.Magenta);


            DrawGui(composer);
        }

        private void DrawGui(RenderComposer composer)
        {
            ImGui.NewFrame();

            ImGui.Text("         X        |        Y         ");
            ImGui.DragFloat2("Barrier Start", ref _barrierStart);
            ImGui.DragFloat2("Barrier End", ref _barrierEnd);
            ImGui.Separator();
            ImGui.DragFloat2("Ray Start", ref _rayStart);
            ImGui.DragFloat2("Ray End", ref _rayDirection);
            ImGui.Text("         X        |        Y         ");
            ImGui.Text($"CrossPoint = {_crossPoint.ToString()}");

            var distance = _crossPoint.HasValue ? (_rayStart - _crossPoint.Value).Length() : (float?) null;
            ImGui.Text($"Distance = {distance}");

            composer.RenderUI();
        }

        public void Unload()
        {
        }
    }
}