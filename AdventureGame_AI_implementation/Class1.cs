using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventureGame_AI_implementation
{
   

    public class Program
    {
        public static void Main()
        {
            bool playAgain;

            do
            {
                Console.Clear();

                var game = new AdventureGame();
                game.Start();

                Console.WriteLine();
                Console.Write("Play again? (Y/N): ");

                string input = Console.ReadLine()!.ToUpper();

                playAgain = input == "Y";

            } while (playAgain);

            Console.WriteLine("Thanks for playing!");
        }
    }
}
