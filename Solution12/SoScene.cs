﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using Adfectus.Common;
using Adfectus.Graphics.Text;
using Adfectus.Input;
using Adfectus.IO;
using Adfectus.Platform.DesktopGL.Assets;
using Adfectus.Primitives;
using Adfectus.Scenography;
using Adfectus.Utility;

namespace Solution12
{
    public class SoScene : Scene
    {
        private readonly Random _random = new Random();

        private Vector3 _positionPlayer = new Vector3(50, 100, 0);
        private readonly Vector2 _sizePlayer = new Vector2(10, 10);
        private const int MoveStepPlayer = 5;

        private KeyCode _direction = KeyCode.Backspace;

        private readonly KeyCode[] _validDirections = new KeyCode[4] {KeyCode.A, KeyCode.S, KeyCode.D, KeyCode.W};

        private Atlas _atlas;

        public override void Load()
        {
            _atlas = Engine.AssetLoader.Get<Font>("font/ubuntumono-r.ttf").GetFontAtlas(48);
        }

        public override void Update()
        {
            foreach (var key in Engine.InputManager.GetAllKeysDown())
            {
                if (key == _direction || !_validDirections.Contains(key)) return;
                _direction = key;
            }

            switch (_direction)
            {
                case KeyCode.A:
                    _positionPlayer.X -= MoveStepPlayer;
                    break;
                case KeyCode.D:
                    _positionPlayer.X += MoveStepPlayer;
                    break;
                case KeyCode.W:
                    _positionPlayer.Y -= MoveStepPlayer;
                    break;
                case KeyCode.S:
                    _positionPlayer.Y += MoveStepPlayer;
                    break;
                case KeyCode.Backspace:
                    /* do noting */
                    break;
                default: throw new Exception();
            }

            _positionPlayer.X = Math.Clamp(_positionPlayer.X, Engine.Renderer.Camera.X + 1, Engine.Renderer.Camera.Width - _sizePlayer.X - 1);
            _positionPlayer.Y = Math.Clamp(_positionPlayer.Y, Engine.Renderer.Camera.Y + 1, Engine.Renderer.Camera.Height - _sizePlayer.Y - 1);
        }

        public override void Draw()
        {
            Engine.Renderer.RenderString(_atlas, "WASTED", _positionPlayer, Color.Red);


            Engine.Renderer.RenderOutline(Engine.Renderer.Camera.Position, Engine.Renderer.Camera.Size, Color.CornflowerBlue);
            Engine.Renderer.Render(_positionPlayer, _sizePlayer, Color.Magenta);
        }

        public override void Unload()
        {
            Engine.AssetLoader.Destroy("font/ubuntumono-r.ttf");
        }
    }
}