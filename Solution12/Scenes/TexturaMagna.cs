#region Using

using System;
using System.Diagnostics;
using System.Numerics;
using Emotion.Common;
using Emotion.Graphics;
using Emotion.Graphics.Command;
using Emotion.Graphics.Command.Batches;
using Emotion.Graphics.Data;
using Emotion.IO;
using Emotion.Plugins.ImGuiNet;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Utility;
using ImGuiNET;

#endregion

namespace Solution12.Scenes
{
    public class TexturaMagna : IScene
    {
        private const string RESOURCE_NAME_FONT_UBUNTU = "font/UbuntuMono-R.ttf";
        private const string RESOURCE_NAME_TEXTURE_TILE_OUTLINED = "iMage/tile_outlined.png"; // Don't like long field names? Don't care.
        private const string RESOURCE_NAME_TEXTURE_TILE = "iMage/tile.png";
        private const string RESOURCE_NAME_SHADER = "Shaders/MeshShader.xml";

        private readonly Random _random = new Random();

        private DrawableFontAtlas _ubuntuFontAsset;
        private TextureAsset _tileTexture;
        private ShaderAsset _shader;

        private const int MAP_WIDTH = 50; // number of desired vertices on the "width" aka the pre-rotation 'X' axis.
        private const int MAP_HEIGHT = 50; // number of desired vertices on the "height" aka the pre-rotation 'Y' axis.
        private static readonly Vector3 TileSize = new Vector3(32, 32, 16); // The size of individual tiles.

        // The height of the terrain.
        private readonly float[,] _heightMap = new float[MAP_WIDTH, MAP_HEIGHT];

        // Batch which holds all RenderSpriteCommand-s to allow batch processing/publishing
        // private readonly MeshBatchCommand _mapSpriteRenderBatch = new MeshBatchCommand();

        // Current location of the mouse pointer in ScreenSpace
        private Vector2 _mouseScreenPos;

        private static readonly Vector3 RotationAngle = new Vector3(30, 50, 0);

        private readonly Matrix4x4 _rotationMatrix =
            Matrix4x4.CreateFromYawPitchRoll(Maths.DegreesToRadians(RotationAngle.Z), Maths.DegreesToRadians(RotationAngle.Y), Maths.DegreesToRadians(RotationAngle.X));

        private bool _rotation = true;
        private int _rendered = 0;


        public void Load()
        {
            // _mapSpriteRenderBatch.Sun = new Vector3(500, 500, 4000);

            // Load assets
            _ubuntuFontAsset = Engine.AssetLoader.Get<FontAsset>(RESOURCE_NAME_FONT_UBUNTU).GetAtlas(12);
            _tileTexture = Engine.AssetLoader.Get<TextureAsset>(RESOURCE_NAME_TEXTURE_TILE);
            _shader = Engine.AssetLoader.Get<ShaderAsset>(RESOURCE_NAME_SHADER);

            // Generate a random-ish height map.
            for (var x = 0; x < _heightMap.GetLength(0); x++)
            {
                for (var y = 0; y < _heightMap.GetLength(1); y++)
                {
                    // Assign in height map @ appropriate position, storing for later use
                    _heightMap[x, y] = _random.Next((int) (TileSize.Z));
                }
            }
        }

        public void Update()
        {
            _mouseScreenPos = Engine.Host.MousePosition;
        }

        private Vector2 oldPosition = Vector2.Zero;

