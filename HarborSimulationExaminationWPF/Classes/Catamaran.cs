using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimulationExaminationWPF
{
    sealed class Catamaran : Boat
    {
        public int NrOfSleepingPlaces { get; set; }
        public Catamaran(int assignedIndex, int counter, string boatType, string iDNumber, int weight, int maxSpeed, int nrOfSleepingPlaces)
            : base(assignedIndex, counter, boatType, iDNumber, weight, maxSpeed)
        {
            NrOfSleepingPlaces = nrOfSleepingPlaces;
        }
        public override int GetMisc()
        {
            return NrOfSleepingPlaces;
        }
        public override string GetBoatPropertiesAsString()
        {
            return $"*{AssignedIndex + 1}-{AssignedIndex + 3}\t\t{BoatType}\t{IDNumber}\t\t" +
                   $"{Weight}\t\t{Math.Round(MaxSpeed * 1.852)} km/h\t\t" +
                   $"Bunks on board: {NrOfSleepingPlaces}\t\t\t{Counter + 1}";
        }
    }
}
