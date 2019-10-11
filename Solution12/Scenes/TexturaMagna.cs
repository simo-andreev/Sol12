using System;
using System.Collections.Generic;
using System.Numerics;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.Graphics.Command;
using Emotion.Graphics.Data;
using Emotion.IO;
using Emotion.Plugins.ImGuiNet;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Utility;
using ImGuiNET;

namespace Solution12.Scenes
{
    public class TexturaMagna : IScene
    {
        private Random _random = new Random();

        private const int MapX = 100;
        private const int MapY = 100;
        private const float TileWidth = 32;
        private static readonly Vector2 TileSize = new Vector2(TileWidth);

        private DrawableFontAtlas _ubuntuFontAsset;
        private TextureAsset _tileTexture;

        private List<RecyclableCommand> _renderCommands;

        private Vector3[,] _map;

        private Vector3 _camRotationInputPosition = new Vector3();
        private readonly QuadBatch _batch = new QuadBatch();
        private Vector2 _mouseWorldPos;
        private Vector3 _mouseMapPos;
        private Matrix4x4 _rotate = Matrix4x4.Identity;
        private Vector3 _rotation = new Vector3(0, 45, 45);
        private Vector2 _mouseScreenPos;

        public void Load()
        {
            _ubuntuFontAsset = Engine.AssetLoader.Get<FontAsset>("font/UbuntuMono-R.ttf").GetAtlas(12);
            _tileTexture = Engine.AssetLoader.Get<TextureAsset>("iMage/tile_outlined.png");

            _map = new Vector3[MapX, MapY];
            for (int x = 0, y = 0, i = 0; i < _map.Length; i++, x = i / MapX, y = i % MapY)
            {
                var vector = new Vector3(x * TileWidth, y * TileWidth, _random.Next(20));
                vector = Vector3.Transform(vector, Matrix4x4.CreateFromYawPitchRoll(MathExtension.DegreesToRadians(0), MathExtension.DegreesToRadians(45), MathExtension.DegreesToRadians(45)));
                _map[x, y] = vector;
            }


            for (var x = 0; x < _map.GetLength(0) - 1; x++)
            {
                for (var y = 0; y < _map.GetLength(1) - 1; y++)
                {
                    var tileRenderCommand = new RenderSpriteCommand
                    {
                        Position = _map[x, y],
                        Color = Color.White.ToUint(),
                        Size = TileSize,
                        Texture = _tileTexture.Texture
                    };

                    tileRenderCommand.Process();
                    var vert1 = tileRenderCommand.Vertices[1].Vertex;
                    tileRenderCommand.Vertices[1].Vertex = _map[x + 1, y];
                    tileRenderCommand.Vertices[2].Vertex = _map[x + 1, y + 1];
                    tileRenderCommand.Vertices[3].Vertex = _map[x, y + 1];

                    _batch.PushSprite(tileRenderCommand);
                }
            }
        }

        public void Update()
        {
            _rotate = Matrix4x4.CreateFromYawPitchRoll(MathExtension.DegreesToRadians(_rotation.X), MathExtension.DegreesToRadians(_rotation.Y), MathExtension.DegreesToRadians(_rotation.Z));

            _mouseScreenPos = Engine.Host.MousePosition;
            _mouseWorldPos = Engine.Renderer.Camera.ScreenToWorld(_mouseScreenPos);

            _mouseMapPos = WorldToMapScape(new Vector3(_mouseWorldPos, 2));
        }

        public void Draw(RenderComposer composer)
        {
            composer.PushCommand(_batch, dontBatch: true);
        }

        public void Unload()
        {
            /* do noting */
        }

        public Vector3 PositionOfTile(Vector2 tileId)
        {
            return Vector3.Zero;
        }

        public Vector3 WorldToMapScape(Vector3 worldPos)
        {
            return Vector3.Transform(worldPos, _rotate);
        }
    }
}