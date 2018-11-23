using Actors;
using Akka.Actor;
using Akka.Configuration;
using System;
using System.IO;

namespace CollectionAgencyHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            var config = ConfigurationFactory.ParseString(File.ReadAllText("akkaconfig.hocon"));

            using (ActorSystem system = ActorSystem.Create("cjcasystem", config))
            {
                Console.WriteLine("Remote actorsystem for the Central Judicial Collection Agency ready\n");
                Console.ReadKey(true);

                System.Console.WriteLine("Stopped. Press any key to exit.");
                Console.ReadKey(true);
            }
        }
    }
}
