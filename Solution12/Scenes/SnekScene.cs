using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Adfectus.Common;
using Adfectus.Graphics.Text;
using Adfectus.ImGuiNet;
using Adfectus.Input;
using Adfectus.Logging;
using Adfectus.Primitives;
using Adfectus.Scenography;
using ImGuiNET;
using SharpFont;

namespace Solution12.Scenes
{
    public class SnekScene : Scene
    {
        // Loaded assets
        private Atlas _atlasUbuntuMonoSmall;
        private Atlas _atlasUbuntuMonoBigg;

        // Scene-status-related shit
        private bool _gameOver = false;
        private readonly Stopwatch _updateTimer = new Stopwatch();
        private readonly Random _random = new Random();

        // Snek bits and parts
        private readonly List<Vector3> _cells = new List<Vector3>();
        private readonly Vector2 _cellSize = new Vector2(10f);

        // Game-y stuffs
        private const byte StartingCellCount = 5;
        private Vector3 _nibble = new Vector3(Engine.Renderer.BaseTarget.Size.X / 2, Engine.Renderer.BaseTarget.Size.X / 2, 0);

        // Control/Input
        private KeyCode? _activeDirection = null;
        private KeyCode? _newDirection = null;
        private readonly KeyCode[] _validDirections = {KeyCode.A, KeyCode.D, KeyCode.W, KeyCode.S};
        private const short StepPeriodMs = 100;

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
            _atlasUbuntuMonoBigg = Engine.AssetLoader.Get<Font>("font/ubuntumono-r.ttf").GetFontAtlas(64);
        }

        public override void Update()
        {
            if (_gameOver) return;

            UpdateDirection();
            if (_newDirection != null) _updateTimer.Start();

            if (_updateTimer.ElapsedMilliseconds >= StepPeriodMs)
            {
                _updateTimer.Restart();
                DoStep();
            }
        }

        private void DoStep()
        {
            _activeDirection = _newDirection;

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

            CheckForHostileCollision();
            CheckForNibbleCollision();
        }

        private void CheckForNibbleCollision()
        {
            if (_cells.Last() == _nibble)
            {
                _cells.Insert(0, _cells[0]);
                RegenerateNibble();
            }
        }

        private void RegenerateNibble()
        {
            // TODO - Simo Andreev - 08.10.2019 - coul produce a non-nommable niblle - e.g. if selected (maxBound - _cellSize.X) is not divisible by size for instance
//            var x = _random.Next((int) (Engine.Renderer.BaseTarget.Size.X - _cellSize.X));
//            var y = _random.Next((int) (Engine.Renderer.BaseTarget.Size.X - _cellSize.X));
            
            int xPositionCount = (int) Math.Floor(Engine.Renderer.BaseTarget.Size.X / _cellSize.X);
            int yPositionCount = (int) Math.Floor(Engine.Renderer.BaseTarget.Size.Y / _cellSize.Y);
            
            var x = _random.Next(xPositionCount) * _cellSize.X;
            var y = _random.Next(yPositionCount) * _cellSize.Y;

            _nibble = new Vector3(x, y, 0);
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

        private void CheckForHostileCollision()
        {
            var head = _cells.Last();

            var boundX = Engine.Renderer.BaseTarget.Size.X;
            var boundY = Engine.Renderer.BaseTarget.Size.Y;

            if (head.X <= 0 || head.Y <= 0 || head.X + _cellSize.X >= boundX || head.Y + _cellSize.Y >= boundY)
            {
                _gameOver = true;
                return;
            }

            // Can't bite yer arse if you're less than 5 blocks/cells long. (Would require turns of over 90° to do so and in _our_ world those don't exist')                     //also -conveniently- allows me to start with 1-to-4 'stacked' cells
            if (_cells.Count <= 4) return;

            // Check for each-but-the-last cell
            for (int i = 0; i < _cells.Count - 1; i++)
            {
                if (_cells[i] == _cells.Last())
                {
                    _gameOver = true;
                    return;
                }
            }
        }


        public override void Draw()
        {
            Engine.Renderer.RenderOutline(Engine.Renderer.Camera.Position, Engine.Renderer.Camera.Size, Color.White);
            Engine.Renderer.RenderString(_atlasUbuntuMonoSmall, $"active: {_activeDirection} | new: {_newDirection}", Vector3.Zero, Color.Green);

            Engine.Renderer.Render(_nibble, _cellSize, Color.FromNonPremultiplied(192, 82, 57, 255));

            foreach (var cell in _cells)
            {
                Engine.Renderer.Render(cell, _cellSize, Color.Green);
                Engine.Renderer.RenderOutline(cell, _cellSize, Color.Black, 1f);
            }

            if (_gameOver)
            {
                _updateTimer.Stop();
                Engine.Renderer.RenderString(_atlasUbuntuMonoSmall, "WASTED", new Vector3(20f), Color.Red);
            }
        }

        public override void Unload()
        {
            Engine.AssetLoader.Destroy("font/ubuntumono-r.ttf");
        }
    }
}