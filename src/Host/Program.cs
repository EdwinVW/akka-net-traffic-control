using Actors;
using Akka.Actor;
using Akka.Configuration;
using Akka.Routing;
using Messages;
using System;
using System.IO;

namespace Host
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;

            var config = ConfigurationFactory.ParseString(File.ReadAllText("akkaconfig.hocon"));

            using (ActorSystem system = ActorSystem.Create("TrafficControlSystem", config))
            {
                (string RoadId, 
                 int SectionLengthInKm, 
                 int MaxAllowedSpeedInKmh, 
                 int LegalCorrectionInKmh) roadInfo = ("A2", 10, 100, 5);
                var trafficControlProps = Props.Create<TrafficControlActor>(roadInfo)
                    .WithRouter(new RoundRobinPool(3));
                var trafficControlActor = system.ActorOf(trafficControlProps, "traffic-control");

                var entryCamActor1 = system.ActorOf<EntryCamActor>("entrycam1");
                var entryCamActor2 = system.ActorOf<EntryCamActor>("entrycam2");
                var entryCamActor3 = system.ActorOf<EntryCamActor>("entrycam3");

                var exitCamActor1 = system.ActorOf<ExitCamActor>("exitcam1");
                var exitCamActor2 = system.ActorOf<ExitCamActor>("exitcam2");
                var exitCamActor3 = system.ActorOf<ExitCamActor>("exitcam3");

                var cjibActor = system.ActorOf<CJIBActor>("cjibactor");
                //var cjibActor = system.ActorOf<PersistentCJIBActor>("cjibactor");

                var simulationProps = Props.Create<SimulationActor>().WithRouter(new BroadcastPool(3));
                var simulationActor = system.ActorOf(simulationProps);

                Console.WriteLine("Actorsystem and actor created. Press any key to start simulation\n");
                Console.ReadKey(true);

                simulationActor.Tell(new StartSimulation(15));

                Console.ReadKey(true);
            }
        }
    }
}