        public void Draw(RenderComposer composer)
        {
            if (_rotation) composer.PushModelMatrix(_rotationMatrix);

            // This is basically what is visible on the screen as a bounding box in world space.
            var worldClip = new Rectangle(Engine.Renderer.Camera.ScreenToWorld(new Vector2(50, 50)), new Vector2(600, 200));

            // if (worldClip.Position != oldPosition)
            // {
            //     oldPosition = worldClip.Position;
            //     Engine.Log.Warning("\t worldClip: " + oldPosition, "fml");
            // }

            var worldClipStart = Vector2.Transform(worldClip.Position, _rotationMatrix);
            var worldClipEnd = Vector2.Transform(worldClip.Position + worldClip.Size, _rotationMatrix);

            composer.RenderOutline(new Vector3(worldClip.Position.X, worldClip.Position.Y, 10), worldClip.Size, Color.Magenta, 2);

            _rendered = 0;


            // int xStartIndex = (int) Maths.Clamp(MathF.Floor(worldClip.X / TileSize.X), 0, _heightMap.GetLength(0) - 1);
            // int xEndIndex = xStartIndex + (int) Maths.Clamp(MathF.Ceiling(worldClip.Width / TileSize.X), 0, _heightMap.GetLength(0) - 2);
            //
            // int yStartIndex = (int) Maths.Clamp(MathF.Floor(worldClip.Y / TileSize.Y), 0, _heightMap.GetLength(1) - 1);
            // int yEndIndex = yStartIndex + (int) Maths.Clamp(MathF.Ceiling(worldClip.Height / TileSize.Y), 0, _heightMap.GetLength(1) - 2);

            // safe values
            // int xStartIndex = 0;
            // int xEndIndex = _heightMap.GetLength(0) - 2;
            //
            // int yStartIndex = 0;
            // int yEndIndex = _heightMap.GetLength(1) - 2;

            int xStartIndex = (int) Math.Clamp(Math.Floor(worldClip.Position.X / TileSize.X), 0, _heightMap.GetLength(0) - 2);
            int xEndIndex = (int) Math.Clamp(Math.Floor((worldClip.Position.X + worldClip.Width) / TileSize.X) + 1, 0, _heightMap.GetLength(0) - 2);

            int yStartIndex = (int) Math.Clamp(Math.Floor(worldClip.Position.Y / TileSize.Y), 0, _heightMap.GetLength(1) - 2);
            int yEndIndex = (int) Math.Clamp(Math.Floor((worldClip.Position.Y + worldClip.Height) / TileSize.X) + 1, 0, _heightMap.GetLength(1) - 2);


            // Drawing the terrain requires a custom batch which knows how to map MeshVertexData. (in the future the engine will handle variable vertex data)

            for (var x = xStartIndex; x < xEndIndex; x++)
            for (var y = yStartIndex; y < yEndIndex; y++)
            {
                _rendered++;
                // Generate a Sprite RenderCommand for the current position vector, settings its bounds to the current and 3 'previous' vertexes
                // This will #Push dat Render boi into the batch
                var dataSpan = composer.GetBatch().GetData(_tileTexture.Texture);
                dataSpan[0].Vertex = new Vector3(x * TileSize.X, y * TileSize.Y, _heightMap[x, y]);
                dataSpan[0].Color = Color.White.ToUint();
                dataSpan[0].UV = Vector2.Zero;

                dataSpan[1].Vertex = new Vector3((x + 1) * TileSize.X, y * TileSize.Y, _heightMap[x + 1, y]);
                dataSpan[1].Color = Color.White.ToUint();
                dataSpan[1].UV = new Vector2(0, 1);

                dataSpan[2].Vertex = new Vector3((x + 1) * TileSize.X, (y + 1) * TileSize.Y, _heightMap[x + 1, y + 1]);
                dataSpan[2].Color = Color.White.ToUint();
                dataSpan[2].UV = Vector2.One;

                dataSpan[3].Vertex = new Vector3(x * TileSize.X, (y + 1) * TileSize.Y, _heightMap[x, y + 1]);
                dataSpan[3].Color = Color.White.ToUint();
                dataSpan[3].UV = new Vector2(1, 0);
            }

            composer.SetDepthTest(false);
            composer.SetUseViewMatrix(false);
            composer.RenderLine(new Vector2(20, 20), new Vector2(20, 30), Color.Green, 2);
            composer.RenderLine(new Vector2(20, 20), new Vector2(30, 20), Color.Red, 2);
            composer.RenderLine(new Vector3(20, 20, 0), new Vector3(20, 20, 30), Color.Blue, 10);

            if (_rotation) composer.PopModelMatrix();

            RenderGui(composer);
        }

        private void RenderGui(RenderComposer composer)
        {
            ImGui.NewFrame();
            ImGui.Begin("TexturaMagna!", ImGuiWindowFlags.AlwaysAutoResize);
            // ImGui.InputFloat3("Sun", ref _mapSpriteRenderBatch.Sun);
            ImGui.InputInt("Rendered", ref _rendered);
            ImGui.Checkbox("Rotate", ref _rotation);
            ImGui.Text("Camera " + Engine.Renderer.Camera.Position);
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