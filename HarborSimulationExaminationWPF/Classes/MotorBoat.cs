using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimulationExaminationWPF
{
    sealed class MotorBoat : Boat
    {
        public int NrOfHorsePower { get; set; }
        public MotorBoat(int assignedIndex, int counter, string boatType, string iDNumber, int weight, int maxSpeed, int nrOfHorsePower)
            : base(assignedIndex, counter, boatType, iDNumber, weight, maxSpeed)
        {
            NrOfHorsePower = nrOfHorsePower;
        }
        public override int GetMisc()
        {
            return NrOfHorsePower;
        }
        public override string GetBoatPropertiesAsString()
        {
            if (Math.Round(MaxSpeed * 1.852) >= 100 && (NrOfHorsePower >= 1000))
            {
                return $"*{AssignedIndex + 1}\t\t{BoatType}\t{IDNumber}\t\t" +
                   $"{Weight}\t\t{Math.Round(MaxSpeed * 1.852)} km/h\t" +
                   $"Horsepower: {NrOfHorsePower}\t{Counter + 1}";
            }
            else if (Math.Round(MaxSpeed * 1.852) >= 100)
            {
                return $"*{AssignedIndex + 1}\t\t{BoatType}\t{IDNumber}\t\t" +
                   $"{Weight}\t\t{Math.Round(MaxSpeed * 1.852)} km/h\t" +
                   $"Horsepower: {NrOfHorsePower}\t\t\t{Counter + 1}";
            }
            else if (NrOfHorsePower >= 1000)
            {
                return $"*{AssignedIndex + 1}\t\t{BoatType}\t{IDNumber}\t\t" +
                   $"{Weight}\t\t{Math.Round(MaxSpeed * 1.852)} km/h\t\t" +
                   $"Horsepower: {NrOfHorsePower}\t\t{Counter + 1}";
            }
            
            else
            {
                return $"*{AssignedIndex + 1}\t\t{BoatType}\t{IDNumber}\t\t" +
                   $"{Weight}\t\t{Math.Round(MaxSpeed * 1.852)} km/h\t\t" +
                   $"Horsepower: {NrOfHorsePower}\t\t\t{Counter + 1}";
            }
        }
    }
}
