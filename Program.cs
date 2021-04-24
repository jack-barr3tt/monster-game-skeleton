using System;
using System.Xml;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace CSPreASSkelton

{

    class Program

    {

        public const int N_S_DISTANCE = 5;

        public const int W_E_DISTANCE = 7;

        public const int NO_OF_TRAPS = 2;

        public class CellReference { public int NoOfCellsSouth; public int NoOfCellsEast; }

        static void Main(string[] args)

        {

            char[,] Cavern = new char[N_S_DISTANCE, W_E_DISTANCE];

            int Choice = 0;

            int Score = 0;

            bool MonsterAwake = false;

            CellReference MonsterPosition = new CellReference();

            CellReference PlayerPosition = new CellReference();

            CellReference FlaskPosition = new CellReference();

            List<CellReference> TrapPositions = new List<CellReference>();

 

            while (Choice != 5)

            {

                DisplayMenu();

                Choice = GetMainMenuChoice();

                switch (Choice)

                {

                    case 1:

                        SetUpGame(Cavern, ref MonsterPosition, ref PlayerPosition, ref FlaskPosition, ref TrapPositions, ref Score, ref MonsterAwake);

                        PlayGame(Cavern, ref MonsterPosition, ref PlayerPosition, ref FlaskPosition, ref TrapPositions, ref Score, ref MonsterAwake);

                        break;

                    case 2: SetUpTrainingGame(Cavern, ref MonsterPosition, ref PlayerPosition, ref FlaskPosition, ref TrapPositions, ref Score, ref MonsterAwake);

                        PlayGame(Cavern, ref MonsterPosition, ref PlayerPosition, ref FlaskPosition, ref TrapPositions, ref Score, ref MonsterAwake);

                        break;
                    case 3:
                        SaveGame(Cavern, MonsterPosition, PlayerPosition, FlaskPosition, TrapPositions, Score, MonsterAwake);
                        break;
                    case 4:
                        LoadGame(ref Cavern, ref MonsterPosition, ref PlayerPosition, ref FlaskPosition, ref TrapPositions, ref Score, ref MonsterAwake);
                        PlayGame(Cavern, ref MonsterPosition, ref PlayerPosition, ref FlaskPosition, ref TrapPositions, ref Score, ref MonsterAwake);
                        break;
                    case 5:
                        break;
                    default:
                        System.Console.WriteLine("Invalid input, please re-enter");
                        break;

               }

            }

        }

        public static void AddItem(ref XmlTextWriter writer, string type, string[] names, string[] values){
            writer.WriteStartElement(type);
            for(int i = 0; i < names.Length; i++){
                writer.WriteStartElement(names[i]);
                writer.WriteString(values[i]);
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
        public static void SaveGame(char[,] Cavern, CellReference MonsterPosition, CellReference PlayerPosition, CellReference FlaskPosition, List<CellReference> TrapPositions, int Score, bool MonsterAwake){
            XmlTextWriter writer = new XmlTextWriter("savegame.xml", System.Text.Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            
            writer.WriteStartDocument(true);
            writer.WriteStartElement("Game");

            List<CellReference> GameObjects = new List<CellReference>();
            GameObjects.AddRange(new CellReference[3]{PlayerPosition, MonsterPosition, FlaskPosition});
            GameObjects.AddRange(TrapPositions);

            writer.WriteStartElement("Items");
            for(int i = 0; i < GameObjects.Count; i++){
                string type = Cavern[GameObjects[i].NoOfCellsSouth, GameObjects[i].NoOfCellsEast]+"";
                string south = GameObjects[i].NoOfCellsSouth+"";
                string east = GameObjects[i].NoOfCellsEast+"";
                
                AddItem(ref writer, "Item", new string[3]{"Type","NoOfCellsSouth","NoOfCellsEast"}, new string[3]{type, south, east});
            }
            writer.WriteEndElement();

            writer.WriteStartElement("Properties"); 
            AddItem(ref writer, "Property", new string[2]{"Name","Value"}, new string[2]{"MonsterAwake",""+MonsterAwake});
            AddItem(ref writer, "Property", new string[2]{"Name","Value"}, new string[2]{"Score",""+Score});
            writer.WriteEndElement();

            writer.WriteEndDocument();
            writer.Close();

            System.Console.WriteLine("Your progress has been saved. Press enter to return to menu.");
            System.Console.ReadLine();
        }

        public static void LoadGame(ref char[,] Cavern, ref CellReference MonsterPosition, ref CellReference PlayerPosition, ref CellReference FlaskPosition, ref List<CellReference> TrapPositions, ref int Score, ref bool MonsterAwake){
            TrapPositions = new List<CellReference>();
            ResetCavern(ref Cavern);
            
            XmlDocument xmldoc = new XmlDocument();
            FileStream fs = new FileStream("savegame.xml", FileMode.Open, FileAccess.Read);
            xmldoc.Load(fs);

            XmlNode items = xmldoc.GetElementsByTagName("Items")[0];          

            for(int i = 0; i < items.ChildNodes.Count; i++){
                XmlNode item = items.ChildNodes[i];

                CellReference itemRef = new CellReference();

                char name = '\u0000';
                
                for(int j = 0; j < item.ChildNodes.Count; j++){
                    XmlNode property = item.ChildNodes[j];

                    try{
                        typeof(CellReference).GetField(property.Name).SetValue(itemRef, Convert.ToInt32(property.InnerText));
                    }catch{
                        name = Convert.ToChar(property.InnerText);
                    }
                }    

                if(name == '*'){
                    PlayerPosition = itemRef;
                }else if(name == 'M'){
                    MonsterPosition = itemRef;
                }else if(name == 'F'){
                    FlaskPosition = itemRef;
                }else{
                    TrapPositions.Add(itemRef);
                }

                Cavern[itemRef.NoOfCellsSouth, itemRef.NoOfCellsEast] = name;
            }

            XmlNode props = xmldoc.GetElementsByTagName("Properties")[0];
            for(int i = 0; i < props.ChildNodes.Count; i++){
                XmlNode prop = props.ChildNodes[i];
                if(prop.ChildNodes[0].InnerText == "MonsterAwake"){
                    MonsterAwake = Convert.ToBoolean(prop.ChildNodes[1].InnerText);
                }else{
                    Score = Convert.ToInt32(prop.ChildNodes[1].InnerText);
                }
            }

            System.Console.WriteLine("Your saved game has been restored.\n");
            Console.ReadLine();
        }

        public static void DisplayMenu()

        {

            Console.WriteLine("MAIN MENU");

            Console.WriteLine();

            Console.WriteLine("1. Start new game");

            Console.WriteLine("2. Play training game");

            Console.WriteLine("3. Save Game data");
            
            Console.WriteLine("4. Load Game data");

            Console.WriteLine("5. Quit");

            Console.WriteLine();

            Console.WriteLine("Please enter your choice: ");

        }

        public static int GetMainMenuChoice()

        {

            int Choice = 6; 
            
            int.TryParse(Console.ReadLine(), out Choice);

            Console.WriteLine();

            return Choice;

        }

        public static void ResetCavern(ref char[,] Cavern)

        {

            int Count1;

            int Count2;

            for (Count1 = 0; Count1 < N_S_DISTANCE; Count1++)

            {

                for (Count2 = 0; Count2 < W_E_DISTANCE; Count2++)

                {

                    Cavern[Count1, Count2] = ' ';

                }

            }

        }

        public static CellReference GetNewRandomPosition() {

            CellReference Position = new CellReference();

            Random rnd = new Random();

            Position.NoOfCellsSouth = rnd.Next(0, N_S_DISTANCE);

            Position.NoOfCellsEast = rnd.Next(0, W_E_DISTANCE);

            return Position;

        }

        public static CellReference SetPositionOfItem(ref char[,] Cavern, char Item){
            while(true){
                CellReference testedPosition = GetNewRandomPosition();
                if(Cavern[testedPosition.NoOfCellsSouth, testedPosition.NoOfCellsEast] == ' '){
                    Cavern[testedPosition.NoOfCellsSouth, testedPosition.NoOfCellsEast] = Item;
                    return testedPosition;
                }
            }
        }

        public static void SetUpGame(char[,] Cavern, ref CellReference MonsterPosition, ref CellReference PlayerPosition, ref CellReference FlaskPosition, ref List<CellReference> TrapPositions, ref int Score, ref bool MonsterAwake)

        {

            ResetCavern(ref Cavern);

            Score = 0;

            MonsterAwake = false;

            PlayerPosition = SetPositionOfItem(ref Cavern, '*');

            MonsterPosition = SetPositionOfItem(ref Cavern, 'M');

            FlaskPosition = SetPositionOfItem(ref Cavern, 'F');

            TrapPositions = new List<CellReference>();
            for(int i = 0; i < NO_OF_TRAPS; i++){
                TrapPositions.Add(SetPositionOfItem(ref Cavern, 'T'));
            }

        }

        public static void SetUpTrainingGame(char[,] Cavern, ref CellReference MonsterPosition, ref CellReference PlayerPosition, ref CellReference FlaskPosition, ref List<CellReference> TrapPositions, ref int Score, ref bool MonsterAwake) {

            ResetCavern(ref Cavern);

            Score = 0;

            MonsterAwake = false;

            PlayerPosition.NoOfCellsSouth = 2;

            PlayerPosition.NoOfCellsEast = 4;

            Cavern[PlayerPosition.NoOfCellsSouth, PlayerPosition.NoOfCellsEast] = '*';

            MonsterPosition.NoOfCellsSouth = 0;

            MonsterPosition.NoOfCellsEast = 3;

            Cavern[MonsterPosition.NoOfCellsSouth, MonsterPosition.NoOfCellsEast] = 'M';

            FlaskPosition = GetNewRandomPosition();

            Cavern[FlaskPosition.NoOfCellsSouth, FlaskPosition.NoOfCellsEast] = 'F';

            TrapPositions = new List<CellReference>();
            for(int i = 0; i < NO_OF_TRAPS; i++){
                TrapPositions.Add(SetPositionOfItem(ref Cavern, 'T'));
            }

        }

        public static void DisplayCavern(char[,] Cavern, bool MonsterAwake)

        {

            int Count1;

            int Count2;

            for (Count1 = 0; Count1 < N_S_DISTANCE; Count1++)

            {

                Console.WriteLine(" -------------");

                for (Count2 = 0; Count2 < W_E_DISTANCE; Count2++) {

                    if ((new char[4]{' ','*','F','T'}).Contains(Cavern[Count1, Count2])) {

                        Console.Write("|" + Cavern[Count1, Count2]);

                    }else if(Cavern[Count1, Count2] == 'M' && MonsterAwake){
                        Console.Write("|" + Cavern[Count1, Count2]);
                    } else {

                        Console.Write("| ");

                    }

                }

                Console.WriteLine("|");

            }

            Console.WriteLine(" -------------");

            Console.WriteLine();

        }

        public static void DisplayMoveOptions() {

            Console.WriteLine();

            Console.WriteLine("Enter N to move NORTH");

            Console.WriteLine("Enter E to move EAST");

            Console.WriteLine("Enter S to move SOUTH");

            Console.WriteLine("Enter W to move WEST");

            Console.WriteLine("Enter M to return to the Main Menu");

            Console.WriteLine();

        }

        public static char GetMove()

        {

            char Move = 'a';

            char.TryParse(Console.ReadLine(), out Move);

            Console.WriteLine();

            return Move;

        }

        public static void MakeMove(char[,] Cavern, char Direction, ref CellReference PlayerPosition) {

            Cavern[PlayerPosition.NoOfCellsSouth, PlayerPosition.NoOfCellsEast] = ' ';

            Direction = char.ToLower(Direction);

            switch (Direction)

            {

                case 'n': PlayerPosition.NoOfCellsSouth = PlayerPosition.NoOfCellsSouth - 1;

                    break;

                case 's': PlayerPosition.NoOfCellsSouth = PlayerPosition.NoOfCellsSouth + 1;

                    break;

                case 'w': PlayerPosition.NoOfCellsEast = PlayerPosition.NoOfCellsEast - 1;

                    break;

                case 'e': PlayerPosition.NoOfCellsEast = PlayerPosition.NoOfCellsEast + 1;

                    break;

            }

            Cavern[PlayerPosition.NoOfCellsSouth, PlayerPosition.NoOfCellsEast] = '*';

        }

        public static Boolean CheckValidMove(CellReference PlayerPosition, char Direction)

        {

            Boolean ValidMove;

            Direction = char.ToLower(Direction);

            ValidMove = true;

            if (!(Direction == 'n' || Direction == 's' || Direction == 'w' || Direction == 'e' || Direction == 'm')) {

                ValidMove = false;

            }

            else if ((Direction == 'n' && PlayerPosition.NoOfCellsSouth == 0) || (Direction == 's' && PlayerPosition.NoOfCellsSouth == N_S_DISTANCE - 1)) {

                ValidMove = false;

            }

            else if ((Direction == 'w' && PlayerPosition.NoOfCellsEast == 0) || (Direction == 'e' && PlayerPosition.NoOfCellsEast == W_E_DISTANCE - 1)) {

                ValidMove = false;

            }

            return ValidMove;

        }

        public static Boolean CheckIfSameCell(CellReference FirstCellPosition, CellReference SecondCellPosition) {

            Boolean InSameCell = false;

            if (FirstCellPosition.NoOfCellsSouth == SecondCellPosition.NoOfCellsSouth && FirstCellPosition.NoOfCellsEast == SecondCellPosition.NoOfCellsEast) {

                InSameCell = true;

            }

            return InSameCell; 
        }

        public static CellReference CopyPosition(CellReference Position){
            CellReference Target = new CellReference();
            Target.NoOfCellsSouth = Position.NoOfCellsSouth;
            Target.NoOfCellsEast = Position.NoOfCellsEast;
            return Target;
        }

        public static void MakeMonsterMove(ref char[,] Cavern, ref CellReference MonsterPosition, CellReference PlayerPosition, ref CellReference FlaskPosition)

        {
            CellReference OriginalMonsterPosition = CopyPosition(MonsterPosition);

            Cavern[MonsterPosition.NoOfCellsSouth, MonsterPosition.NoOfCellsEast] = ' ';

            int XDiff = PlayerPosition.NoOfCellsEast - MonsterPosition.NoOfCellsEast;
            int YDiff = PlayerPosition.NoOfCellsSouth - MonsterPosition.NoOfCellsSouth;

            int XMove = XDiff / (XDiff == 0 ? 1 : Math.Abs(XDiff));
            int YMove = YDiff / (YDiff == 0 ? 1 : Math.Abs(YDiff));

            if(XMove != 0 && YMove != 0){
                MonsterPosition.NoOfCellsSouth += YMove;
            }else{
                MonsterPosition.NoOfCellsEast += XMove;
                MonsterPosition.NoOfCellsSouth += YMove;
            }

            if(CheckIfSameCell(MonsterPosition, FlaskPosition)){
                Cavern[OriginalMonsterPosition.NoOfCellsSouth, OriginalMonsterPosition.NoOfCellsEast] = 'F';
                FlaskPosition = CopyPosition(OriginalMonsterPosition);
            }

            Cavern[MonsterPosition.NoOfCellsSouth, MonsterPosition.NoOfCellsEast] = 'M';

        }

        public static void DisplayLostGameMessage()

        {

            Console.WriteLine("ARGHHHHHH! The monster has eaten you. GAME OVER.");

            Console.WriteLine("Maybe you will have better luck the next time you play MONSTER!");

            Console.WriteLine();

        }

        public static void DisplayWonGameMessage(bool GotFlask){
            System.Console.WriteLine("You win! " + (GotFlask ? "You grabbed the flask before the monster grabbed you!" : "You have outrun the monster and survived the cavern!"));
        }

        public static void PlayGame(char[,] Cavern, ref CellReference MonsterPosition, ref CellReference PlayerPosition, ref CellReference FlaskPosition, ref List<CellReference> TrapPositions, ref int Score, ref bool MonsterAwake)

        {

            Boolean Eaten = false;

            Boolean ValidMove = false;

            int Count = 0;

            char MoveDirection = ' ';

            DisplayCavern(Cavern, MonsterAwake);

            while (true)

            {

                ValidMove = false;

                while (true)

                {

                    DisplayMoveOptions();

                    MoveDirection = GetMove();

                    ValidMove = CheckValidMove(PlayerPosition, MoveDirection);

                    if(ValidMove){
                        break;
                    }else{
                        System.Console.WriteLine("Invalid input, please re-enter");
                    }

                }

                if (char.ToLower(MoveDirection) == 'm'){
                    break;
                }else{

                    MakeMove(Cavern, MoveDirection, ref PlayerPosition);

                    DisplayCavern(Cavern, MonsterAwake);

                    Eaten = CheckIfSameCell(MonsterPosition, PlayerPosition);

                    for(int i = 0; i < TrapPositions.Count; i++){
                        if(CheckIfSameCell(PlayerPosition, TrapPositions[i])){
                            System.Console.WriteLine("You have stepped on a trap and awoken the MONSTER!");
                            MonsterAwake = true;
                        }
                    }

                    if(CheckIfSameCell(PlayerPosition, FlaskPosition)){
                        DisplayWonGameMessage(true);
                        break;
                    }else if (!Eaten) { 
                        DisplayCavern(Cavern, MonsterAwake);
                        if(MonsterAwake == true){
                            Score += 1;
                        }
                    }
                }                

                if (!Eaten)

                {

                    Count = 0;

                    while (Count < 2 && !Eaten && MonsterAwake)

                    {

                        MakeMonsterMove(ref Cavern, ref MonsterPosition, PlayerPosition, ref FlaskPosition);

                        Eaten = CheckIfSameCell(MonsterPosition, PlayerPosition);

                        Console.WriteLine();

                        Console.WriteLine("Press Enter key to continue");

                        Console.ReadLine();

                        DisplayCavern(Cavern, MonsterAwake);

                        Count = Count + 1;

                    }

                }

                if(Eaten) { 
                    DisplayLostGameMessage();
                    break;
                }

                if(Score > 9){
                    DisplayWonGameMessage(false);
                    break;
                }
            }

        }

    }

}