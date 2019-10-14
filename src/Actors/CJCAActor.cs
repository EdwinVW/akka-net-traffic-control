using Akka.Actor;
using Messages;
using System;

namespace Actors
{
    /// <summary>
    /// Actor that handles communication with the department of justice (for registering violations).
    /// </summary>
    public class CJCAActor : UntypedActor
    {
        private decimal _totalAmountFined = 0;

        public CJCAActor()
        {
            Console.WriteLine($"Total amount fined: € {_totalAmountFined}\n");
        }

        /// <summary>
        /// Handle received message.
        /// </summary>
        /// <param name="message">The message to handle.</param>
        protected override void OnReceive(object message)
        {
            switch(message)
            {
                case RegisterSpeedingViolation rsv:
                    Handle(rsv);
                    break;
            }
        }

        /// <summary>
        /// Handle a RegisterSpeedingViolation message.
        /// </summary>
        /// <param name="msg">The message to handle.</param>
        private void Handle(RegisterSpeedingViolation msg)
        {
            decimal fine = CJCALogic.CalculateFine(msg.ViolationInKmh);

            _totalAmountFined += fine;

            string fineString = fine == 0 ? "tbd by the prosecutor" : fine.ToString();
            System.Console.WriteLine($"Sent speeding ticket. Road: {msg.RoadId}, Licensenumber: {msg.VehicleId}" + 
                $", Violation: {msg.ViolationInKmh} Km/h, Fine: € {fineString}");

            ConsoleHelpers.PrintAtLocation(0, 2, $"Total amount fined: € {_totalAmountFined}");
        }
    }
}
