using System;
using System.Collections.Generic;
using System.Numerics;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.Graphics.Command;
using Emotion.Graphics.Data;
using Emotion.Graphics.Objects;
using Emotion.IO;
using Emotion.Primitives;

namespace Solution12.Scenes
{
    public class TexturaMagna : UnScene
    {
        private Random _random = new Random();

        private const int MapX = 5;
        private const int MapY = 5;
        private const float TileWidth = 64;
        private static readonly Vector2 TileSize = new Vector2(TileWidth);

        private DrawableFontAtlas _ubuntuFontAsset;
        private TextureAsset _tileTexture;

        private List<RecyclableCommand> _renderCommands;

        private Vector3[,] _map;

        protected override void Load()
        {
            _ubuntuFontAsset = Engine.AssetLoader.Get<FontAsset>("font/UbuntuMono-R.ttf").GetAtlas(12);
            _tileTexture = Engine.AssetLoader.Get<TextureAsset>("iMage/tile_outlined.png");

            // TODO - Simo Andreev - 11.10.2019 - populate here? elsewhere? nowhere? idk
            _renderCommands = new List<RecyclableCommand>();

            _map = new Vector3[5, 5];
            for (int x = 0, y = 0, i = 0; i < _map.Length; i++, x = i / MapX, y = i % MapY)
                _map[x, y] = new Vector3(x * TileWidth, y * TileWidth, x%2 * 10);
        }

        protected override void Update()
        {
        }

        protected override void Draw(RenderComposer composer)
        {
            foreach (var position in _map)
                composer.RenderSprite(position, TileSize, Color.White, _tileTexture.Texture);
        }
    }
}