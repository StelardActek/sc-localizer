using System.CommandLine;
using System.CommandLine.Parsing;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Text;

namespace sc_localizer;

public static class Program
{
    private static readonly string[] VALID_SOURCE_EXT = new[] { ".ini", ".p4k" };
    private const string LOCALIZATION_PATH = @"Data/Localization/{lang}/global.ini";
    private const string LOCALIZATION_LANG_DEFAULT = "english";

    public static int Main(string[] args)
    {
        Option<FileInfo> baseOption = new("--base", "-b")
        {
            Description = "Path to your base localization file. Can be global.ini or Data.p4k.",
            Required = true
        };
        baseOption.Validators.Add(result =>
        {
            var finfo = result.GetValue(baseOption);
            if (!VALID_SOURCE_EXT.Contains(finfo?.Extension, StringComparer.InvariantCultureIgnoreCase))
            {
                result.AddError("Base file must be either an INI or a P4K file.");
            }
            if (finfo?.Exists != true)
            {
                result.AddError("Base file must exist.");
            }
        });

        Option<string?> langOption = new("--language", "--lang", "-l")
        {
            Description = "When combined with a Data.p4k base file, determines what language to extract. Default: english"
        };
        langOption.DefaultValueFactory = (result) => LOCALIZATION_LANG_DEFAULT;

        Option<FileInfo[]> mergeOption = new("--merge", "-m") {
            Description = "Files to merge. Must be .ini files."
        };
        mergeOption.Validators.Add(result =>
        {
            var finfos = result.GetValue(mergeOption);
            if (finfos == null || finfos.Length == 0)
                return;
            foreach (var finfo in finfos)
            {
                if (!".ini".Equals(finfo?.Extension, StringComparison.InvariantCultureIgnoreCase))
                {
                    result.AddError("Merge files must be .ini files.");
                    break;
                }

                if (finfo?.Exists != true)
                {
                    result.AddError("All merge files must exist.");
                    break;
                }
            }
        });

        Option<FileInfo> outputOption = new("--output", "-o") { 
            Description = "Path to write the output. Default: ./global.ini"
        };
        outputOption.Validators.Add(result =>
        {
            var finfo = result.GetValue(outputOption);
            if (finfo?.Directory?.Exists != true)
            {
                result.AddError("Output file directory must exist.");
            }
        });

        RootCommand rootCommand = new("Compile a Star Citizen localization file from multiple source files.")
        {
            baseOption,
            mergeOption,
            langOption,
            outputOption
        };

        var parseResult = rootCommand.Parse(args);

        if (parseResult.Errors.Count > 0)
        {
            foreach (var error in parseResult.Errors)
            {
                Console.Error.WriteLine(error.Message);
            }

            return -1;
        }

        parseResult.Invoke();

        var baseInfo = parseResult.GetValue(baseOption);
        var mergeInfos = parseResult.GetValue(mergeOption) ?? [];
        var outputInfo = parseResult.GetValue(outputOption);
        var lang = parseResult.GetValue(langOption);
        Debug.Assert(baseInfo != null);
        Debug.Assert(lang != null);

        Stream? stream;
        var ini = new Dictionary<string, string>();
        
        if (baseInfo.Extension.Equals(".p4k", StringComparison.InvariantCultureIgnoreCase))
        {
            var localizationPath = LOCALIZATION_PATH.Replace("{lang}", lang);

            var p4KSource = new P4kSource(baseInfo);

            stream = p4KSource.GetDataStream(localizationPath);

            if (stream == null)
            {
                Console.Error.WriteLine($"Error: Could not find {localizationPath} in {baseInfo.Name}");
                return -1;
            }
            else
            {
                Console.Error.WriteLine($"Using base localization file {localizationPath} from {baseInfo.FullName}");
            }
        }
        else
        {
            stream = baseInfo.OpenRead();
            Console.Error.WriteLine($"Using base localisation file {baseInfo.FullName}");
        }

        using (stream)
        {
            PopulateIni(ini, stream, reportAdd: false);
        }

        foreach (var mergeInfo in mergeInfos)
        {
            Console.Error.WriteLine($"Merging changes from {mergeInfo.FullName}");
            using var mergeStream = mergeInfo.OpenRead();
            PopulateIni(ini, mergeStream);
        }

        if (outputInfo != null)
        {
            Console.Error.WriteLine($"Writing output file {outputInfo.FullName}");
            using var outputStream = outputInfo.OpenWrite();
            using var outputStreamWriter = new StreamWriter(outputStream, new UTF8Encoding(true));
            foreach (var kvp in ini)
            {
                outputStreamWriter.WriteLine($"{kvp.Key}={kvp.Value}");
            }
        }
        
        return 0;
    }

    private static void PopulateIni(Dictionary<string, string> ini, Stream source, bool reportAdd = true)
    {
        using var sr = new StreamReader(source);
        while (!sr.EndOfStream)
        {
            var line = sr.ReadLine();
            if (line == null)
                continue;

            var eqIdx = line?.IndexOf('=');

            if (eqIdx is null or -1)
            {
                Console.Error.WriteLine($"Skipping invalid line: {line}");
                continue;
            }

            var (key, value) = (line.Substring(0, eqIdx.Value), line.Substring(eqIdx.Value + 1));

            if (ini.ContainsKey(key))
            {
                Console.Error.WriteLine($"[Replace]: {key}={value}");
                ini[key] = value;
            }
            else
            {
                if (reportAdd)
                    Console.Error.WriteLine($"[Add]: {key}={value}");
                ini.Add(key, value);
            }
        }
    }
}