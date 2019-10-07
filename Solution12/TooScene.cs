using System;
using System.Numerics;
using Adfectus.Common;
using Adfectus.Input;
using Adfectus.Primitives;
using Adfectus.Scenography;

namespace Solution12
{
    public class TooScene : Scene
    {
        private readonly Random _random = new Random();

        private Vector3 _positionPlayer = new Vector3(50, 100, 0);
        private readonly Vector2 _sizePlayer = new Vector2(10, 10);
        private const int MoveStepPlayer = 5;

        private Vector3[] _positionThangs = new Vector3[1]
        {
            new Vector3(Engine.Renderer.Camera.Width / 2 - SizeThang.X, Engine.Renderer.Camera.Height / 2 - SizeThang.Y, 1)
        };

        private static readonly Vector2 SizeThang = new Vector2(7, 7);
        private const int MoveStepThang = 4;

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
                        modX -= MoveStepPlayer;
                        break;
                    case KeyCode.D:
                        modX += MoveStepPlayer;
                        break;
                    case KeyCode.W:
                        modY -= MoveStepPlayer;
                        break;
                    case KeyCode.S:
                        modY += MoveStepPlayer;
                        break;
                }
            }

            _positionPlayer.X = Math.Clamp(_positionPlayer.X + modX, 0, Engine.Renderer.Camera.Width - _sizePlayer.X);
            _positionPlayer.Y = Math.Clamp(_positionPlayer.Y + modY, 0, Engine.Renderer.Camera.Height - _sizePlayer.Y);

//            _positionThangs[0].X += _random.Next(-1, 2) * MoveStepThang;
//            _positionThangs[0].Y += _random.Next(-1, 2) * MoveStepThang;

            for (int i = 0; i < _positionThangs.Length; i++)
            {
                switch (i % 6)
                {
                    case 0:
                        _positionThangs[i] = Vector3.Lerp(_positionPlayer, _positionThangs[i], 0.999f);
                        break;
                }

//                _positionThangs[i] = Vector3.TransformNormal()
            }

//            foreach (var t in _positionThangs)
//            {
//                var thang = t;
//                thang.X = Vector3.Lerp(thang._positionPlayer)
//                thang.X += _random.Next(-1, 2) * MoveStepThang;
//                thang.Y += _random.Next(-1, 2) * MoveStepThang;
//            }
        }

        public override void Draw()
        {
//            Console.WriteLine("SoScene - Draw");

            Engine.Renderer.RenderOutline(Engine.Renderer.Camera.Position, Engine.Renderer.Camera.Size, Color.CornflowerBlue);
            Engine.Renderer.Render(_positionPlayer, _sizePlayer, Color.Magenta);

            foreach (var thang in _positionThangs)
                Engine.Renderer.Render(thang, SizeThang, Color.Pink);
        }

        public override void Unload()
        {
//            Console.WriteLine("SoScene - Unload");
        }
    }
}