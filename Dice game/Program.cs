using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        var parser = new DiceParser();
        if (!parser.TryParseDiceArguments(args, out List<Dice> diceList, out string error))
        {
            Console.WriteLine("\nInput error: " + error);
            Console.WriteLine("Usage: 2,2,4,4,9,9 6,8,1,1,8,6 7,5,3,7,5,3 type input ");
            Console.WriteLine("You must specify at least 3 dice, each with exactly 6 comma-separated integers.");
            return;
        }

        if (diceList.Count < 3)
        {
            Console.WriteLine("Error: At least 3 dice must be provided.");
            return;
        }

        var probabilityCalc = new ProbabilityCalculator(diceList);
        var probabilityTable = new ProbabilityTable(diceList, probabilityCalc);

        var fairRandom = new FairRandom();

        Console.WriteLine("Let's determine who makes the first move.");

        // Fair 0 or 1 with HMAC protocol
        int firstMove = fairRandom.FairNumberWithProof(2);

        Console.WriteLine($"You go first if you guess my number correctly.");
        Console.WriteLine("Try to guess my selection:");
        Console.WriteLine("0 - 0");
        Console.WriteLine("1 - 1");
        Console.WriteLine("X - exit");
        Console.WriteLine("? - help");
        while (true)
        {
            Console.Write("Your selection: ");
            var userInput = Console.ReadLine()?.Trim().ToLower();
            if (userInput == "x") return;
            if (userInput == "?")
            {
                probabilityTable.PrintTable();
                continue;
            }
            if (userInput == "0" || userInput == "1")
            {
                int userGuess = int.Parse(userInput);
                int computerNum = fairRandom.LastComputerNumber;
                string key = fairRandom.LastKeyHex;
                string hmac = fairRandom.LastHmacHex;
                Console.WriteLine($"My selection: {computerNum} (KEY={key})");
                if (userGuess == computerNum)
                {
                    Console.WriteLine("You guessed right! You make the first move.");
                    firstMove = 0; // User first
                }
                else
                {
                    Console.WriteLine("You guessed wrong. I make the first move.");
                    firstMove = 1; // Computer first
                }
                break;
            }
            Console.WriteLine("Invalid input. Enter 0 or 1, ? for help, or X to exit.");
        }

        // Player (0) and Computer (1) pick dice
        Dice userDice = null;
        Dice computerDice = null;

        if (firstMove == 0)
        {
            userDice = UserSelectDice(diceList, null);
            if (userDice == null) return; // Exit
            computerDice = ComputerSelectDice(diceList, userDice);
            Console.WriteLine($"I choose the dice [{string.Join(",", computerDice.Faces)}].");
        }
        else
        {
            computerDice = ComputerSelectDice(diceList, null);
            Console.WriteLine($"I choose the dice [{string.Join(",", computerDice.Faces)}].");
            userDice = UserSelectDice(diceList, computerDice);
            if (userDice == null) return; // Exit
        }

        // Rolls
        Console.WriteLine("It's time for my roll.");
        int computerRoll = RollDiceWithFairRandom(fairRandom, computerDice);

        Console.WriteLine("It's time for your roll.");
        int userRoll = RollDiceWithFairRandom(fairRandom, userDice);

        Console.WriteLine($"My roll result is {computerRoll}.");
        Console.WriteLine($"Your roll result is {userRoll}.");

        if (userRoll > computerRoll)
            Console.WriteLine("You win!");
        else if (userRoll < computerRoll)
            Console.WriteLine("I win!");
        else
            Console.WriteLine("It's a tie!");
    }

    // User selects dice from menu excluding excludedDice
    static Dice UserSelectDice(List<Dice> diceList, Dice excludedDice)
    {
        Console.WriteLine("Choose your dice:");
        while (true)
        {
            for (int i = 0; i < diceList.Count; i++)
            {
                if (excludedDice != null && diceList[i] == excludedDice) continue;
                Console.WriteLine($"{i} - {string.Join(",", diceList[i].Faces)}");
            }
            Console.WriteLine("X - exit");
            Console.WriteLine("? - help");

            Console.Write("Your selection: ");
            var input = Console.ReadLine()?.Trim().ToLower();
            if (input == "x") return null;
            if (input == "?")
            {
                var probabilityCalc = new ProbabilityCalculator(diceList);
                var table = new ProbabilityTable(diceList, probabilityCalc);
                table.PrintTable();
                continue;
            }
            if (int.TryParse(input, out int idx))
            {
                if (idx >= 0 && idx < diceList.Count)
                {
                    var selected = diceList[idx];
                    if (excludedDice != null && selected == excludedDice)
                    {
                        Console.WriteLine("You cannot select the same dice as the opponent.");
                        continue;
                    }
                    Console.WriteLine($"You choose the [{string.Join(",", selected.Faces)}] dice.");
                    return selected;
                }
            }
            Console.WriteLine("Invalid selection.");
        }
    }

    // Computer selects dice excluding user's dice (randomly)
    static Dice ComputerSelectDice(List<Dice> diceList, Dice excludedDice)
    {
        var candidates = diceList.Where(d => d != excludedDice).ToList();
        var rng = RandomNumberGenerator.Create();
        byte[] buf = new byte[4];
        rng.GetBytes(buf);
        int idx = BitConverter.ToInt32(buf, 0);
        idx = Math.Abs(idx) % candidates.Count;
        return candidates[idx];
    }

    // Roll dice with fair random number generation and HMAC protocol
    static int RollDiceWithFairRandom(FairRandom fairRandom, Dice dice)
    {
        int range = dice.Faces.Count;
        int rollValue = fairRandom.FairNumberWithProof(range);

        Console.WriteLine("Add your number modulo " + range + ".");
        for (int i = 0; i < range; i++)
            Console.WriteLine($"{i} - {i}");
        Console.WriteLine("X - exit");
        Console.WriteLine("? - help");

        int userAdd = -1;
        while (true)
        {
            Console.Write("Your selection: ");
            var input = Console.ReadLine()?.Trim().ToLower();
            if (input == "x") Environment.Exit(0);
            if (input == "?")
            {
                Console.WriteLine("Select a number from 0 to " + (range - 1));
                continue;
            }
            if (int.TryParse(input, out int val))
            {
                if (val >= 0 && val < range)
                {
                    userAdd = val;
                    break;
                }
            }
            Console.WriteLine("Invalid input.");
        }

        int result = (rollValue + userAdd) % range;
        Console.WriteLine($"My number is {rollValue} (KEY={fairRandom.LastKeyHex}).");
        Console.WriteLine($"The fair number generation result is {rollValue} + {userAdd} = {result} (mod {range}).");
        Console.WriteLine($"My roll result is {dice.Faces[result]}.");
        return dice.Faces[result];
    }
}