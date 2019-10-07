using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using Adfectus.Common;
using Adfectus.Graphics.Text;
using Adfectus.Primitives;
using Adfectus.Scenography;
using SharpFont;

namespace Solution12
{
    public class SnekScene : Scene
    {
        // Loaded assets
        private Atlas _ubuntuMono;

        private bool _gameOver = false;
        private Stopwatch updateTimer = new Stopwatch();


        private List<Vector3> _cells = new List<Vector3> {new Vector3(10, 10, 0), new Vector3(10, 20, 0), new Vector3(10, 30, 0)};
        private Vector2 _cellSize = new Vector2(10f);

        public override void Load()
        {
            _ubuntuMono = Engine.AssetLoader.Get<Font>("font/ubuntumono-r.ttf").GetFontAtlas(48);
            updateTimer.Start();
        }

        public override void Update()
        {
            if (updateTimer.ElapsedMilliseconds >= 250)
            {
                updateTimer.Restart();
                DoStep();
            }
        }

        public override void Draw()
        {
            Engine.Renderer.RenderOutline(Engine.Renderer.Camera.Position, Engine.Renderer.Camera.Size, Color.White);

            foreach (var cell in _cells)
            {
                Engine.Renderer.Render(cell, new Vector2(10, 10), Color.Green);
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