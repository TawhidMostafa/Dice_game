class ProbabilityTable
{
    private List<Dice> DiceList;
    private ProbabilityCalculator Calculator;

    public ProbabilityTable(List<Dice> diceList, ProbabilityCalculator calculator)
    {
        DiceList = diceList;
        Calculator = calculator;
    }

    public void PrintTable()
    {
        Console.WriteLine("\nProbability table of dice winning over each other:");
        Console.Write("Dice#".PadRight(8));
        foreach (var d in DiceList)
        {
            Console.Write($"[{string.Join(",", d.Faces)}]".PadRight(15));
        }
        Console.WriteLine();

        for (int i = 0; i < DiceList.Count; i++)
        {
            Console.Write($"Dice {i}".PadRight(8));
            for (int j = 0; j < DiceList.Count; j++)
            {
                if (i == j)
                {
                    Console.Write("---".PadRight(15));
                }
                else
                {
                    double p = Calculator.ProbabilityDice1BeatsDice2(DiceList[i], DiceList[j]);
                    Console.Write($"{p,5:P1}".PadRight(15));
                }
            }
            Console.WriteLine();
        }
        Console.WriteLine();
    }
}