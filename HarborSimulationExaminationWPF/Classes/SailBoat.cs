using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimulationExaminationWPF
{
    sealed class SailBoat : Boat
    {
        public int BoatLenght { get; set; }
        public SailBoat(int assignedIndex, int counter, string boatType, string iDNumber, int weight, int maxSpeed, int boatLenght)
            : base(assignedIndex, counter, boatType, iDNumber, weight, maxSpeed)
        {
            BoatLenght = boatLenght;
        }
        public override int GetMisc()
        {
            return BoatLenght;
        }
        public override string GetBoatPropertiesAsString()
        {

            return $"*{AssignedIndex}-{AssignedIndex+1}\t\t{BoatType}\t\t{IDNumber}\t\t" +
                   $"{Weight}\t\t{Math.Round(MaxSpeed * 1.852)} km/h\t\t" +
                   $"Boat lenght: {Math.Round(BoatLenght* 0.3048)}\t\t\t{Counter + 1}";
        }
    }
    
}
