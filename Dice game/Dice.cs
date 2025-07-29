class Dice
{
    public List<int> Faces { get; }
    public Dice(IEnumerable<int> faces)
    {
        Faces = faces.ToList();
    }
}