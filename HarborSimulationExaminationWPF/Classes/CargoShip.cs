using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimulationExaminationWPF
{
    sealed class CargoShip : Boat
    {
        public int ContainersOnCargo { get; set; }
        public CargoShip(int assignedIndex, int counter, string boatType, string iDNumber, int weight, int maxSpeed, int containersOnCargo)
            : base(assignedIndex, counter, boatType, iDNumber, weight, maxSpeed)
        {
            ContainersOnCargo = containersOnCargo;
        }
        public override int GetMisc()
        {
            return ContainersOnCargo;
        }
        public override string GetBoatPropertiesAsString()
        {
            return $"*{AssignedIndex - 2}-{AssignedIndex + 1}\t\t{BoatType}\t{IDNumber}\t\t" +
                   $"{Weight}\t\t{Math.Round(MaxSpeed * 1.852)} km/h\t\t" +
                   $"Containers: {ContainersOnCargo}\t\t\t{Counter + 1}";
        }
    }
}
