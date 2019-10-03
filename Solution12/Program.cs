using System;
using Adfectus.Implementation;
using Jint;
using Engine = Adfectus.Common.Engine;

namespace Solution12
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            Engine.Setup<Adfectus.Platform.DesktopGL.DesktopPlatform>();
            Engine.SceneManager.SetScene(new SoScene());
            Engine.Run();
        }
    }
}