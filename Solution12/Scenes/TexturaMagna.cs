#region Using

using System;
using System.Collections.Generic;
using System.Numerics;
using Emotion.Common;
using Emotion.Game.QuadTree;
using Emotion.Graphics;
using Emotion.IO;
using Emotion.Plugins.ImGuiNet;
using Emotion.Plugins.ImGuiNet.Windowing;
using Emotion.Primitives;
using Emotion.Scenography;
using Emotion.Tools.Windows;
using Emotion.Utility;
using ImGuiNET;
using Solution12.Graphics.Data;

#endregion

namespace Solution12.Scenes
{
    public unsafe class TexturaMagna : IScene
    {
        private const string RESOURCE_NAME_FONT_UBUNTU = "font/UbuntuMono-R.ttf";
        private const string RESOURCE_NAME_TEXTURE_TILE_OUTLINED_CARDINAL = "iMage/tile_outlined_cardinal.png"; // Don't like long field names? Don't care.
        private const string RESOURCE_NAME_TEXTURE_TILE_OUTLINED = "iMage/tile_outlined.png";
        private const string RESOURCE_NAME_TEXTURE_TILE = "iMage/tile.png";
        private const string RESOURCE_NAME_SHADER = "Shaders/MeshShader.xml";

        private readonly Random _random = new Random();

        private DrawableFontAtlas _ubuntuFontAsset;
        private TextureAsset _tileTexture;
        private ShaderAsset _shader;

        private const int MAP_WIDTH = 100; // number of desired vertices on the "width" aka the pre-rotation 'X' axis.
        private const int MAP_HEIGHT = 100; // number of desired vertices on the "height" aka the pre-rotation 'Y' axis.
        private static readonly Vector3 TileSize = new Vector3(32, 32, 16); // The size of individual tiles.

        // The height of the terrain.
        private readonly float[,] _heightMap = new float[MAP_WIDTH, MAP_HEIGHT];

        private static readonly Vector3 RotationAngle = new Vector3(30, 50, 0);

        private readonly Matrix4x4 _rotationMatrix =
            Matrix4x4.CreateFromYawPitchRoll(Maths.DegreesToRadians(RotationAngle.Z), Maths.DegreesToRadians(RotationAngle.Y), Maths.DegreesToRadians(RotationAngle.X));

        private int _rendered = 0;

        private WindowManager _menu = new WindowManager();

        private Quadrilateral[,] tiles;
        private QuadTree<Quadrilateral> quadTree;

        private List<Quadrilateral> _drawMemory = new List<Quadrilateral>(64);

        public void Load()
        {
            // Load assets
            _ubuntuFontAsset = Engine.AssetLoader.Get<FontAsset>(RESOURCE_NAME_FONT_UBUNTU).GetAtlas(12);
            _tileTexture = Engine.AssetLoader.Get<TextureAsset>(RESOURCE_NAME_TEXTURE_TILE_OUTLINED_CARDINAL);
            _shader = Engine.AssetLoader.Get<ShaderAsset>(RESOURCE_NAME_SHADER);

            // tile count is vertexCount-1, because end vertices are included in only 1 tile (per axis).
            tiles = new Quadrilateral[MAP_WIDTH - 1, MAP_HEIGHT - 1];

            // Generate a random-ish height map.
            for (var x = 0; x < MAP_WIDTH; x++)
            for (var y = 0; y < MAP_HEIGHT; y++)
                // Assign in height map @ appropriate position, storing for later use
                _heightMap[x, y] = _random.Next((int) TileSize.Z);


            for (var y = 0; y < MAP_HEIGHT - 1; y++)
            {
                for (var x = 0; x < MAP_WIDTH - 1; x++)
                {
                    // Assign in height map @ appropriate position, storing for later use
                    var quad = new Quadrilateral(
                        Vector3.Transform(new Vector3(x * TileSize.X, y * TileSize.Y, _heightMap[x, y]), _rotationMatrix),
                        Vector3.Transform(new Vector3((x + 1) * TileSize.X, y * TileSize.Y, _heightMap[x + 1, y]), _rotationMatrix),
                        Vector3.Transform(new Vector3((x + 1) * TileSize.X, (y + 1) * TileSize.Y, _heightMap[x + 1, y + 1]), _rotationMatrix),
                        Vector3.Transform(new Vector3(x * TileSize.X, (y + 1) * TileSize.Y, _heightMap[x, y + 1]), _rotationMatrix)
                    );
                    tiles[x, y] = quad;
                }
            }

            Rectangle boundsOfMap = Rectangle.BoundsFromPolygonPoints(new[]
            {
                tiles[0, 0].Vertex0.ToVec2(),
                tiles[MAP_WIDTH - 2, 0].Vertex1.ToVec2(),
                tiles[MAP_WIDTH - 2, MAP_HEIGHT - 2].Vertex2.ToVec2(),
                tiles[0, MAP_HEIGHT - 2].Vertex3.ToVec2(),
            });

            quadTree = new QuadTree<Quadrilateral>(boundsOfMap, 100);
            foreach (var tile in tiles)
            {
                quadTree.Add(tile);
            }
        }

