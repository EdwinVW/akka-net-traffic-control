using Akka.Actor;
using Messages;

namespace Actors
{
    /// <summary>
    /// Actor that handles traffic control.
    /// </summary>
    public class TrafficControlActor : UntypedActor
    {
        private RoadInfo _roadInfo;

        public TrafficControlActor(RoadInfo roadInfo)
        {
            // initialize state
            _roadInfo = roadInfo;
        }

        /// <summary>
        /// Handle received message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        protected override void OnReceive(object message)
        {
            switch(message)
            {
                case VehicleEntryRegistered ver:
                    Handle(ver);
                    break;
                case VehicleExitRegistered vxr:
                    Handle(vxr);
                    break;            
            }
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
