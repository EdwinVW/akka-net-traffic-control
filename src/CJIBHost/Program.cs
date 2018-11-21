using Actors;
using Akka.Actor;
using Akka.Configuration;
using System;
using System.IO;

namespace CJIBHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            var config = ConfigurationFactory.ParseString(File.ReadAllText("akkaconfig.hocon"));

            using (ActorSystem system = ActorSystem.Create("cjibsystem", config))
            {
                Console.WriteLine("Remote actorsystem for CJIB ready\n");
                Console.ReadKey(true);

                System.Console.WriteLine("Stopped. Press any key to exit.");
                Console.ReadKey(true);
            }
        }
    }
}
