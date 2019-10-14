using Akka.Actor;
using Messages;

namespace Actors
{
    /// <summary>
    /// Actor that represents an exit camera.
    /// </summary>
    public class ExitCamActor : UntypedActor
    {
        private ActorSelection _trafficControlActor;

        public ExitCamActor()
        {
            // initialize state
            _trafficControlActor = Context.System.ActorSelection("/user/traffic-control");
        }

        /// <summary>
        /// Handle received message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        protected override void OnReceive(object message)
        {
            switch(message)
            {
                case VehiclePassed vp:
                    Handle(vp);
                    break;
            }
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
