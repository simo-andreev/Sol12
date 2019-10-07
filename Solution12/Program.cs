using System;
using Adfectus.Common;
using Adfectus.Common.Configuration;
using Adfectus.Implementation;
using Jint;
using Solution12.Scenes;
using Engine = Adfectus.Common.Engine;

namespace Solution12
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Engine.Setup<Adfectus.Platform.DesktopGL.DesktopPlatform>(new EngineBuilder()
                .SetLogger<ConsoleLogger>()
                .SetupAssets("Assets")
                .SetupHost("Sol-12", WindowMode.Windowed, resizable: true)
//                .SetupFlags(targetTPS: 1)
            );

            Engine.SceneManager.SetScene(new SnekScene());
            Engine.Run();
        }
    }
}