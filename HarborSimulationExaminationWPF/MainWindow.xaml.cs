using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace HarborSimulationExaminationWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Alpha_Presentation.Content = "Place:\t\tType:\t\tNr:\t\tWeight:\t\tTopspeed:\tMiscellaneous:\t\tStayed days:";
            Beta_Presentation.Content = "Place:\t\tType:\t\tNr:\t\tWeight:\t\tTopspeed:\tMiscellaneous:\t\tStayed days:";
            FileExistsCheck();
            Load();
            CalculatePropertiesAndCountBoats();
            Print();
        }

        private Boat[] HarborAlpha = new Boat[32];
        private Boat[] HarborBeta = new Boat[32];
        private Boat[] SharedSpotAlpha = new Boat[32];
        private Boat[] SharedSpotBeta = new Boat[32];
        private Random Rnd = new Random();
        private string IterateStopper;
        private string BoatsInHarborsMessage = null;
        private string BoatsThatDidNotFitMsg = null;
        private int BoatsThatDidNotFitNr = 0;
        private int NrOfFreeSpots = 0;
        private int TotalWeight = 0;
        private double AvgBoatSpeed = 0;


        //PROGRAM
        private void RunProgram(int thisMany)
        {
            List<Boat> generatedNewBoats = BoatGenerator(thisMany);

            HarborArrayIndexingMethod(generatedNewBoats);

            CalculatePropertiesAndCountBoats();

            Save();

            Print();

            UpdateTimePropertiesAndRemoveBoats();

        }
        private List<Boat> BoatGenerator(int thisMany)
        {
            List<Boat> newBoats = new List<Boat>();
            for (int i = 0; i < thisMany; i++)
            {
                int thisType = Rnd.Next(1, 5 + 1);
                string boatId = null;
                bool nameAlreadyExists = true;
                while (nameAlreadyExists)
                {
                    List<Boat> gatheredList = new List<Boat>();
                    gatheredList.AddRange(HarborAlpha);
                    gatheredList.AddRange(HarborBeta);
                    gatheredList.AddRange(SharedSpotAlpha);
                    gatheredList.AddRange(SharedSpotBeta);

                    var NameExists = gatheredList
                        .Where(b => b != null)
                        .GroupBy(b => b.IDNumber)
                        .Select(b => b.First())
                        .Select(b => $"{b.IDNumber[2]}{b.IDNumber[3]}{b.IDNumber[4]}")
                        .ToList();

                    for (int j = 0; j < 3; j++)
                    {
                        char randomChar = (char)Rnd.Next('a', 'z');
                        boatId += randomChar.ToString().ToUpper();
                    }
                    nameAlreadyExists = NameExists.Contains(boatId);
                }
                switch (thisType)
                {
                    case 1:
                        string rbType = "Rowboat";
                        int rbWeight = Rnd.Next(100, 300 + 1);
                        int rbMaxSpeed = Rnd.Next(0, 3 + 1);
                        int rbMaxPassengers = Rnd.Next(1, 6 + 1);
                        RowBoat rowBoat = new RowBoat(0, 0, rbType, $"R-{boatId}", rbWeight, rbMaxSpeed, rbMaxPassengers);
                        newBoats.Add(rowBoat);
                        break;
                    case 2:
                        string mbType = "Motorboat";
                        int mbWeight = Rnd.Next(200, 3000 + 1);
                        int mbMaxSpeed = Rnd.Next(0, 60 + 1);
                        int mbAmtOfHP = Rnd.Next(10, 1000 + 1);
                        MotorBoat motorBoat = new MotorBoat(0, 0, mbType, $"M-{boatId}", mbWeight, mbMaxSpeed, mbAmtOfHP);
                        newBoats.Add(motorBoat);
                        break;
                    case 3:
                        string sbType = "Sailboat";
                        int sbWeight = Rnd.Next(800, 6000 + 1);
                        int sbMaxSpeed = Rnd.Next(0, 12 + 1);
                        int sbLenght = Rnd.Next(10, 60 + 1);
                        SailBoat sailBoat = new SailBoat(0, 0, sbType, $"S-{boatId}", sbWeight, sbMaxSpeed, sbLenght);
                        newBoats.Add(sailBoat);
                        break;
                    case 4:
                        string csType = "Cargoship";
                        int csWeight = Rnd.Next(3000, 20000 + 1);
                        int csMaxSpeed = Rnd.Next(0, 20 + 1);
                        int csContainers = Rnd.Next(0, 500 + 1);
                        CargoShip cargoShip = new CargoShip(0, 0, csType, $"L-{boatId}", csWeight, csMaxSpeed, csContainers);
                        newBoats.Add(cargoShip);
                        break;
                    default:
                        string cType = "Catamaran";
                        int cWeight = Rnd.Next(1200, 8000 + 1);
                        int cMaxSpeed = Rnd.Next(0, 12 + 1);
                        int cAmtOfBeds = Rnd.Next(1, 4 + 1);
                        Catamaran catamaran = new Catamaran(0, 0, cType, $"K-{boatId}", cWeight, cMaxSpeed, cAmtOfBeds);
                        newBoats.Add(catamaran);
                        break;
                }
            }
            return newBoats;
        }
        private void HarborArrayIndexingMethod(List<Boat> generatedNewBoats)
        {
            foreach (var boat in generatedNewBoats)
            {
                for (int i = 0; i < HarborAlpha.Length; i++)
                {
                    if (boat is RowBoat)
                    {
                        while (true)
                        {
                            if (HarborAlpha[i] == null)
                            {
                                HarborAlpha[i] = boat;
                                boat.AssignedIndex = i;
                                break;
                            }
                            else if (HarborAlpha[i] is RowBoat && SharedSpotAlpha[i] == null)
                            {
                                SharedSpotAlpha[i] = boat;
                                boat.AssignedIndex = i;
                                break;
                            }
                            else
                            {
                                bool indexSuccessRow = false;
                                if (i == 31)
                                {
                                    for (int j = 0; j < HarborBeta.Length; j++)
                                    {
                                        if (HarborBeta[j] == null)
                                        {
                                            HarborBeta[j] = boat;
                                            boat.AssignedIndex = j;
                                            indexSuccessRow = true;
                                            break;
                                        }
                                        else if (HarborBeta[j] is RowBoat && SharedSpotBeta[j] == null)
                                        {
                                            SharedSpotBeta[j] = boat;
                                            boat.AssignedIndex = j;
                                            indexSuccessRow = true;
                                            break;
                                        }
                                    }
                                    if (indexSuccessRow == false)
                                    {
                                        BoatsThatDidNotFitNr++;
                                        BoatsThatDidNotFitMsg += $"{boat.BoatType}. ";
                                    }
                                    break;
                                }
                                else
                                    i++;
                            }
                        }
                        break;
                    }
                    else if (boat is MotorBoat)
                    {
                        while (true)
                        {
                            if (HarborAlpha[i] == null)
                            {
                                HarborAlpha[i] = boat;
                                boat.AssignedIndex = i;
                                break;
                            }
                            else
                            {
                                bool indexSuccessMotor = false;
                                if (i == 31)
                                {
                                    for (int betaIndex = 0; betaIndex < HarborBeta.Length; betaIndex++)
                                    {
                                        if (HarborBeta[betaIndex] == null)
                                        {
                                            HarborBeta[betaIndex] = boat;
                                            boat.AssignedIndex = betaIndex;
                                            indexSuccessMotor = true;
                                            break;
                                        }
                                    }
                                    if (indexSuccessMotor == false)
                                    {
                                        BoatsThatDidNotFitNr++;
                                        BoatsThatDidNotFitMsg += $"{boat.BoatType}. ";
                                    }
                                    break;
                                }
                                else
                                    i++;
                            }
                        }
                        break;
                    }
                    else if (boat is Catamaran)
                    {
                        while (true)
                        {
                            if (HarborAlpha[i] == null && HarborAlpha[i + 1] == null && HarborAlpha[i + 2] == null)
                            {
                                HarborAlpha[i] = boat;
                                HarborAlpha[i + 1] = boat;
                                HarborAlpha[i + 2] = boat;
                                boat.AssignedIndex = i;
                                break;
                            }
                            else
                            {
                                bool indexSuccessCata = false;
                                if (i == 29)
                                {
                                    for (int j = 0; j < HarborBeta.Length; j++)
                                    {
                                        if (HarborBeta[j] == null && HarborBeta[j + 1] == null && HarborBeta[j + 2] == null)
                                        {
                                            HarborBeta[j] = boat;
                                            HarborBeta[j + 1] = boat;
                                            HarborBeta[j + 2] = boat;
                                            boat.AssignedIndex = j;
                                            indexSuccessCata = true;
                                            break;
                                        }
                                    }
                                    if (indexSuccessCata == false)
                                    {
                                        BoatsThatDidNotFitNr++;
                                        BoatsThatDidNotFitMsg += $"{boat.BoatType}. ";
                                    }
                                    break;
                                }
                                else
                                    i++;
                            }
                        }
                        break;
                    }
                }
                for (int i = HarborAlpha.Length - 1; i > 0; i--)
                {
                    if (boat is SailBoat)
                    {
                        while (true)
                        {
                            if (HarborAlpha[i] == null && HarborAlpha[i - 1] == null)
                            {
                                HarborAlpha[i] = boat;
                                HarborAlpha[i - 1] = boat;
                                boat.AssignedIndex = i;
                                break;
                            }
                            else
                            {
                                bool indexSuccessSail = false;
                                if (i == 1)
                                {
                                    for (int j = HarborBeta.Length - 1; j > 0; j--)
                                    {

                                        if (HarborBeta[j] == null && HarborBeta[j - 1] == null)
                                        {
                                            HarborBeta[j] = boat;
                                            HarborBeta[j - 1] = boat;
                                            boat.AssignedIndex = j;
                                            indexSuccessSail = true;
                                            break;
                                        }
                                    }
                                    if (indexSuccessSail == false)
                                    {
                                        BoatsThatDidNotFitNr++;
                                        BoatsThatDidNotFitMsg += $"{boat.BoatType}. ";
                                    }
                                    break;
                                }
                                else
                                    i--;
                            }
                        }
                        break;
                    }
                }
                for (int j = HarborBeta.Length - 1; j > 0; j--)
                {
                    if (boat is CargoShip)
                    {
                        while (true)
                        {
                            if (HarborBeta[j] == null && HarborBeta[j - 1] == null && HarborBeta[j - 2] == null && HarborBeta[j - 3] == null)
                            {
                                HarborBeta[j] = boat;
                                HarborBeta[j - 1] = boat;
                                HarborBeta[j - 2] = boat;
                                HarborBeta[j - 3] = boat;
                                boat.AssignedIndex = j;
                                break;
                            }
                            else
                            {
                                bool indexSuccessCargo = false;
                                if (j == 3)
                                {
                                    for (int i = HarborAlpha.Length - 1; i > 0; i--)
                                    {
                                        if (HarborAlpha[i] == null && HarborAlpha[i - 1] == null && HarborAlpha[i - 2] == null && HarborAlpha[i - 3] == null)
                                        {
                                            HarborAlpha[i] = boat;
                                            HarborAlpha[i - 1] = boat;
                                            HarborAlpha[i - 2] = boat;
                                            HarborAlpha[i - 3] = boat;
                                            boat.AssignedIndex = i;
                                            indexSuccessCargo = true;
                                            break;
                                        }
                                    }
                                    if (indexSuccessCargo == false)
                                    {
                                        BoatsThatDidNotFitNr++;
                                        BoatsThatDidNotFitMsg += $"{boat.BoatType}. ";
                                    }
                                    break;
                                }
                                else
                                    j--;
                            }
                        }
                        break;
                    }
                }
            }
        }
        private void CalculatePropertiesAndCountBoats()
        {
            List<Boat> harbors = new List<Boat>();
            harbors.AddRange(HarborAlpha);
            harbors.AddRange(HarborBeta);

            var emptySpotsQuery = harbors
                .Where(b => b == null);

            NrOfFreeSpots = emptySpotsQuery
                .Count();

            harbors.AddRange(SharedSpotAlpha);
            harbors.AddRange(SharedSpotBeta);

            var noDuplicatesQuery = harbors
                .Where(b => b != null)
                .GroupBy(b => b.IDNumber)
                .Select(b => b.First())
                .ToList();

            var boatSpeedQuery = noDuplicatesQuery
                .Select(b => b.MaxSpeed);

            if (boatSpeedQuery.Count() > 0)
            {
                AvgBoatSpeed = boatSpeedQuery
                    .Average();
            }

            var boatWeightQuery = noDuplicatesQuery
                .Select(b => b.Weight);

            TotalWeight = boatWeightQuery
                .Sum();

            var nrOfBoatsQuery = noDuplicatesQuery
                .OrderBy(b => b.BoatType)
                .GroupBy(b => b.BoatType);

            foreach (var group in nrOfBoatsQuery)
            {
                BoatsInHarborsMessage += $"{group.Key}s: {group.Count()}. ";
            }
        }
        private void Print()
        {
            int indexCounter = 0;

            //ALPHA
            List<Boat> alphaAll = new List<Boat>();
            alphaAll.AddRange(HarborAlpha);

            var boatsInAlphaSharedQuery = SharedSpotAlpha
                .Where(b => b != null);

            foreach (var boat in boatsInAlphaSharedQuery)
            {
                alphaAll.Insert(boat.AssignedIndex, boat);
            }

            foreach (var boat in alphaAll)
            {
                if (indexCounter > 31)
                    break;
                if (boat == null)
                    Alpha_ListBox.Items.Add($"*{indexCounter + 1}\t\tEmpty");

                else if (boat != null && IterateStopper == boat.IDNumber)
                {
                    indexCounter++;
                    continue;
                }
                else if (boat != null && IterateStopper != boat.IDNumber)
                {
                    Alpha_ListBox.Items.Add(boat.GetBoatPropertiesAsString());
                    IterateStopper = boat.IDNumber;
                    if (boat is RowBoat && indexCounter != 0)
                    {
                        if (boat.AssignedIndex == alphaAll[indexCounter - 1].AssignedIndex)
                            continue;
                    }
                }
                indexCounter++;
            }

            //BETA
            indexCounter = 0;
            List<Boat> betaAll = new List<Boat>();
            betaAll.AddRange(HarborBeta);

            var boatsInBetaSharedQuery = SharedSpotBeta
                .Where(b => b != null);

            foreach (var boat in boatsInBetaSharedQuery)
            {
                betaAll.Insert(boat.AssignedIndex, boat);
            }
            foreach (var boat in betaAll)
            {
                if (indexCounter > 31)
                    break;
                if (boat == null)
                    Beta_ListBox.Items.Add($"*{indexCounter + 1}\t\tEmpty");

                else if (boat != null && IterateStopper == boat.IDNumber)
                {
                    indexCounter++;
                    continue;
                }
                else if (boat != null && IterateStopper != boat.IDNumber)
                {
                    Beta_ListBox.Items.Add(boat.GetBoatPropertiesAsString());
                    IterateStopper = boat.IDNumber;
                    if (boat is RowBoat && indexCounter != 0)
                    {
                        if (boat.AssignedIndex == betaAll[indexCounter - 1].AssignedIndex)
                            continue;
                    }
                }
                indexCounter++;
            }

            //INFO
            Info_ListBox.Items.Add($"Docked types of boats: {BoatsInHarborsMessage}");
            Info_ListBox.Items.Add($"Total weight: {TotalWeight} kg.");
            Info_ListBox.Items.Add($"Average topspeed: {Math.Round(AvgBoatSpeed * 1.852)} km/h.");
            Info_ListBox.Items.Add($"Number of free spots: {NrOfFreeSpots}.");
            Info_ListBox.Items.Add($"Rejected boats: {BoatsThatDidNotFitNr}. {BoatsThatDidNotFitMsg}");

            //RESET FIELDS
            NrOfFreeSpots = 0;
            TotalWeight = 0;
            AvgBoatSpeed = 0;
            BoatsThatDidNotFitNr = 0;
            BoatsInHarborsMessage = null;
            BoatsThatDidNotFitMsg = null;
        }
        private void UpdateTimePropertiesAndRemoveBoats()
        {
            List<Boat[]> allArrays = new List<Boat[]>();
            allArrays.Add(SharedSpotAlpha);
            allArrays.Add(SharedSpotBeta);
            allArrays.Add(HarborAlpha);
            allArrays.Add(HarborBeta);

            foreach (var array in allArrays)
            {
                foreach (var boat in array)
                {
                    if (boat == null)
                        continue;

                    if (boat.IDNumber == IterateStopper)
                        continue;

                    boat.Counter++;

                    if (boat is RowBoat && boat.Counter == 1)
                        array[boat.AssignedIndex] = null;

                    if (boat is MotorBoat && boat.Counter == 3)
                        array[boat.AssignedIndex] = null;

                    if (boat is SailBoat && boat.Counter == 4)
                    {
                        array[boat.AssignedIndex] = null;
                        array[boat.AssignedIndex - 1] = null;
                    }
                    if (boat is Catamaran && boat.Counter == 3)
                    {
                        array[boat.AssignedIndex] = null;
                        array[boat.AssignedIndex + 1] = null;
                        array[boat.AssignedIndex + 2] = null;
                    }
                    if (boat is CargoShip && boat.Counter == 6)
                    {
                        array[boat.AssignedIndex] = null;
                        array[boat.AssignedIndex - 1] = null;
                        array[boat.AssignedIndex - 2] = null;
                        array[boat.AssignedIndex - 3] = null;
                    }
                    IterateStopper = boat.IDNumber;
                }
            }
        }

        //FILE
        private void FileExistsCheck()
        {
            if (!Directory.Exists("File"))
                Directory.CreateDirectory("File");


            string[] filePaths = new string[5] { @"File\AlphaHarbor.txt", @"File\AlphaSharedSpot.txt",
                                                 @"File\BetaHarbor.txt", @"File\BetaSharedSpot.txt",
                                                 @"File\BoatsThatDidNotFit.txt" };
            foreach (var filePath in filePaths)
            {
                if (!File.Exists(filePath))
                    File.Create(filePath).Close();
            }
        }
        private void ClearFile(string path)
        {
            if (File.Exists(path))
                File.WriteAllText($"{path}", string.Empty);
        }
        private void Load()
        {
            LoadFile(@"File\AlphaHarbor.txt", HarborAlpha);
            LoadFile(@"File\AlphaSharedSpot.txt", SharedSpotAlpha);
            LoadFile(@"File\BetaHarbor.txt", HarborBeta);
            LoadFile(@"File\BetaSharedSpot.txt", SharedSpotBeta);

            string[] loadBoatsThatDidNotFit = File.ReadAllLines(@"File\BoatsThatDidNotFit.txt");
            if (loadBoatsThatDidNotFit.Length != 0)
            {
                BoatsThatDidNotFitNr = Convert.ToInt32(loadBoatsThatDidNotFit[0]);
                BoatsThatDidNotFitMsg = (loadBoatsThatDidNotFit[1]);
            }
        }
        private void LoadFile(string filePath, Boat[] arrayType)
        {
            string[] loadText = File.ReadAllLines($"{filePath}");

            for (int i = 0; i < loadText.Length; i++)
            {
                if (loadText[i] == "Empty" || string.IsNullOrEmpty(loadText[i]))
                    arrayType[i] = null;
                else
                {
                    string[] splitInfo = loadText[i].Split(';');
                    string boatName = splitInfo[2].ToUpper();

                    switch (boatName)
                    {
                        case "ROWBOAT":
                            RowBoat rowBoat = new RowBoat(Convert.ToInt32(splitInfo[0]), Convert.ToInt32(splitInfo[1]), splitInfo[2], splitInfo[3],
                                                      Convert.ToInt32(splitInfo[4]), Convert.ToInt32(splitInfo[5]), Convert.ToInt32(splitInfo[6]));
                            arrayType[i] = rowBoat;
                            break;
                        case "MOTORBOAT":
                            MotorBoat motorBoat = new MotorBoat(Convert.ToInt32(splitInfo[0]), Convert.ToInt32(splitInfo[1]), splitInfo[2], splitInfo[3],
                                                            Convert.ToInt32(splitInfo[4]), Convert.ToInt32(splitInfo[5]), Convert.ToInt32(splitInfo[6]));
                            arrayType[i] = motorBoat;
                            break;
                        case "SAILBOAT":
                            SailBoat sailBoat = new SailBoat(Convert.ToInt32(splitInfo[0]), Convert.ToInt32(splitInfo[1]), splitInfo[2], splitInfo[3],
                                                         Convert.ToInt32(splitInfo[4]), Convert.ToInt32(splitInfo[5]), Convert.ToInt32(splitInfo[6]));
                            arrayType[i] = sailBoat;
                            break;
                        case "CATAMARAN":
                            Catamaran catamaran = new Catamaran(Convert.ToInt32(splitInfo[0]), Convert.ToInt32(splitInfo[1]), splitInfo[2], splitInfo[3],
                                                            Convert.ToInt32(splitInfo[4]), Convert.ToInt32(splitInfo[5]), Convert.ToInt32(splitInfo[6]));
                            arrayType[i] = catamaran;
                            break;
                        default:
                            CargoShip cargoShip = new CargoShip(Convert.ToInt32(splitInfo[0]), Convert.ToInt32(splitInfo[1]), splitInfo[2], splitInfo[3],
                                                            Convert.ToInt32(splitInfo[4]), Convert.ToInt32(splitInfo[5]), Convert.ToInt32(splitInfo[6]));
                            arrayType[i] = cargoShip;
                            break;
                    }
                }
            }
        }
        private void Save()
        {
            SaveFile(HarborAlpha, $@"File\AlphaHarbor.txt");
            SaveFile(SharedSpotAlpha, $@"File\AlphaSharedSpot.txt");
            SaveFile(HarborBeta, $@"File\BetaHarbor.txt");
            SaveFile(SharedSpotBeta, $@"File\BetaSharedSpot.txt");

            using (StreamWriter sw = new StreamWriter(@"File\BoatsThatDidNotFit.txt", false))
            {
                sw.WriteLine(BoatsThatDidNotFitNr);
                sw.WriteLine(BoatsThatDidNotFitMsg);
            }
        }
        private void SaveFile(Boat[] array, string path)
        {
            using (StreamWriter sw = new StreamWriter(path, false))
            {
                foreach (var boat in array)
                {
                    if (boat == null)
                        sw.WriteLine("Empty");
                    else
                        sw.WriteLine($"{boat.AssignedIndex};{boat.Counter};{boat.BoatType};{boat.IDNumber};{boat.Weight};{boat.MaxSpeed};{boat.GetMisc()}");
                }
            }
        }

        //EVENTS 
        private void Run_Next_Day_Button_Click(object sender, RoutedEventArgs e)
        {
            Info_ListBox.Items.Clear();
            Alpha_ListBox.Items.Clear();
            Beta_ListBox.Items.Clear();
            try
            {
                int thisManyBoatsPerDay = int.Parse(Boats_Per_Day_TextBox.Text);
                Error_Label.Visibility = Visibility.Hidden;
                RunProgram(thisManyBoatsPerDay);
            }
            catch
            {
                Error_Label.Visibility = Visibility.Visible;
            }
        }
        private void MouseEnter_Run_Next_Day_Button(object sender, System.Windows.Input.MouseEventArgs e)
        {

            Run_Next_Day_Button.Foreground = Brushes.Black;
        }
        private void MouseLeave_Run_Next_Day_Button(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Run_Next_Day_Button.Foreground = Brushes.White;
        }
        private void Quit_Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void MouseEnter_QuitButton(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Quit_Button.Foreground = Brushes.Black;
        }
        private void MouseLeave_Quit_Button(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Quit_Button.Foreground = Brushes.White;
        }
        private void Reset_Button_Click(object sender, RoutedEventArgs e)
        {
            ClearFile(@"File\AlphaHarbor.txt");
            ClearFile(@"File\AlphaSharedSpot.txt");
            ClearFile(@"File\BetaHarbor.txt");
            ClearFile(@"File\BetaSharedSpot.txt");
            ClearFile(@"File\BoatsThatDidNotFit.txt");

            Info_ListBox.Items.Clear();
            Alpha_ListBox.Items.Clear();
            Beta_ListBox.Items.Clear();

            Array.Clear(HarborAlpha, 0, HarborAlpha.Length);
            Array.Clear(HarborBeta, 0, HarborBeta.Length);
            Array.Clear(SharedSpotAlpha, 0, HarborBeta.Length);
            Array.Clear(SharedSpotBeta, 0, HarborBeta.Length);
        }
        private void MouseEnter_Reset_Button(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Reset_Button.Foreground = Brushes.Black;
        }
        private void Mouse_Leave_Reset_Button(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Reset_Button.Foreground = Brushes.White;
        }
        private void Credits_Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("matte1337", "2020 ITHS .NET");
        }
        private void MouseEnter_Credits_Button(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Credits_Button.Foreground = Brushes.Black;
        }
        private void MouseLeave_Credits_Button(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Credits_Button.Foreground = Brushes.White;
        }
        private void GotFocus_Boats_Per_Day_Textbox(object sender, RoutedEventArgs e)
        {
            Boats_Per_Day_TextBox.Background = Brushes.White;
            Boats_Per_Day_TextBox.Foreground = Brushes.Black;
            Boats_Per_Day_TextBox.Text = null;

        }
        private void LostFocus_Boats_Per_Day_TextBox(object sender, RoutedEventArgs e)
        {
            Boats_Per_Day_TextBox.Background = Brushes.Black;
            Boats_Per_Day_TextBox.Foreground = Brushes.White;
        }
    }
}
