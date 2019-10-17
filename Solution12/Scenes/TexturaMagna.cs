using System;
using System.Numerics;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Plugins.ImGuiNet;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Utility;
using ImGuiNET;
using Solution12.Graphics.Command;

namespace Solution12.Scenes
{
    public class TexturaMagna : IScene
    {
        private const string RESOURCE_NAME_FONT_UBUNTU = "font/UbuntuMono-R.ttf";
        private const string RESOURCE_NAME_TEXTURE_TILE_OUTLINED = "iMage/tile_outlined.png"; // Don't like long field names? Don't care.
        private const string RESOURCE_NAME_TEXTURE_TILE = "iMage/tile.png"; // Don't like long field names? Don't care.
        private const string RESOURCE_NAME_SHADER = "Shaders/MeshShader.xml";

        private readonly Random _random = new Random();

        private DrawableFontAtlas _ubuntuFontAsset;
        private TextureAsset _tileTexture;
        private ShaderAsset _shader;

        private const int MAP_TILE_COUNT_X = 100; // number of desired tiles on the "width" aka the pre-rotation 'X' axis.
        private const int MAP_TILE_COUNT_Y = 100; // number of desired tiles on the "height" aka the pre-rotation 'Y' axis.
        private static readonly Vector2 TileSize = new Vector2(32f); // the individual tile size Vect

        // Init the height map.
        // the size of the 2d Array is tileCount+1, because tiles are drawn in between vertices/vector3s. e.g. 0-1; 1-2; 2-3 would be 3 tiles, but needs 4 vertices
        private readonly Vector3[,] _heightMap = new Vector3[MAP_TILE_COUNT_X + 1, MAP_TILE_COUNT_Y + 1];

        // Batch which holds all RenderSpriteCommand-s to allow batch processing/publishing
        private readonly MeshBatchCommand _mapSpriteRenderBatch = new MeshBatchCommand();

        // The height map (and from there, effectively the tile sprites) are rotated to get an isometric-ish perspective
        private readonly Matrix4x4 _tileRotation = Matrix4x4.CreateFromYawPitchRoll(MathExtension.DegreesToRadians(0), MathExtension.DegreesToRadians(45), MathExtension.DegreesToRadians(45));

        // Current location of the mouse pointer in ScreenSpace
        private Vector2 _mouseScreenPos;

        public void Load()
        {
            _mapSpriteRenderBatch.Sun = new Vector3(500, 500, 4000);

            // Load assets
            _ubuntuFontAsset = Engine.AssetLoader.Get<FontAsset>(RESOURCE_NAME_FONT_UBUNTU).GetAtlas(12);
            _tileTexture = Engine.AssetLoader.Get<TextureAsset>(RESOURCE_NAME_TEXTURE_TILE);
            _shader = Engine.AssetLoader.Get<ShaderAsset>(RESOURCE_NAME_SHADER);

            for (var x = 0; x < _heightMap.GetLength(0); x++)
            for (var y = 0; y < _heightMap.GetLength(1); y++)
            {
                // Generate a position vector, offset appropriately as to current position in height map.
                var vector = new Vector3(x * TileSize.X, y * TileSize.Y, _random.Next((int) (TileSize.Y / 2)));

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
            composer.SetShader(_shader.Shader, () => { _shader.Shader.SetUniformVector3("Sun", _mapSpriteRenderBatch.Sun); });

            // Draw dem map tiles. Draw 'em good.
            composer.PushCommand(_mapSpriteRenderBatch);

            composer.RenderSprite(_mapSpriteRenderBatch.Sun, new Vector2(20, 20), Color.Red);

            RenderGui(composer);
        }

        private void RenderGui(RenderComposer composer)
        {
            ImGui.NewFrame();
            ImGui.Begin("TexturaMagna!", ImGuiWindowFlags.AlwaysAutoResize);
            ImGui.InputFloat3("Sun", ref _mapSpriteRenderBatch.Sun);
            ImGui.Text("Scroll to zoom, hold control (left) to fast!");
            ImGui.End();
            composer.RenderUI();
        }

        public void Unload()
        {
            Engine.AssetLoader.Destroy(RESOURCE_NAME_FONT_UBUNTU);
            Engine.AssetLoader.Destroy(RESOURCE_NAME_TEXTURE_TILE);
            Engine.AssetLoader.Destroy(RESOURCE_NAME_TEXTURE_TILE_OUTLINED);
        }
    }
}