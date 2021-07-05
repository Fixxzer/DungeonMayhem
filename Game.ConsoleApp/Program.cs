using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using DungeonMayhem.Library;

namespace Game.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                int numGames = 1;
                bool useMightyPowers = true;
                bool writeToConsole = true;

                Stopwatch sw = new Stopwatch();
                sw.Start();
             
                List<List<Creature>> stats = new List<List<Creature>>();
                
                for (int i = 1; i <= numGames; i++)
                {
                    List<Creature> creatureList = new List<Creature>
                    {
                        CreateCreature("Azzan", "Azzan.json"),
                        CreateCreature("Blorp", "Blorp.json"),
                        CreateCreature("Delilah Deathray", "DelilahDeathray.json"),
                        CreateCreature("Dr. Tentaculous", "DrTentaculous.json"),
                        CreateCreature("Hoots McGoots", "HootsMcGoots.json"),
                        CreateCreature("Jaheira", "Jaheira.json"),
                        CreateCreature("Lia", "Lia.json")
                    };

                    GameEngine engine = new GameEngine(creatureList, useMightyPowers, writeToConsole);
                    stats.Add(engine.GameLoop());

                    if (i % 1000 == 0)
                    {
                        Console.WriteLine($"Playing game {i}");
                    }
                }


                Dictionary<string, int> results = new Dictionary<string, int>();
                foreach (var stat in stats)
                {
                    var winner = stat.First();
                    if (results.ContainsKey(winner.CreatureName))
                    {
                        results[winner.CreatureName]++;
                    }
                    else
                    {
                        results.Add(winner.CreatureName, 1);
                    }
                }

                sw.Stop();

                Console.WriteLine($"Game complete in {sw.Elapsed}.");
                Console.WriteLine();

                foreach (var result in results.OrderByDescending(x => x.Value))
                {
                    Console.WriteLine($"{result.Key}: won {result.Value} times! {result.Value / (float)numGames:P}%");
                }

                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static Creature CreateCreature(string name, string jsonFile)
        {
            return new Creature
            {
                CreatureName = name,
                CurrentHitPoints = 10,
                DiscardDeck = new Deck(),
                DrawDeck = JsonSerializer.Deserialize<Deck>(File.ReadAllText($"Creatures\\{jsonFile}")),
                InHandDeck = new Deck(),
                IsCpu = true,
                MaxHitPoints = 10,
                NumberOfShields = 0,
                PlayerName = string.Empty
            };
        }
    }
}
