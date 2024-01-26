using XorHashDemo;

if (args.Length == 1)
{
    //Hash all files in the given directory
    Console.WriteLine("Final hash: {0}", HashTools.ToHex(HashTools.HashDir(args[0])));
}
else if (args.Length == 2 && args.Any(m => m.Equals("/gen", StringComparison.InvariantCultureIgnoreCase)))
{
    //Create 300 files, then hash them
    var dir = args.First(m => !m.Equals("/gen", StringComparison.InvariantCultureIgnoreCase));
    byte[] data = new byte[48];
    for (var i = 0; i < 300; i++)
    {
        Random.Shared.NextBytes(data);
        File.WriteAllText(Path.Combine(dir, $"{i:X4}.txt"), Convert.ToBase64String(data));
    }

    Console.WriteLine("Final hash: {0}", HashTools.ToHex(HashTools.HashDir(dir)));
}
else
{
    Console.WriteLine("XorHashDemo <dir> [/gen]");
}