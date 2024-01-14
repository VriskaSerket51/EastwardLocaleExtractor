using CommandLine;

namespace EastwardLocaleExtractor;

public class Options
{
    [Option('i', "input_dir", Required = true, HelpText = "Set Input Directory.")]
    public required string InputDirectory { get; set; }

    [Option('o', "output_dir", Required = true, HelpText = "Set Output Directory.")]
    public required string OutputDirectory { get; set; }
    
    [Option('m', "mode", Required = true, HelpText = "Set Program mode. 1: Extract locale to json; 2: Compile json to locale")]
    public required int Mode { get; set; }
}