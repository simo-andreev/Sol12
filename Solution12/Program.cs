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

            Engine.SceneManager.SetScene(new SceneScene());

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

        Vector3 cameraMoveDirection = Vector3.Zero;

        // note any-and-all 'WASD' move input
        if (Engine.InputManager.IsKeyHeld(Key.W)) cameraMoveDirection.Y -= 1;
        if (Engine.InputManager.IsKeyHeld(Key.A)) cameraMoveDirection.X -= 1;
        if (Engine.InputManager.IsKeyHeld(Key.S)) cameraMoveDirection.Y += 1;
        if (Engine.InputManager.IsKeyHeld(Key.D)) cameraMoveDirection.X += 1;

        // If mouse scroll-ed, note Zoom amount and direction
        if (Engine.InputManager.GetMouseScrollRelative() == -1) Zoom += 0.035f * Engine.DeltaTime;
        if (Engine.InputManager.GetMouseScrollRelative() == 1) Zoom -= 0.035f * Engine.DeltaTime;

        // Clamp Camera Zoom
        if (Zoom > 6) Zoom = 6;
        if (Zoom < 0.5) Zoom = 0.5f;

        var speed = 0.35f;
        // If fast-move key down -> quadruple speed coefficient 
        if (Engine.InputManager.IsKeyHeld(Key.LeftControl)) speed *= 4;

        // Vect-multiply the recorded camera movement input by DeltaT'd speed coefficient in all axes.
        cameraMoveDirection *= new Vector3(speed * Engine.DeltaTime, speed * Engine.DeltaTime, speed * Engine.DeltaTime);

        // Finally apply the movement Vector to the camera and RecreateMatrix
        Engine.Renderer.Camera.Position += cameraMoveDirection;
        RecreateMatrix();
    }
}