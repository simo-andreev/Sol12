﻿using System;
using Adfectus.Common;
using Adfectus.Common.Configuration;
using Adfectus.ImGuiNet;
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
                    .AddGenericPlugin(new ImGuiNetPlugin())
//                .SetupFlags( /*targetTPS: 1*/ performBootstrap: true)
            );
            Engine.SceneManager.SetScene(new SceneScene());
            Engine.Run();
        }
    }
}