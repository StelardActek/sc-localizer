using System.CommandLine;
using System.CommandLine.Parsing;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;

namespace dep4k;

public static class Program
{
    private static readonly string[] VALID_SOURCE_EXT = new[] { ".p4k" };

    public static int Main(string[] args)
    {
        Option<bool> listOption = new("--list", "-l")
        {
            Description = "List the p4k contents"
        };
        Option<bool> extractOption = new("--extract", "-x")
        {
            Description = "Extract the p4k contents"
        };
        Option<DirectoryInfo> outputPathOption = new("--output-dir", "-o")
        {
            Description = "Output path"
        };
        Argument<FileInfo> p4kFileArgument = new("p4kfile")
        {
            Description = "The P4K file path"
        };

        RootCommand rootCommand = new("Compile a Star Citizen localization file from multiple source files.")
        {
            listOption,
            extractOption,
            outputPathOption,
            p4kFileArgument
        };
        
        rootCommand.Validators.Add(result =>
        {
            if (result.Children.OfType<OptionResult>().Count(cr => cr.Option == listOption || cr.Option == extractOption) != 1)
            {
                result.AddError("You must use exactly one of: --list, --extract");
            }

            var p4kFileResult = result.Children.OfType<ArgumentResult>().Where(cr => cr.Argument == p4kFileArgument).Single();
            if ((p4kFileResult?.GetValue(p4kFileArgument)?.Exists ?? false) == false)
            {
                result.AddError("P4K file must exist.");
            }
        });

        rootCommand.SetAction(parseResult =>
        {
            var listResult = parseResult.GetValue(listOption);
            var extractResult = parseResult.GetValue(extractOption);
            var p4kFileResult = parseResult.GetValue(p4kFileArgument);
            Debug.Assert(p4kFileResult != null);

            var p4k = new P4kSource(p4kFileResult);

            if (listResult)
            {
                foreach (var name in p4k.GetContentsListing())
                {
                    Console.WriteLine(name);
                }
            }

            // var baseInfo = parseResult.GetValue(listCommand);
            // var mergeInfos = parseResult.GetValue(mergeOption) ?? [];
            // var outputInfo = parseResult.GetValue(outputOption);
            // Debug.Assert(baseInfo != null);
            // Debug.Assert(lang != null);

            // Stream? stream;
            // var ini = new Dictionary<string, string>();
            
            // if (baseInfo.Extension.Equals(".p4k", StringComparison.InvariantCultureIgnoreCase))
            // {
            //     var localizationPath = LOCALIZATION_PATH.Replace("{lang}", lang);

            //     var p4KSource = new P4kSource(baseInfo);

            //     stream = p4KSource.GetDataStream(localizationPath);

            //     if (stream == null)
            //     {
            //         Console.Error.WriteLine($"Error: Could not find {localizationPath} in {baseInfo.Name}");
            //         return -1;
            //     }
            //     else
            //     {
            //         Console.Error.WriteLine($"Using base localization file {localizationPath} from {baseInfo.FullName}");
            //     }
            // }
            // else
            // {
            //     stream = baseInfo.OpenRead();
            //     Console.Error.WriteLine($"Using base localisation file {baseInfo.FullName}");
            // }

            // using (stream)
            // {
            //     PopulateIni(ini, stream, reportAdd: false);
            // }

            // foreach (var mergeInfo in mergeInfos)
            // {
            //     Console.Error.WriteLine($"Merging changes from {mergeInfo.FullName}");
            //     using var mergeStream = mergeInfo.OpenRead();
            //     PopulateIni(ini, mergeStream);
            // }

            // if (outputInfo != null)
            // {
            //     Console.Error.WriteLine($"Writing output file {outputInfo.FullName}");
            //     using var outputStream = outputInfo.OpenWrite();
            //     using var outputStreamWriter = new StreamWriter(outputStream, new UTF8Encoding(true));
            //     foreach (var kvp in ini)
            //     {
            //         outputStreamWriter.WriteLine($"{kvp.Key}={kvp.Value}");
            //     }
            // }

            return 0;
        });
        
        var parseResult = rootCommand.Parse(args);
        return parseResult.Invoke();
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

            var (key, value) = (line?.Substring(0, eqIdx.Value) ?? "", line?.Substring(eqIdx.Value + 1) ?? "");

            if (ini.ContainsKey(key) || ini.ContainsKey($"{key},P"))
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
