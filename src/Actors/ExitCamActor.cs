using Akka.Actor;
using Messages;

namespace Actors
{
    /// <summary>
    /// Actor that represents an exit camera.
    /// </summary>
    public class ExitCamActor : ReceiveActor
    {
        private ActorSelection _trafficControlActor;

        public ExitCamActor()
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
            VehicleExitRegistered vehicleExitRegistered = new VehicleExitRegistered(msg.VehicleId, msg.Timestamp);
            _trafficControlActor.Tell(vehicleExitRegistered);

        }
    }
}
