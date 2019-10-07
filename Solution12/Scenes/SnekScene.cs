using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using Adfectus.Common;
using Adfectus.Graphics.Text;
using Adfectus.Primitives;
using Adfectus.Scenography;

namespace Solution12.Scenes
{
    public class SnekScene : Scene
    {
        // Loaded assets
        private Atlas _ubuntuMono;

        // Scene-status-related shit
        private bool _gameOver = false;
        private readonly Stopwatch _updateTimer = new Stopwatch();

        // Snek bits and parts
        private readonly List<Vector3> _cells = new List<Vector3>();
        private readonly Vector2 _cellSize = new Vector2(10f);
        
        
        // Game-y stuffs
        private const byte StartingCellCount = 5;

        public SnekScene()
        {
            for (byte i = 0; i < StartingCellCount; i++)
            {
                _cells.Add(new Vector3(10, _cellSize.Y * i, 0));
            }
        }

        public override void Load()
        {
            _ubuntuMono = Engine.AssetLoader.Get<Font>("font/ubuntumono-r.ttf").GetFontAtlas(48);
            _updateTimer.Start();
        }

        public override void Update()
        {
            if (_updateTimer.ElapsedMilliseconds >= 250)
            {
                _updateTimer.Restart();
                DoStep();
            }
        }

        public override void Draw()
        {
            Engine.Renderer.RenderOutline(Engine.Renderer.Camera.Position, Engine.Renderer.Camera.Size, Color.White);

            foreach (var cell in _cells)
            {
                Engine.Renderer.Render(cell, _cellSize, Color.Green);
            }

            if (_gameOver)
            {
                Engine.Renderer.RenderString(_ubuntuMono, "WASTED", Vector3.One, Color.Red);
            }
        }

        private void DoStep()
        {
            for (int i = 0; i < _cells.Count; i++)
            {
                if (i < _cells.Count - 1) // if not last cell
                {
                    _cells[i] = _cells[i + 1];
                }
                else
                {
                    var cell = _cells[i];
                    cell.X += _cellSize.X;
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