using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.CompilerServices;
using Emotion.Common;
using Emotion.Graphics.Camera;
using Emotion.Graphics.Command;
using Emotion.Graphics.Objects;
using Emotion.IO;
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
            );

            Engine.AssetLoader.AddSource(new FileAssetSource("assets/iMage"));
            Engine.AssetLoader.AddSource(new FileAssetSource("assets/Font"));
            Engine.AssetLoader.AddSource(new FileAssetSource("assets/SicBeats"));

            Engine.Renderer.FarZ = 600000;
            Engine.Renderer.NearZ = -600000;
            Engine.Renderer.Camera = new SolCam(new Vector3(0, 0, 0));

            Engine.SceneManager.SetScene(new TexturaMagna());
            
            Engine.Run();
        }
    }
}

class SolCam : CameraBase
{
    public static Vector3 Rotation;

    public SolCam(Vector3 position, float zoom = 1) : base(position, zoom)
    {
    }

    public override void Update()
    {
        base.Update();
        RecreateMatrix();
    }

    public override void RecreateMatrix()
    {
        base.RecreateMatrix();
        ViewMatrix = Matrix4x4.CreateFromYawPitchRoll(MathExtension.DegreesToRadians(Rotation.X), MathExtension.DegreesToRadians(Rotation.Y), MathExtension.DegreesToRadians(Rotation.Z)) * ViewMatrix;
    }
}