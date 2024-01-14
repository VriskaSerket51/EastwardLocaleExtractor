using CommandLine;
using EastwardLocaleExtractor;

#if DEBUG
Test.DoTest();
#else
Parser.Default.ParseArguments<Options>(args)
    .WithParsed(OnParsed);
#endif

void OnParsed(Options o)
{
    if (o.Mode != 1 && o.Mode != 2)
    {
        Console.WriteLine("Mode 1: Extract locale to json; 2: Compile json to locale");
        return;
    }

    var files = Directory.GetFiles(o.InputDirectory);
    foreach (var file in files)
    {
        var fileName = Path.GetFileName(file);
        var output = Path.Combine(o.OutputDirectory, fileName);
        
        if (o.Mode == 1)
        {
            var extractor = Extractor.Create(file);
            extractor.ExtractTo(output);
        }
        else
        {
            var locale = Compiler.Compile(file);
            File.WriteAllText(output, locale);
        }
    }
}