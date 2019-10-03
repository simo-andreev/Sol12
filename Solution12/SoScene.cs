using System;
using System.Globalization;
using System.Numerics;
using Adfectus.Common;
using Adfectus.Input;
using Adfectus.Primitives;
using Adfectus.Scenography;

namespace Solution12
{
    public class SoScene : Scene
    {
        private Vector3 _position = new Vector3(50, 100, 0);
        private readonly Vector2 _size = new Vector2(10, 10);
        private const int MoveStep = 5;

        public override void Load()
        {
//            Console.WriteLine("SoScene - Load");
        }

        public override void Update()
        {
//            Console.WriteLine("SoScene - Update");

            var modX = 0;
            var modY = 0;
            foreach (var key in Engine.InputManager.GetAllKeysHeld())
            {
                switch (key)
                {
                    case KeyCode.A:
                        modX -= MoveStep;
                        break;
                    case KeyCode.D:
                        modX += MoveStep;
                        break;
                    case KeyCode.W:
                        modY -= MoveStep;
                        break;
                    case KeyCode.S:
                        modY += MoveStep;
                        break;
                }
            }
            
            Console.WriteLine($"FT: {Engine.FrameTime.ToString(CultureInfo.InvariantCulture)}");
            Console.WriteLine($"modX: {modX} | modY: {modY}");

            _position.X = Math.Clamp(_position.X + modX, 0, Engine.Renderer.Camera.Width - _size.X);
            _position.Y = Math.Clamp(_position.Y + modY, 0, Engine.Renderer.Camera.Height - _size.Y);
        }

        public override void Draw()
        {
//            Console.WriteLine("SoScene - Draw");
            
            Engine.Renderer.RenderOutline(Engine.Renderer.Camera.Position, Engine.Renderer.Camera.Size, Color.CornflowerBlue);
            Engine.Renderer.Render(_position, _size, Color.Magenta);
        }

        public override void Unload()
        {
//            Console.WriteLine("SoScene - Unload");
        }
    }
}