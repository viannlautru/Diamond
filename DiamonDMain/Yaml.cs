using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace DiamonDMain
{
    public class Yaml
    {   
        public static string Serialize(object o)
        {
            var stringBuilder = new StringBuilder();
            var serializer = new Serializer();
            serializer.Serialize(new IndentedTextWriter(new StringWriter(stringBuilder)), o);
            return stringBuilder.ToString();
        }

        public static string GetPath(string name)
        {
            string fileName = @"Ressources\" + name + ".yaml";
            string thePath = Path.GetFullPath(fileName);
            thePath = thePath.Replace(@"\bin\Debug\net6.0", "");
            return thePath;
        }
        public static Object DeserializeGame()
        {
            String path = @"../../../../Server\Ressources\Server.yaml";
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            Dictionary<String, Object> config = deserializer.Deserialize<Dictionary<String, Object>>(File.ReadAllText(path));
            var a = config.Values.ElementAt(2);
            return a;
        }

    }
}
