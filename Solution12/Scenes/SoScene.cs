//using System;
//using System.Linq;
//using System.Numerics;
//using Adfectus.Common;
//using Adfectus.Graphics.Text;
//using Adfectus.Input;
//using Adfectus.Logging;
//using Adfectus.Primitives;
//using Adfectus.Scenography;
//
//namespace Solution12.Scenes
//{
//    public class SoScene : Scene
//    {
//        private readonly Random _random = new Random();
//        private bool _gameOver = false;
//
//
//        private Vector3 _positionPlayer = new Vector3(50, 100, 0);
//        private readonly Vector2 _sizePlayer = new Vector2(10, 10);
//        private const int MoveStepPlayer = 5;
//
//        private KeyCode _direction = KeyCode.Backspace;
//
//        private readonly KeyCode[] _validDirections = new KeyCode[4] {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W};
//
//        private Atlas _atlas;
//
//
//        public override void Load()
//        {
//            _atlas = Engine.AssetLoader.Get<Font>("font/ubuntumono-r.ttf").GetFontAtlas(48);
//        }
//
//        public override void Update()
//        {
//            if (_gameOver) return;
//
//            foreach (var key in Engine.InputManager.GetAllKeysDown())
//            {
//                if (key == _direction || !_validDirections.Contains(key)) return;
//                _direction = key;
//            }
//
//            switch (_direction)
//            {
//                case KeyCode.A:
//                    _positionPlayer.X -= MoveStepPlayer;
//                    break;
//                case KeyCode.D:
//                    _positionPlayer.X += MoveStepPlayer;
//                    break;
//                case KeyCode.W:
//                    _positionPlayer.Y -= MoveStepPlayer;
//                    break;
//                case KeyCode.S:
//                    _positionPlayer.Y += MoveStepPlayer;
//                    break;
//                case KeyCode.Backspace:
//                    /* do noting */
//                    break;
//                default: throw new Exception();
//            }
//
////            _positionPlayer.X = Math.Clamp(_positionPlayer.X, Engine.Renderer.Camera.X + 1, Engine.Renderer.Camera.Width - _sizePlayer.X - 1);
////            _positionPlayer.Y = Math.Clamp(_positionPlayer.Y, Engine.Renderer.Camera.Y + 1, Engine.Renderer.Camera.Height - _sizePlayer.Y - 1);
//        }
//
//        public override void Draw()
//        {
//            Engine.Renderer.RenderOutline(Engine.Renderer.Camera.Position, Engine.Renderer.Camera.Size, Color.CornflowerBlue);
//            Engine.Renderer.Render(_positionPlayer, _sizePlayer, Color.Magenta);
//
//            if (!_gameOver)
//            {
//                var boundX = Engine.Renderer.BaseTarget.Size.X;
//                var boundY = Engine.Renderer.BaseTarget.Size.Y;
//
//                if (_positionPlayer.X <= 0 || _positionPlayer.X + _sizePlayer.X >= boundX || _positionPlayer.Y <= 0 || _positionPlayer.Y + _sizePlayer.Y >= boundY)
//                    _gameOver = true;
//
//                Engine.Log.Error($"posX {_positionPlayer.X}, boundX{boundX}", MessageSource.Game);
//            }
//
//            if (!_gameOver) return;
//
//            Engine.Renderer.RenderString(_atlas, "WASTED", _positionPlayer, Color.Red);
//        }
//
//        public override void Unload()
//        {
//            Engine.AssetLoader.Destroy("font/ubuntumono-r.ttf");
//        }
//    }
//}