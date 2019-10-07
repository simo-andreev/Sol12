using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Adfectus.Common;
using Adfectus.Graphics.Text;
using Adfectus.Input;
using Adfectus.Logging;
using Adfectus.Primitives;
using Adfectus.Scenography;

namespace Solution12.Scenes
{
    public class SnekScene : Scene
    {
        // Loaded assets
        private Atlas _atlasUbuntuMonoSmall;
        private Atlas _atlasUbuntuMonoBicc;

        // Scene-status-related shit
        private bool _gameOver = false;
        private readonly Stopwatch _updateTimer = new Stopwatch();

        // Snek bits and parts
        private readonly List<Vector3> _cells = new List<Vector3>();
        private readonly Vector2 _cellSize = new Vector2(10f);

        // Game-y stuffs
        private const byte StartingCellCount = 5;

        // Control/Input
        private KeyCode? _activeDirection = null;
        private KeyCode? _newDirection = null;
        private readonly KeyCode[] _validDirections = {KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S};

        public SnekScene()
        {
            for (byte i = 0; i < StartingCellCount; i++)
            {
                _cells.Add(new Vector3(10, _cellSize.Y * i, 0));
            }
        }

        public override void Load()
        {
            _atlasUbuntuMonoSmall = Engine.AssetLoader.Get<Font>("font/ubuntumono-r.ttf").GetFontAtlas(8);
            _atlasUbuntuMonoBicc = Engine.AssetLoader.Get<Font>("font/ubuntumono-r.ttf").GetFontAtlas(64);
            _updateTimer.Start();
        }

        public override void Update()
        {
            UpdateDirection();

            if (_updateTimer.ElapsedMilliseconds >= 250)
            {
                _updateTimer.Restart();
                DoStep();
            }
        }

        public override void Draw()
        {
            Engine.Renderer.RenderOutline(Engine.Renderer.Camera.Position, Engine.Renderer.Camera.Size, Color.White);
            Engine.Renderer.RenderString(_atlasUbuntuMonoSmall, $"active: {_activeDirection} | new: {_newDirection}", Vector3.Zero, Color.Green);
            
            foreach (var cell in _cells)
            {
                Engine.Renderer.Render(cell, _cellSize, Color.Green);
            }

            if (_gameOver)
            {
                Engine.Renderer.RenderString(_atlasUbuntuMonoSmall, "WASTED", Vector3.One, Color.Red);
            }
        }

        private void UpdateDirection()
        {
            foreach (var keyCode in Engine.InputManager.GetAllKeysDown())
            {
                if (keyCode == _activeDirection) return; // If the input is the same as the current direction -> no need to do anything
                if (!_validDirections.Contains(keyCode)) return; // Ignore inputs outside of the predefined 'validDirection' keys
                if (IsReverseOfCurrent(keyCode)) return; // U-turns in place are not allowed

                _newDirection = keyCode;
            }
        }

        private bool IsReverseOfCurrent(KeyCode keyCode)
        {
            if (_activeDirection == null || keyCode == _activeDirection) return false;

            switch (_activeDirection)
            {
                case KeyCode.A: return keyCode == KeyCode.D;
                case KeyCode.D: return keyCode == KeyCode.A;
                case KeyCode.W: return keyCode == KeyCode.S;
                case KeyCode.S: return keyCode == KeyCode.W;
            }

            return false;
        }

        private void DoStep()
        {
            _activeDirection = _newDirection;
//            Engine.Log.Info($, MessageSource.Input);
            
            for (int i = 0; i < _cells.Count; i++)
            {
                if (i < _cells.Count - 1) // if not last cell
                {
                    // move to position of next cell
                    _cells[i] = _cells[i + 1];
                }
                else // is last cell -> move to new position
                {
                    var cell = _cells[i];
                    switch (_activeDirection)
                    {
                        case KeyCode.A:
                            cell.X -= _cellSize.X;
                            break;
                        case KeyCode.D:
                            cell.X += _cellSize.X;
                            break;
                        case KeyCode.W:
                            cell.Y -= _cellSize.Y;
                            break;
                        case KeyCode.S:
                            cell.Y += _cellSize.Y;
                            break;
                    }

                    _cells[i] = cell;
                }
            }
        }

        public override void Unload()
        {
            Engine.AssetLoader.Destroy("font/ubuntumono-r.ttf");
        }
    }
}