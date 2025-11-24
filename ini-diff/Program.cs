namespace ini_diff;

public static class Program
{
    public static int Main(string[] args)
    {
        if (args.Length != 2)
        {
            Console.WriteLine("Usage: ini_diff <base_file> <altered_file>");
            Console.WriteLine();
            Console.WriteLine("Keys found in altered_file that are new or different from base_file will be displayed.");
            return -1;
        }
        
        var (baseFile, alteredFile) = (new FileInfo(args[0]), new FileInfo(args[1]));

        if (!baseFile.Exists || !alteredFile.Exists)
        {
            Console.WriteLine("Either base_file or altered_file doesn't exist.");
        }

        var ini = new Dictionary<string, string>();
        using var baseStream = baseFile.OpenRead();
        using var baseStreamReader = new StreamReader(baseStream);
        while (!baseStreamReader.EndOfStream)
        {
            var line = baseStreamReader.ReadLine();
            if (line == null)
                continue;

            var eqIdx = line?.IndexOf('=');

            if (eqIdx is null or -1)
            {
                Console.Error.WriteLine($"Skipping invalid line: {line}");
                continue;
            }

            var (key, value) = (line.Substring(0, eqIdx.Value), line.Substring(eqIdx.Value + 1));

            ini[key] = value;
        }
        
        using var alteredStream = alteredFile.OpenRead();
        using var alteredStreamReader = new StreamReader(alteredStream);
        while (!alteredStreamReader.EndOfStream)
        {
            var line = alteredStreamReader.ReadLine();
            if (line == null)
                continue;

            var eqIdx = line?.IndexOf('=');

            if (eqIdx is null or -1)
            {
                Console.Error.WriteLine($"Skipping invalid line: {line}");
                continue;
            }

            var (key, value) = (line.Substring(0, eqIdx.Value), line.Substring(eqIdx.Value + 1));

            if (!ini.ContainsKey(key))
            {
                Console.Error.WriteLine($"New entry: {key}={value}");
                Console.WriteLine($"{key}={value}");
            }
            
            if (ini.ContainsKey(key) && !ini[key].Trim().Equals(value.Trim()))
            {
                Console.Error.WriteLine($"Altered entry: {key}={ini[key]}");
                Console.WriteLine($"{key}={value}");
            }
        }
        
        return 0;
    }
}