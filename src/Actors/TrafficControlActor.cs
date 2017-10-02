using Akka.Actor;
using Messages;

namespace Actors
{
    /// <summary>
    /// Actor that handles traffic control.
    /// </summary>
    public class TrafficControlActor : ReceiveActor
    {
        private (string RoadId, int SectionLengthInKm, int MaxAllowedSpeedInKmh, int LegalCorrectionInKmh) _roadInfo;

        public TrafficControlActor((string RoadId, int SectionLengthInKm, int MaxAllowedSpeedInKmh, int LegalCorrectionInKmh) roadInfo)
        {
            // initialize state
            _roadInfo = roadInfo;

            // setup message-handling
            Receive<VehicleEntryRegistered>(msg => Handle(msg));
            Receive<VehicleExitRegistered>(msg => Handle(msg));
        }

        /// <summary>
        /// Handle VehicleEntryRegistered message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(VehicleEntryRegistered msg)
        {
            var props = Props.Create<VehicleActor>(_roadInfo);
            var vehicleActor = Context.ActorOf(props, $"vehicle-{msg.VehicleId}");
            vehicleActor.Tell(msg);
        }

        /// <summary>
        /// Handle VehicleExitRegistered message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(VehicleExitRegistered msg)
        {
            var vehicleActor = Context.ActorSelection($"/user/traffic-control/*/vehicle-{msg.VehicleId}");
            vehicleActor.Tell(msg);
        }
    }
}
