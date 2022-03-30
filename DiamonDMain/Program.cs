using System;
using System.Collections;

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
          public static string DeserializeGame()
        {
            String path = @"\Server\Ressources\Server.yaml";
            var deserializer = new YamlDotNet.Serialization.DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
            string config = deserializer.Deserialize<string>(File.ReadAllText(path));

            return config;
        }
    }
}
