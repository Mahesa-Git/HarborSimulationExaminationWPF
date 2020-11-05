using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimulationExaminationWPF
{
    sealed class RowBoat : Boat
    {
        public int MaxPassengers { get; set; }
        public RowBoat(int assignedIndex, int counter, string boatType, string iDNumber, int weight, int maxSpeed, int maxPassengers)
            : base(assignedIndex, counter, boatType, iDNumber, weight, maxSpeed)
        {
            MaxPassengers = maxPassengers;
        }
        public override int GetMisc()
        {
            return MaxPassengers;
        }
        public override string GetBoatPropertiesAsString()
        {
            return $"*{AssignedIndex + 1}\t\t{BoatType}\t\t{IDNumber}\t\t" +
                   $"{Weight}\t\t{Math.Round(MaxSpeed * 1.852)} km/h\t\t" +
                   $"Passenger limit: {MaxPassengers}\t\t\t{Counter + 1}";
        }
    }
}
