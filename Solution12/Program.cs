using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Emotion.Common;
using Emotion.Graphics.Camera;
using Emotion.Graphics.Command;
using Emotion.Graphics.Objects;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Plugins.ImGuiNet;
using Emotion.Primitives;
using Emotion.Utility;
using Solution12.Scenes;

namespace Solution12
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Engine.Setup(new Configurator()
                .AddPlugin(new ImGuiNetPlugin())
                .SetDebug(true)
                .SetHostSettings(new Vector2(1270, 720))
            );

            Engine.AssetLoader.AddSource(new FileAssetSource("assets/iMage"));
            Engine.AssetLoader.AddSource(new FileAssetSource("assets/Font"));
            Engine.AssetLoader.AddSource(new FileAssetSource("assets/SicBeats"));

            Engine.Renderer.FarZ = 600000; // TODO - Simo Andreev - 12.10.2019 - temp sht
            Engine.Renderer.NearZ = -600000; // TODO - Simo Andreev - 12.10.2019 - temp sht
            Engine.Renderer.Camera = new SolCam(new Vector3(0, 0, 0));

            Engine.SceneManager.SetScene(new TexturaMagna());

            Engine.Run();
        }
    }
}

class SolCam : CameraBase
{
    public SolCam(Vector3 position, float zoom = 1) : base(position, zoom)
    {
    }

    public override void Update()
    {
        base.Update();
        
        Vector2 dir = Vector2.Zero;
        if (Engine.InputManager.IsKeyHeld(Key.W)) dir.Y -= 1;
        if (Engine.InputManager.IsKeyHeld(Key.A)) dir.X -= 1;
        if (Engine.InputManager.IsKeyHeld(Key.S)) dir.Y += 1;
        if (Engine.InputManager.IsKeyHeld(Key.D)) dir.X += 1;

        dir *= new Vector2(0.35f * Engine.DeltaTime, 0.35f * Engine.DeltaTime);
        Engine.Renderer.Camera.Position += new Vector3(dir, 0);
    }
}