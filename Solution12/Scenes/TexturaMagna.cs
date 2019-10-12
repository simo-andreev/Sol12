using System;
using System.Numerics;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.Graphics.Command;
using Emotion.IO;
using Emotion.Scenography;
using Emotion.Utility;
using Solution12.Scenes.Graphics.Command;

namespace Solution12.Scenes
{
    public class TexturaMagna : IScene
    {
        private const string ResourceNameFontUbuntu = "font/UbuntuMono-R.ttf";
        private const string ResourceNameTextureTileOutlinedUbuntu = "iMage/tile_outlined.png"; // Don't like long field names? Don't care.

        private readonly Random _random = new Random();

        private DrawableFontAtlas _ubuntuFontAsset;
        private TextureAsset _tileTexture;

        private const int MapTileCountX = 100; // number of desired tiles on the "width" aka the pre-rotation 'X' axis.
        private const int MapTileCountY = 100; // number of desired tiles on the "height" aka the pre-rotation 'Y' axis.
        private static readonly Vector2 TileSize = new Vector2(32f); // the individual tile size Vect

        // Init the height map.
        // the size of the 2d Array is tileCount+1, because tiles are drawn in between vertices/vector3s. e.g. 0-1; 1-2; 2-3 would be 3 tiles, but needs 4 vertices
        private readonly Vector3[,] _heightMap = new Vector3[MapTileCountX + 1, MapTileCountY + 1];

        // Batch which holds all RenderSpriteCommand-s to allow batch processing/publishing
        private readonly QuadBatch _mapSpriteRenderBatch = new QuadBatch();

        // The height map (and from there, effectively the tile sprites) are rotated to get an isometric-ish perspective
        private readonly Matrix4x4 _tileRotation = Matrix4x4.CreateFromYawPitchRoll(MathExtension.DegreesToRadians(0), MathExtension.DegreesToRadians(45), MathExtension.DegreesToRadians(45));

        // Current location of the mouse pointer in ScreenSpace
        private Vector2 _mouseScreenPos;

        public void Load()
        {
            // Load assets
            _ubuntuFontAsset = Engine.AssetLoader.Get<FontAsset>(ResourceNameFontUbuntu).GetAtlas(12);
            _tileTexture = Engine.AssetLoader.Get<TextureAsset>(ResourceNameTextureTileOutlinedUbuntu);

            // Do an unnecessarily confusing loop, 'cause Rider didn't auto-format sth the way _I_ like. Yup.                                                                                           Any complaints can be sent via passive-aggressive pull requests on github. Tnx.
            for (int x = 0, y = 0, i = 0; i < _heightMap.Length; i++, x = i / _heightMap.GetLength(0), y = i % _heightMap.GetLength(1))
            {
                // Generate a position vector, offset appropriately as to current position in height map.
                var vector = new Vector3(x * TileSize.X, y * TileSize.Y, _random.Next(20));

                // Rotate the position using the isometric rotation
                vector = Vector3.Transform(vector, _tileRotation);

                // Assign in height map @ appropriate position, storing for later use
                _heightMap[x, y] = vector;

                // Skip first column and first row, as 'neighbours' necessary for generating a quadrilateral, instead create TileRenderCommand, once last vertex of each quad is generated
                if (x == 0 || y == 0) continue;

                // Generate a Sprite RenderCommand for the current position vector, settings its bounds to the current and 3 'previous' vertexes
                var tileRenderCommand = new RenderTileCommand(
                    _heightMap[x - 1, y - 1],
                    _heightMap[x, y - 1],
                    _heightMap[x, y],
                    _heightMap[x - 1, y],
                    _tileTexture.Texture
                );

                // #Push dat Render boi into the batch
                _mapSpriteRenderBatch.PushSprite(tileRenderCommand);
            }
        }

        public void Update()
        {
            _mouseScreenPos = Engine.Host.MousePosition;
        }

        public void Draw(RenderComposer composer)
        {
            // Draw dem map tiles. Draw 'em good.
            composer.PushCommand(_mapSpriteRenderBatch, dontBatch: true);
        }

        public void Unload()
        {
            Engine.AssetLoader.Destroy(ResourceNameFontUbuntu);
            Engine.AssetLoader.Destroy(ResourceNameTextureTileOutlinedUbuntu);
        }
    }
}