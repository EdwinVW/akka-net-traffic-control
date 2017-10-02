using Akka.Actor;
using Messages;

namespace Actors
{
    /// <summary>
    /// Actor that represents an entry camera.
    /// </summary>
    public class EntryCamActor : ReceiveActor
    {
        private ActorSelection _trafficControlActor;

        public EntryCamActor()
        {
            // initialize state
            _trafficControlActor = Context.System.ActorSelection("/user/traffic-control");

            // setup message-handling
            Receive<VehiclePassed>(msg => Handle(msg));
        }

        /// <summary>
        /// Handle VehiclePassed message
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(VehiclePassed msg)
        {
            VehicleEntryRegistered vehicleEntryRegistered = 
                new VehicleEntryRegistered(msg.VehicleId, msg.Timestamp);
            _trafficControlActor.Tell(vehicleEntryRegistered);
        }
    }
}
