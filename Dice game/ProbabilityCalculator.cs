class ProbabilityCalculator
{
    private List<Dice> DiceList;

    public ProbabilityCalculator(List<Dice> diceList)
    {
        DiceList = diceList;
    }

    // Probability dice1 wins over dice2
    public double ProbabilityDice1BeatsDice2(Dice dice1, Dice dice2)
    {
        int wins = 0;
        int total = dice1.Faces.Count * dice2.Faces.Count;
        foreach (var face1 in dice1.Faces)
        {
            foreach (var face2 in dice2.Faces)
            {
                if (face1 > face2)
                    wins++;
            }
        }
        return (double)wins / total;
    }
}