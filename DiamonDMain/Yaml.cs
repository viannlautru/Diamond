using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
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
        public static List<Partie> DeserializeGame(string name)
        {
            string path = @"../../../../Server\Ressources\" + name + ".yaml";
            var parser = new Parser(new StringReader(File.ReadAllText(path)));
            var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance).Build();

            parser.Consume<StreamStart>();
            parser.Consume<DocumentStart>();
            parser.Consume<MappingStart>();
            List<Partie> conf = new();
            while (parser.TryConsume<Scalar>(out var key))
            {
                if (key.Value == "games")
                    conf = deserializer.Deserialize<List<Partie>>(parser);
                else
                    parser.SkipThisAndNestedEvents();

                
            }

            return conf;
        }

    }
}