        public void Update()
        {
            _menu.Update();
        }


        public void Draw(RenderComposer composer)
        {
            _rendered = 0;

            _drawMemory.Clear();
            var rect = new Rectangle(
                Engine.Renderer.Camera.ScreenToWorld(Vector2.Zero),
                Engine.Renderer.Camera.ScreenToWorld(Engine.Renderer.DrawBuffer.Size) * 2
                // Engine.Configuration.RenderSize * (Engine.Renderer.Scale - (Engine.Renderer.IntScale - 1)) / Engine.Renderer.Camera.Zoom
                );
            quadTree.GetObjects(rect ,ref _drawMemory);
            composer.RenderOutline(new Vector3(rect.Position, 0f), rect.Size, Color.CornflowerBlue, 2);
            _rendered = _drawMemory.Count;
            
            Engine.Log.Info("\t" + "Mouse position \t" + Engine.Host.MousePosition, "TAAAG");
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _drawMemory.Count; i++)
            {
                var tile = _drawMemory[i];

                var c = Color.White.ToUint();

                var a = composer.GetBatch();
                var data = a.GetData(_tileTexture.Texture);
                data[0].Vertex = tile.Vertex0;
                data[0].Color = c;
                data[0].UV = Vector2.Zero;

                data[1].Vertex = tile.Vertex1;
                data[1].Color = c;
                data[1].UV = new Vector2(1, 0);

                data[2].Vertex = tile.Vertex2;
                data[2].Color = c;
                data[2].UV = new Vector2(1, 1);

                data[3].Vertex = tile.Vertex3;
                data[3].Color = c;
                data[3].UV = new Vector2(0, 1);
            }

            composer.SetDepthTest(false);
            composer.SetUseViewMatrix(false);
            composer.RenderLine(new Vector2(20, 20), new Vector2(20, 30), Color.Green, 2);
            composer.RenderLine(new Vector2(20, 20), new Vector2(30, 20), Color.Red, 2);
            composer.RenderLine(new Vector3(20, 20, 0), new Vector3(20, 20, 30), Color.Blue, 10);

            RenderGui(composer);
        }

        private void RenderGui(RenderComposer composer)
        {
            ImGui.NewFrame();
            ImGui.Begin("TexturaMagna!", ImGuiWindowFlags.AlwaysAutoResize);
            ImGui.InputInt("Rendered", ref _rendered);
            ImGui.Text("Camera " + Engine.Renderer.Camera.Position);
            ImGui.Text("Scroll to zoom, hold control (left) to fast!");
            ImGui.End();

            composer.RenderToolsMenu(_menu);
            _menu.Render(composer);

            composer.RenderUI();
        }

        public void Unload()
        {
            Engine.AssetLoader.Destroy(RESOURCE_NAME_FONT_UBUNTU);
            Engine.AssetLoader.Destroy(RESOURCE_NAME_TEXTURE_TILE);
            Engine.AssetLoader.Destroy(RESOURCE_NAME_TEXTURE_TILE_OUTLINED);
            Engine.AssetLoader.Destroy(RESOURCE_NAME_TEXTURE_TILE_OUTLINED_CARDINAL);
        }
    }
}