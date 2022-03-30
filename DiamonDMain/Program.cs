using System;
using System.Collections;
using System.IO;
using YamlDotNet.Serialization.NamingConventions;

namespace DiamonDMain
{
    class Program
    {
        private static ArrayList a = new ArrayList { new Danger(), new Tresor(), new Trophee() };

        static void Main(string[] args)
        {
            Carte c = Tirercarte();
            DeserializeGame();
            
        }
        public static Carte Tirercarte()
        {
            Carte c = null;
            if("" == "")
            {
            var rand = new Random();
            c = (Carte)a[rand.Next(a.Count)];
            }
            return c; 
        }
          public static object DeserializeGame()
        {
            String path = @"../../../../Server\Ressources\Server.yaml";

            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            Object config = deserializer.Deserialize<Object>(File.ReadAllText(path));

            return config;
        }
    }
}
