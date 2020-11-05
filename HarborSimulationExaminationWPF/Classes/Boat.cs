using System;
using System.Collections.Generic;
using System.Text;

namespace HarborSimulationExaminationWPF
{
    abstract class Boat
    {
        public int AssignedIndex { get; set; }
        public int Counter { get; set; }
        public string BoatType { get; set; }
        public string IDNumber { get; set; }
        public int Weight { get; set; }
        public int MaxSpeed { get; set; }
        public Boat(int assignedIndex, int counter, string boatType, string iDNumber, int weight, int maxSpeed)
        {
            AssignedIndex = assignedIndex;
            Counter = counter;
            BoatType = boatType;
            IDNumber = iDNumber;
            Weight = weight;
            MaxSpeed = maxSpeed;
        }
        public virtual int GetMisc()
        {
            return 0;
        }
        public virtual string GetBoatPropertiesAsString()
        {
            return null;
        }
    }
}
