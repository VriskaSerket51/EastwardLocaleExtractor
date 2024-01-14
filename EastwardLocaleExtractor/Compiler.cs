using System.Text;
using Newtonsoft.Json;

namespace EastwardLocaleExtractor;

public static class Compiler
{
    private static string AddQuote(string s)
    {
        return $"\"{s}\"";
    }

    private static string Escape(string s)
    {
        s = s.Replace("\n", "\\n");
        s = s.Replace("\t", "\\t");
        s = s.Replace("\"", "\\\"");
        s = s.Replace("\'", "\\\'");

        return s;
    }

    public static string Compile(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }

        var json = File.ReadAllText(path).Trim();

        var data = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(json);

        if (data == null)
        {
            throw new Exception("not a json file");
        }

        StringBuilder sb = new StringBuilder(data.Count * 2);
        sb.AppendLine("return {");
        foreach (var (sqName, dialogs) in data)
        {
            var key = $"[{AddQuote(sqName)}]" + "={";
            sb.AppendLine(key);
            foreach (var (hash, dialog) in dialogs)
            {
                sb.Append('\t');
                sb.Append($"[{AddQuote(hash)}]");
                sb.Append(" = ");
                sb.Append($"{AddQuote(Escape(dialog))}");
                sb.AppendLine(";");
            }

            sb.AppendLine("\t};");
        }

        sb.AppendLine("}");

        return sb.ToString();
    }
}