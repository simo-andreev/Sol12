#region Using

using System;
using System.Numerics;
using Emotion.Common;
using Emotion.Graphics.Camera;
using Emotion.IO;
using Emotion.Platform.Input;
using Emotion.Plugins.ImGuiNet;
using Solution12.Scenes;

#endregion

namespace Solution12
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Engine.Setup(new Configurator()
                .AddPlugin(new ImGuiNetPlugin())
                .SetDebug(true)
                .SetHostSettings(new Vector2(1270, 720))
            );

            Engine.Renderer.FarZ = 600000; // TODO - Simo Andreev - 12.10.2019 - temp sht
            Engine.Renderer.NearZ = -600000; // TODO - Simo Andreev - 12.10.2019 - temp sht
            // Engine.Renderer.Camera = new SolCam(new Vector3(0, 0, 0));

            Engine.SceneManager.SetScene(new SceneScene());

            Engine.Run();
        }
    }
}

internal class SolCam : CameraBase
{
    public float Speed { get; set; } = 0.35f;

    public SolCam(Vector3 position, float zoom = 1) : base(position, zoom)
    {
    }

    /// <inheritdoc />
    public override void RecreateMatrix()
    {
        Vector2 targetSize = Engine.Configuration.RenderSize;
        Vector2 currentSize = Engine.Renderer.DrawBuffer.Size;

        // Transform the position from the center position to the offset position.
        Vector3 posOffset = new Vector3(X, Y, 0) - new Vector3(targetSize, 0) / 2;

        // Get the scale relative to the zoom.
        float scale = Engine.Renderer.Scale * Zoom;

        #region integer_scale

        // Find the camera margin and scale from the center.
        // As the current size expands more of the world will come into view until the integer scale changes at which point everything will be resized.
        // This allows for pixel art to scale integerly in FullScale mode.
        Vector2 margin = (currentSize - targetSize) / 2;
        Vector3 pos = posOffset - new Vector3((int) margin.X, (int) margin.Y, 0);
        ViewMatrixUnscaled = Matrix4x4.CreateLookAt(pos, pos + new Vector3(0, 0, -1), new Vector3(0.0f, 1.0f, 0.0f));
        ViewMatrix = Matrix4x4.CreateScale(new Vector3(scale, scale, 1), new Vector3(X, Y, 0)) * ViewMatrixUnscaled;

        #endregion
    }

    public override void Update()
    {
        Vector3 cameraMoveDirection = Vector3.Zero;

        // note any-and-all 'WASD' move input
        if (Engine.InputManager.IsKeyHeld(Key.W)) cameraMoveDirection.Y -= 1;
        if (Engine.InputManager.IsKeyHeld(Key.A)) cameraMoveDirection.X -= 1;
        if (Engine.InputManager.IsKeyHeld(Key.S)) cameraMoveDirection.Y += 1;
        if (Engine.InputManager.IsKeyHeld(Key.D)) cameraMoveDirection.X += 1;

        // If mouse scroll-ed, note Zoom amount and direction
        if (Engine.InputManager.GetMouseScrollRelative() < 0) Zoom += 0.035f * Engine.DeltaTime;
        if (Engine.InputManager.GetMouseScrollRelative() > 0) Zoom -= 0.035f * Engine.DeltaTime;

        // Clamp Camera Zoom
        if (Zoom > 6) Zoom = 6;
        if (Zoom < 0.5) Zoom = 0.5f;

        // If fast-move key down -> quadruple speed coefficient 
        float speed = Speed;
        if (Engine.InputManager.IsKeyHeld(Key.LeftControl)) speed *= 4;
        speed *= Engine.DeltaTime;

        // Since the camera is isometric its X and Y axes are rotated - however we want the movement to be relative to the screen 
        // based on the perspective. To do this we need to find the "proper" up and right directions and move along them.
        cameraMoveDirection *= new Vector3(speed, speed, speed);

        // Finally apply the movement Vector to the camera.
        Engine.Renderer.Camera.Position += cameraMoveDirection;

        if (Engine.InputManager.IsKeyHeld(Key.Home)) Engine.Renderer.Camera.Position = Vector3.Zero;

        // todo: Currently the matrix must be manually recreated as zooming doesn't trigger a recreation.
        RecreateMatrix();
    }
}