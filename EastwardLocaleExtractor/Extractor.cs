using System.Text;
using Newtonsoft.Json;
using NLua;

namespace EastwardLocaleExtractor;

public class Extractor : Dictionary<string, Dictionary<string, string>>
{
    private Extractor(int capacity) : base(capacity)
    {
    }

    public static Extractor Create(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException(path);
        }

        var locale = File.ReadAllText(path).Trim();

        if (!locale.StartsWith("return {") || !locale.EndsWith("}"))
        {
            throw new Exception("file is not locale");
        }

        using Lua lua = new Lua();
        lua.State.Encoding = Encoding.UTF8;
        var result = lua.DoString(locale)[0] as LuaTable;
        if (result == null)
        {
            throw new Exception("error during parsing locale");
        }

        var extractor = new Extractor(result.Keys.Count);

        foreach (string sqName in result.Keys)
        {
            if (result[(object)sqName] is not LuaTable hashMap)
            {
                continue;
            }

            var dialogs = new Dictionary<string, string>(hashMap.Keys.Count);
            foreach (string hash in hashMap.Keys)
            {
                string dialog = (string)hashMap[(object)hash];
                dialogs[hash] = dialog;
            }

            extractor[sqName] = dialogs;
        }

        return extractor;
    }

    public void ExtractTo(string path)
    {
        string? root = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(root))
        {
            Directory.CreateDirectory(root);
        }

        File.WriteAllText(path, ToString());
    }

    public override string ToString()
    {
        return JsonConvert.SerializeObject(this, Formatting.Indented);
    }
}