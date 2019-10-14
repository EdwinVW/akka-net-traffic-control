using Akka.Persistence;
using Messages;
using System;

namespace Actors
{
    /// <summary>
    /// Actor that handles communication with the department of justice (for registering violations).
    /// </summary>
    public class PersistentCJCAActor : UntypedPersistentActor
    {
        private decimal _totalAmountFined = 0;

        public override string PersistenceId => "PersistentCJCAActor";

        public PersistentCJCAActor()
        {
            Console.WriteLine();
            Console.WriteLine();
        }

        protected override void OnCommand(object message)
        {
            switch(message)
            {
                case RegisterSpeedingViolation rsv:
                    Persist(rsv, Handle);
                    break;

            }            
        }

        protected override void OnRecover(object message)
        {
            switch(message)
            {
                case RegisterSpeedingViolation rsv:
                    Handle(rsv);
                    break;
                case RecoveryCompleted rc:
                    ShowTotal();
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

            if (!IsRecovering)
            {
                string fineString = fine == 0 ? "tbd by the prosecutor" : fine.ToString();
                System.Console.WriteLine($"Sent speeding ticket. Road: {msg.RoadId}, Licensenumber: {msg.VehicleId}" +
                    $", Violation: {msg.ViolationInKmh} Km/h, Fine: € {fineString}");

                ShowTotal();
            }
        }

        #region Private helper methods

        /// <summary>
        /// Show total amount fined.
        /// </summary>
        private void ShowTotal()
        {
            ConsoleHelpers.PrintAtLocation(0, 2, $"Total amount fined: € {_totalAmountFined}");
        }

        #endregion
    }
}
