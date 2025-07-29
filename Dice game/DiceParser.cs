class DiceParser
{
    public bool TryParseDiceArguments(string[] args, out List<Dice> diceList, out string error)
    {
        diceList = new List<Dice>();
        error = null;
        if (args == null || args.Length < 3)
        {
            error = "At least 3 dice must be provided.";
            return false;
        }

        foreach (var arg in args)
        {
            var parts = arg.Split(',');
            if (parts.Length != 6)
            {
                error = $"Each dice must have exactly 6 faces. Invalid dice: {arg}";
                return false;
            }

            var faces = new List<int>();
            foreach (var p in parts)
            {
                if (!int.TryParse(p.Trim(), out int val))
                {
                    error = $"Non-integer value detected in dice: {arg}";
                    return false;
                }
                faces.Add(val);
            }
            diceList.Add(new Dice(faces));
        }
        return true;
    }
}