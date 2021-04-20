using System;

using System.IO;

namespace CSPreASSkelton

{

    class Program

    {

        public const int N_S_DISTANCE = 5;

        public const int W_E_DISTANCE = 7;

        public struct CellReference { public int NoOfCellsSouth; public int NoOfCellsEast; }

        static void Main(string[] args)

        {

            char[,] Cavern = new char[N_S_DISTANCE, W_E_DISTANCE];

            int Choice = 0;

            CellReference MonsterPosition = new CellReference();

            CellReference PlayerPosition = new CellReference();

 

            while (Choice != 3)

            {

                DisplayMenu();

                Choice = GetMainMenuChoice();

                switch (Choice)

                {

                    case 1:

                        SetUpGame(Cavern, ref MonsterPosition, ref PlayerPosition);

                        PlayGame(Cavern, ref MonsterPosition, ref PlayerPosition);

                        break;

                    case 2: SetUpTrainingGame(Cavern, ref MonsterPosition, ref PlayerPosition);

                        PlayGame(Cavern, ref MonsterPosition, ref PlayerPosition);

                        break;
                    case 3:
                        break;
                    default:
                        System.Console.WriteLine("Invalid input, please re-enter");
                        break;

               }

            }

        }

        public static void DisplayMenu()

        {

            Console.WriteLine("MAIN MENU");

            Console.WriteLine();

            Console.WriteLine("1. Start new game");

            Console.WriteLine("2. Play training game");

            Console.WriteLine("3. Quit");

            Console.WriteLine();

            Console.WriteLine("Please enter your choice: ");

        }

        public static int GetMainMenuChoice()

        {

            int Choice = 4; 
            
            int.TryParse(Console.ReadLine(), out Choice);

            Console.WriteLine();

            return Choice;

        }

        public static void ResetCavern(char[,] Cavern)

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

        public static void SetUpGame(char[,] Cavern, ref CellReference MonsterPosition, ref CellReference PlayerPosition)

        {

            ResetCavern(Cavern);

            PlayerPosition.NoOfCellsSouth = 0;

            PlayerPosition.NoOfCellsEast = 0;

            Cavern[PlayerPosition.NoOfCellsSouth, PlayerPosition.NoOfCellsEast] = '*';

            MonsterPosition = GetNewRandomPosition();

            Cavern[MonsterPosition.NoOfCellsSouth, MonsterPosition.NoOfCellsEast] = 'M';

        }

        public static void SetUpTrainingGame(char[,] Cavern, ref CellReference MonsterPosition, ref CellReference PlayerPosition) {

            ResetCavern(Cavern);

            PlayerPosition.NoOfCellsSouth = 2;

            PlayerPosition.NoOfCellsEast = 4;

            Cavern[PlayerPosition.NoOfCellsSouth, PlayerPosition.NoOfCellsEast] = '*';

            MonsterPosition.NoOfCellsSouth = 0;

            MonsterPosition.NoOfCellsEast = 3;

            Cavern[MonsterPosition.NoOfCellsSouth, MonsterPosition.NoOfCellsEast] = 'M';

        }

        public static void DisplayCavern(char[,] Cavern)

        {

            int Count1;

            int Count2;

            for (Count1 = 0; Count1 < N_S_DISTANCE; Count1++)

            {

                Console.WriteLine(" -------------");

                for (Count2 = 0; Count2 < W_E_DISTANCE; Count2++) {

                    if (Cavern[Count1, Count2] == ' ' || Cavern[Count1, Count2] == '*' || ((Cavern[Count1, Count2] == 'M'))) {

                        Console.Write("|" + Cavern[Count1, Count2]);

                    }

                    else {

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

            return ValidMove;

        }

        public static Boolean CheckIfSameCell(CellReference FirstCellPosition, CellReference SecondCellPosition) {

            Boolean InSameCell = false;

            if (FirstCellPosition.NoOfCellsSouth == SecondCellPosition.NoOfCellsSouth && FirstCellPosition.NoOfCellsEast == SecondCellPosition.NoOfCellsEast) {

                InSameCell = true;

            }

            return InSameCell; }

        public static void MakeMonsterMove(char[,] Cavern, ref CellReference MonsterPosition, CellReference PlayerPosition)

        {

            CellReference OriginalMonsterPosition = new CellReference();

            OriginalMonsterPosition.NoOfCellsSouth = MonsterPosition.NoOfCellsSouth;

            OriginalMonsterPosition.NoOfCellsEast = MonsterPosition.NoOfCellsEast;

            Cavern[MonsterPosition.NoOfCellsSouth, MonsterPosition.NoOfCellsEast] = ' ';

            if (MonsterPosition.NoOfCellsSouth < PlayerPosition.NoOfCellsSouth)

            {

                MonsterPosition.NoOfCellsSouth = MonsterPosition.NoOfCellsSouth + 1;

            }

            else if (MonsterPosition.NoOfCellsSouth > PlayerPosition.NoOfCellsSouth)

            {

                MonsterPosition.NoOfCellsSouth = MonsterPosition.NoOfCellsSouth - 1;

            }

 

            else if (MonsterPosition.NoOfCellsEast < PlayerPosition.NoOfCellsEast)

            {

                MonsterPosition.NoOfCellsEast = MonsterPosition.NoOfCellsEast + 1;

 

            }

 

            else if (MonsterPosition.NoOfCellsEast > PlayerPosition.NoOfCellsEast)

            {

                MonsterPosition.NoOfCellsEast = MonsterPosition.NoOfCellsEast - 1;

            }

 

            Cavern[MonsterPosition.NoOfCellsSouth, MonsterPosition.NoOfCellsEast] = 'M';

        }

        public static void DisplayLostGameMessage()

        {

            Console.WriteLine("ARGHHHHHH! The monster has eaten you. GAME OVER.");

            Console.WriteLine("Maybe you will have better luck the next time you play MONSTER!");

            Console.WriteLine();

        }

        public static void PlayGame(char[,] Cavern, ref CellReference MonsterPosition, ref CellReference PlayerPosition)

        {

            Boolean Eaten = false;

            Boolean ValidMove = false;

            int Count = 0;

            char MoveDirection = ' ';

            DisplayCavern(Cavern);

            while (!(Eaten || MoveDirection == 'M'))

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

                if (MoveDirection != 'M')

                {

                    MakeMove(Cavern, MoveDirection, ref PlayerPosition);

                    DisplayCavern(Cavern);

                    Eaten = CheckIfSameCell(MonsterPosition, PlayerPosition);

                    if (!Eaten) { DisplayCavern(Cavern);

                    }

                }

                if (!Eaten)

                {

                    Count = 0;

                    while (Count < 2 && !Eaten)

                    {

                        MakeMonsterMove(Cavern, ref MonsterPosition, PlayerPosition);

                        Eaten = CheckIfSameCell(MonsterPosition, PlayerPosition);

                        Console.WriteLine();

                        Console.WriteLine("Press Enter key to continue");

                        Console.ReadLine();

                        DisplayCavern(Cavern);

                        Count = Count + 1;

                    }

                }

                if (Eaten) { DisplayLostGameMessage();

                }

            }

        }

    }

}