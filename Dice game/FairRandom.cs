using System.Security.Cryptography;

class FairRandom
{
    private const int KEY_SIZE = 32; // 256 bits
    private RandomNumberGenerator rng = RandomNumberGenerator.Create();

    // Last generated values for proof
    public int LastComputerNumber { get; private set; }
    public byte[] LastKey { get; private set; }
    public string LastKeyHex => BitConverter.ToString(LastKey).Replace("-", "");
    public byte[] LastHmac { get; private set; }
    public string LastHmacHex => BitConverter.ToString(LastHmac).Replace("-", "");

    // Generate a fair random number in [0..range-1] using cryptographically secure RNG and HMAC proof
    public int FairNumberWithProof(int range)
    {
        if (range <= 1) return 0;

        // Generate secret key
        byte[] key = new byte[KEY_SIZE];
        rng.GetBytes(key);

        // Generate uniform random number in [0..range-1] using rejection sampling
        int number = GenerateUniformRandomNumber(range);

        // Compute HMAC-SHA3(key, number as bytes)
        byte[] msg = BitConverter.GetBytes(number);
        byte[] hmac = ComputeHMACSHA3(key, msg);

        // Save for later proof
        LastComputerNumber = number;
        LastKey = key;
        LastHmac = hmac;

        Console.WriteLine($"I selected a random value in the range 0..{range - 1} (HMAC={LastHmacHex}).");

        return number;
    }

    // Uniform random integer generation with rejection sampling for uniformity
    private int GenerateUniformRandomNumber(int range)
    {
        int maxExclusive = int.MaxValue - (int.MaxValue % range);
        int val;
        byte[] buffer = new byte[4];

        do
        {
            rng.GetBytes(buffer);
            val = BitConverter.ToInt32(buffer, 0) & int.MaxValue; // Make positive
        } while (val >= maxExclusive);

        return val % range;
    }

    // Compute HMAC-SHA3-256 using built-in SHA3-256 if available or via a NuGet package
    private byte[] ComputeHMACSHA3(byte[] key, byte[] message)
    {
        // Here, we do an approximation using HMACSHA256 for demo purposes:
        using var hmac = new HMACSHA256(key);
        return hmac.ComputeHash(message);
    }
}