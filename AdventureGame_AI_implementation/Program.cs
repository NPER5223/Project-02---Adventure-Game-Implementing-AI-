namespace AdventureGame_AI_implementation
{
    public class AdventureGame
    {
        public readonly string GO_NORTH = "W";
        public readonly string GO_SOUTH = "S";
        public readonly string GO_EAST = "D";
        public readonly string GO_WEST = "A";
        public readonly string GET_LAMP = "L";
        public readonly string GET_KEY = "K";
        public readonly string OPEN_CHEST = "O";
        public readonly string QUIT = "Q";

        private Adventurer adventurer;
        private Room[,] dungeon;
        private int aRow;
        private int aCol;
        private bool isChestOpen;
        private bool hasPlayerQuit;
        private bool hasPlayerWon;
        private bool isAdventureAlive;
        private string lastDirection;
        private int exitRow;
        private int exitCol;
        private int grueRow;
        private int grueCol;

        public AdventureGame() { }

        public void Start()
        {
            Init();
            ShowGameStartScreen();

            string input;

            do
            {
                Console.Clear();

                ShowGameStartScreen();

                ShowScene();

                do
                {
                    ShowInputOptions();
                    input = GetInput();
                }
                while (!IsValidInput(input));

                ProcessInput(input);

                UpdateGameState();

                if (!IsGameOver())
                {
                    Console.WriteLine("\nPress any key to continue...");
                    Console.ReadKey(true);
                }

            }
            while (!IsGameOver());

            ShowGameOverScreen();
        }

        private void Init()
        {
            adventurer = new Adventurer();
            LoadDungeon("Dungeon1.txt");

            isChestOpen = false;
            hasPlayerQuit = false;
            hasPlayerWon = false;
            isAdventureAlive = true;
            lastDirection = string.Empty;
        }

        private void LoadDungeon(string filename)
        {
            string[] lines = File.ReadAllLines(filename);

            int rows = 0;
            int cols = 0;

            List<string> roomData = new List<string>();
            List<(int, int)> litRooms = new List<(int, int)>();
            bool readingRooms = false;

            foreach (string raw in lines)
            {
                string line = raw.Trim();

                if (line == "") continue;

                if (line.StartsWith("ROWS="))
                {
                    rows = int.Parse(line.Substring(5));
                }
                else if (line.StartsWith("COLS="))
                {
                    cols = int.Parse(line.Substring(5));
                }
                else if (line.StartsWith("START="))
                {
                    string[] parts = line.Substring(6).Split(',');
                    aRow = int.Parse(parts[0]);
                    aCol = int.Parse(parts[1]);
                }
                else if (line.StartsWith("EXIT="))
                {
                    string[] parts = line.Substring(5).Split(',');
                    exitRow = int.Parse(parts[0]);
                    exitCol = int.Parse(parts[1]);
                }
                else if (line.StartsWith("GRUE="))
                {
                    string[] parts = line.Substring(5).Split(',');
                    grueRow = int.Parse(parts[0]);
                    grueCol = int.Parse(parts[1]);
                }
                else if (line.StartsWith("LIT="))
                {
                    string[] parts = line.Substring(4).Split(',');
                    litRooms.Add((int.Parse(parts[0]), int.Parse(parts[1])));
                }
                else if (line == "ROOMS")
                {
                    readingRooms = true;
                }
                else if (readingRooms)
                {
                    string[] codes = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    foreach (string code in codes)
                        roomData.Add(code);
                }
            }

            dungeon = new Room[rows, cols];

            int roomIndex = 0;
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    string code = roomData[roomIndex++];

                    Room room = new Room();
                    room.SetNorth(code[0] == '1');
                    room.SetEast(code[1] == '1');
                    room.SetSouth(code[2] == '1');
                    room.SetWest(code[3] == '1');
                    room.SetDescription($"Room ({r},{c})");

                    dungeon[r, c] = room;
                }
            }

            // Light up only the rooms defined in the dungeon file
            foreach (var (r, c) in litRooms)
                dungeon[r, c].SetLit(true);

            dungeon[aRow, aCol].SetLit(true);


            ParseSpecialRooms(lines);
        }

        private void ParseSpecialRooms(string[] lines)
        {
            foreach (string raw in lines)
            {
                string line = raw.Trim();

                if (line.StartsWith("LAMP="))
                {
                    string[] p = line.Substring(5).Split(',');
                    dungeon[int.Parse(p[0]), int.Parse(p[1])].SetLamp(true);
                }
                else if (line.StartsWith("KEY="))
                {
                    string[] p = line.Substring(4).Split(',');
                    dungeon[int.Parse(p[0]), int.Parse(p[1])].SetKey(true);
                }
                else if (line.StartsWith("CHEST="))
                {
                    string[] p = line.Substring(6).Split(',');
                    dungeon[int.Parse(p[0]), int.Parse(p[1])].SetChest(true);
                }
                else if (line.StartsWith("LIT="))
                {
                    string[] p = line.Substring(4).Split(',');
                    dungeon[int.Parse(p[0]), int.Parse(p[1])].SetLit(true);
                }
            }
        }

        private void ShowGameStartScreen()
        {
            Console.WriteLine("=================================");
            Console.WriteLine("   Welcome to Adventure Game!   ");
            Console.WriteLine("=================================");
            Console.WriteLine("Find the lamp, get the key,");
            Console.WriteLine("open the chest and reach the exit!");
            Console.WriteLine("Beware of the Grue in the dark!");
            Console.WriteLine("=================================\n");
        }

        private void ShowScene()
        {
            var r = dungeon[aRow, aCol];

            if (adventurer.HasLamp() || r.IsLit())
            {
                Console.WriteLine(r.GetDescription());

                // Show items in the room
                if (r.HasLamp()) Console.WriteLine("You see a lamp here!");
                if (r.HasKey()) Console.WriteLine("You see a key here!");
                if (r.HasChest()) Console.WriteLine("You see a chest here!");

                if (aRow == exitRow && aCol == exitCol)
                    Console.WriteLine("You see the dungeon exit!");
            }
            else
            {
                Console.WriteLine("This room is pitch black! Go back or the Grue will eat you!");
            }
        }

        private void ShowInputOptions()
        {
            Console.WriteLine();
            Console.WriteLine($"GO NORTH [{GO_NORTH}] | GO EAST [{GO_EAST}] | GET LAMP [{GET_LAMP}] | OPEN CHEST [{OPEN_CHEST}]");
            Console.WriteLine($"GO SOUTH [{GO_SOUTH}] | GO WEST [{GO_WEST}] | GET KEY  [{GET_KEY}] | QUIT       [{QUIT}]");
            Console.Write("> ");
        }

        private string GetInput()
        {
            return Console.ReadLine()!.ToUpper();
        }

        private bool IsValidInput(string input)
        {
            string[] validInputs = { GO_NORTH, GO_SOUTH, GO_EAST, GO_WEST, GET_LAMP, GET_KEY, OPEN_CHEST, QUIT };

            if (!validInputs.Contains(input))
            {
                Console.WriteLine("ERROR: Invalid input. Please try again.");
                return false;
            }

            return true;
        }

        private void ProcessInput(string input)
        {
            Room r = dungeon[aRow, aCol];

            bool isMovement = input == GO_NORTH || input == GO_SOUTH ||
                              input == GO_EAST || input == GO_WEST;

            // In a dark room you can only go back or get eaten
            if (isMovement && !adventurer.HasLamp() && !r.IsLit())
            {
                if (input == lastDirection)
                {
                    // Going back to the previous room — allowed
                    if (input == GO_NORTH) GoNorth(r);
                    else if (input == GO_SOUTH) GoSouth(r);
                    else if (input == GO_EAST) GoEast(r);
                    else if (input == GO_WEST) GoWest(r);
                }
                else
                {
                    // Going deeper into the dark — Grue gets you
                    Console.WriteLine("You venture deeper into the darkness...");
                    Console.WriteLine("The Grue eats you alive!");
                    isAdventureAlive = false;
                }
                return;
            }

            if (input == GO_NORTH) GoNorth(r);
            else if (input == GO_SOUTH) GoSouth(r);
            else if (input == GO_EAST) GoEast(r);
            else if (input == GO_WEST) GoWest(r);
            else if (input == GET_LAMP) GetLamp(r);
            else if (input == GET_KEY) GetKey(r);
            else if (input == OPEN_CHEST) OpenChest(r);
            else Quit();
        }

        private void UpdateGameState()
        {
            if (isChestOpen && aRow == exitRow && aCol == exitCol)
            {
                Console.WriteLine("You escaped the dungeon with the treasure!");
                hasPlayerWon = true;
                return;
            }

            if (isChestOpen)
                MoveGrue();

            if (aRow == grueRow && aCol == grueCol)
            {
                Console.WriteLine("The Grue caught you!");
                isAdventureAlive = false;
            }
        }

        private void MoveGrue()
        {
            if (grueRow < aRow && grueRow + 1 < dungeon.GetLength(0))
                grueRow++;
            else if (grueRow > aRow && grueRow - 1 >= 0)
                grueRow--;
            else if (grueCol < aCol && grueCol + 1 < dungeon.GetLength(1))
                grueCol++;
            else if (grueCol > aCol && grueCol - 1 >= 0)
                grueCol--;

            Console.WriteLine("You hear something moving nearby...");
        }

        private bool IsGameOver()
        {
            return hasPlayerWon || hasPlayerQuit || !isAdventureAlive;
        }

        private void ShowGameOverScreen()
        {
            Console.WriteLine("\n=================================");
            if (hasPlayerWon)
                Console.WriteLine("  YOU WIN! Congratulations!");
            else if (hasPlayerQuit)
                Console.WriteLine("  You quit the game. Goodbye!");
            else
                Console.WriteLine("  YOU DIED! Better luck next time!");
            Console.WriteLine("=================================");
        }

        private void GoNorth(Room r)
        {
            if (r.HasNorth() && aRow - 1 >= 0)
            {
                aRow -= 1;
                lastDirection = GO_SOUTH;
            }
            else
            {
                Console.WriteLine("You cannot go north!");
            }
        }

        private void GoSouth(Room r)
        {
            if (r.HasSouth() && aRow + 1 < dungeon.GetLength(0))
            {
                aRow += 1;
                lastDirection = GO_NORTH;
            }
            else
            {
                Console.WriteLine("You cannot go south!");
            }
        }

        private void GoEast(Room r)
        {
            if (r.HasEast() && aCol + 1 < dungeon.GetLength(1))
            {
                aCol += 1;
                lastDirection = GO_WEST;
            }
            else
            {
                Console.WriteLine("You cannot go east!");
            }
        }

        private void GoWest(Room r)
        {
            if (r.HasWest() && aCol - 1 >= 0)
            {
                aCol -= 1;
                lastDirection = GO_EAST;
            }
            else
            {
                Console.WriteLine("You cannot go west!");
            }
        }

        private void GetLamp(Room r)
        {
            if (r.HasLamp())
            {
                Console.WriteLine("You picked up the LAMP! The dungeon lights up around you!");
                adventurer.SetLamp(true);
                r.SetLamp(false);
            }
            else
            {
                Console.WriteLine("There is no lamp in this room.");
            }
        }

        private void GetKey(Room r)
        {
            if (r.HasKey())
            {
                Console.WriteLine("You picked up the KEY!");
                adventurer.SetKey(true);
                r.SetKey(false);
            }
            else
            {
                Console.WriteLine("There is no key in this room.");
            }
        }

        private void OpenChest(Room r)
        {
            if (r.HasChest())
            {
                if (adventurer.HasKey())
                {
                    Console.WriteLine("You opened the CHEST and grabbed the treasure!");
                    Console.WriteLine("The Grue has awakened and is hunting you!");
                    isChestOpen = true;
                }
                else
                {
                    Console.WriteLine("The chest is locked. You need the KEY!");
                }
            }
            else
            {
                Console.WriteLine("There is no chest in this room.");
            }
        }

        private void Quit()
        {
            Console.WriteLine("You quit the game!");
            hasPlayerQuit = true;
        }
    }
}