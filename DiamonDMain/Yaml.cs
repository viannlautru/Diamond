using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

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

      
    }
}
