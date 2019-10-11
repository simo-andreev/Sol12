using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using Emotion.Common;
using Emotion.Graphics.Camera;
using Emotion.Graphics.Command;
using Emotion.Graphics.Objects;
using Emotion.IO;
using Emotion.Plugins.ImGuiNet;
using Emotion.Primitives;
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
            );

            Engine.AssetLoader.AddSource(new FileAssetSource("assets/iMage"));
            Engine.AssetLoader.AddSource(new FileAssetSource("assets/Font"));
            Engine.AssetLoader.AddSource(new FileAssetSource("assets/SicBeats"));
            
            Engine.Renderer.Camera = new SolCam(new Vector3(0, 0, 200));

            new TexturaMagna().Attach();

            Engine.Run();
        }
    }
}

class SolCam : CameraBase
{
    public SolCam(Vector3 position, float zoom = 1) : base(position, zoom)
    {
    }
    
    

    public override void RecreateMatrix()
    {
        base.RecreateMatrix();
        ViewMatrix = Matrix4x4.CreateRotationX(4) * ViewMatrixUnscaled;
    }
}