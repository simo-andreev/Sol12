using System;
using System.Numerics;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.Plugins.ImGuiNet;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Utility;
using ImGuiNET;

namespace Solution12.Scenes
{
    public class RayTraceToy : IScene
    {
        private Vector2 _mouseWorldPosition;

        private Vector2 _barrierPosition = Vector2.Zero;
        private Vector2 _barrierSize = new Vector2(10f);

        private Vector2 _rayStart = Vector2.Zero;
        private Vector2 _rayEnd = new Vector2(200, 0);

        public void Load()
        {
        }

        public void Update()
        {
            _mouseWorldPosition = Engine.Renderer.Camera.ScreenToWorld(Engine.Host.MousePosition);
        }

        public void Draw(RenderComposer composer)
        {
            composer.RenderLine(new Vector2(-800, 0), new Vector2(800, 0), Color.White, 0.5f);
            composer.RenderLine(new Vector2(0, -800), new Vector2(0, 800), Color.White, 0.5f);


            var rectangle = new Rectangle(_barrierPosition, _barrierSize);
            var ray = new Ray2D(_rayStart, _rayEnd);

//            var color = rectangle.RayIntersects(ref rectangle, ref ray, out _) ? Color.Red : Color.CornflowerBlue;
//            composer.RenderSprite(new Vector3(_barrierPosition, 0f), _barrierSize, color);

            var crossPoint = new Vector2();
            var color = LineIntersectsLine(new Line(_rayStart, _rayEnd), new Line(_barrierPosition, _barrierSize), out crossPoint) ? Color.Red : Color.CornflowerBlue;
            composer.RenderLine(_barrierPosition, _barrierSize, color);
            composer.RenderSprite(new Vector3(crossPoint.X - 1, crossPoint.Y - 1, 5), new Vector2(2f), Color.Magenta);

            composer.RenderLine(_rayStart, _rayEnd, Color.Green);


            ImGui.NewFrame();

            ImGui.Text("         X        |        Y         ");
            ImGui.DragFloat2("Barrier Position", ref _barrierPosition);
            ImGui.DragFloat2("Barrier Size", ref _barrierSize);
            ImGui.Separator();
            ImGui.DragFloat2("Ray Start", ref _rayStart);
            ImGui.DragFloat2("Ray End", ref _rayEnd);
            ImGui.Text("         X        |        Y         ");

            composer.RenderUI();
        }

        public struct Line
        {
            public Vector2 start;
            public Vector2 end;

            public Line(Vector2 start, Vector2 end)
            {
                this.start = start;
                this.end = end;
            }
        }

        public bool LineIntersectsLine(Line line, Line barrier, out Vector2 intersection)
        {
            float intersectCoef = -1f;
            Vector2 lineSegment = line.end - line.start;
            Vector2 barrierSegment = barrier.end - barrier.start;
            intersectCoef = Vector2Cross(line.start - barrier.start, lineSegment) / Vector2Cross(barrierSegment, lineSegment);

            intersection = barrier.start + intersectCoef * barrierSegment;
            return intersectCoef >= 0 && intersectCoef <= 1;
        }

        public float Vector2Cross(Vector2 vector1, Vector2 vector2)
        {
            return vector1.X * vector2.Y - vector1.Y * vector2.X;
        }

        public bool Intersects(Rectangle rectangle, Ray2D ray, out float distance, float maxDistance = float.MaxValue)
        {
            const float nearZeroLimit = 1E-06f;

            distance = -1f;

            var directionXAbs = Math.Abs(ray.Direction.X);
            var directionYAbs = Math.Abs(ray.Direction.Y);

            // If the ray _starts_ withing the rectangle - yeah - it intersects. note*: distance remains 0f
            if (ray.Start.X >= rectangle.X && ray.Start.X <= rectangle.X + rectangle.Width)
                return true;

            // If the magnitude of the Direction vector is ≈ 0 -> Ray  intersects only if start is within rectangle, which we already checked for
            if (Math.Abs(ray.Direction.Length()) < nearZeroLimit) return false;

            // Ray is of non-zero magnitude & does not start within the rectangle

            // Ray has no X component => travels straight along Y axis.
            if (Math.Abs(ray.Direction.X) < nearZeroLimit)
            {
                // For a Y-bound ray, if it does not start within the X axis projection of teh rectangle => it will never intersect.
                if (ray.Start.X < rectangle.X || ray.Start.X > rectangle.X + rectangle.Width) return false;

                // If a Y-axis bound ray points towards negative infinity & starts 'before' the rectangle it will never intersect
                if (ray.Direction.Y < 0 && ray.Start.Y < rectangle.Y) return false;

                // If a Y-axis bound ray points towards positive infinity & starts 'after' the rectangle it will never intersect
                if (ray.Direction.Y > 0 && ray.Start.Y > rectangle.Y + rectangle.Height) return false;

                // The ray is Y-bound, non-zero, points towards the Rectangle.Y and starts within its X projection.

                float distanceToRectangleStart = 0f;
                float distanceToRectangleEnd = 0f;
                if (Math.Sign(ray.Start.Y) == Math.Sign(rectangle.Y))
                {
//                    distanceToRectangleStart = 
                }
            }

            return true;
        }

        public void Unload()
        {
        }
    }
}