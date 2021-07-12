using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DungeonMayhem.Library;

namespace Game.ConsoleApp
{
    internal class Program
    {
        private static void Main()
        {
            try
            {
                const int numGames = 10000;
                const bool useMightyPowers = true;
                const bool writeToConsole = false;
                const bool isInteractive = false;

                Stopwatch sw1 = new Stopwatch();
                Stopwatch sw2 = new Stopwatch();

                sw1.Start();
                sw2.Start();

                var stats = new List<List<Creature>>();

                for (int i = 1; i <= numGames; i++)
                {
                    var creatureList = new List<Creature>
                    {
                        new Creature(1, "Azzan", "Azzan.json"),
                        new Creature(2, "Blorp", "Blorp.json"),
                        new Creature(3, "Delilah Deathray", "DelilahDeathray.json"),
                        new Creature(4, "Dr. Tentaculous", "DrTentaculous.json"),
                        new Creature(5, "Hoots McGoots", "HootsMcGoots.json"),
                        new Creature(6, "Jaheira", "Jaheira.json"),
                        new Creature(7, "Lia", "Lia.json"),
                        new Creature(8, "Lord Cinderpuff", "LordCinderpuff.json"),
                        new Creature(9, "Mimi LeChaise", "MimiLeChaise.json"),
                        new Creature(10, "Minsc & Boo", "MinscAndBoo.json")
                    };

                    int? numHumans = null;
                    if (isInteractive)
                    {
                        while (numHumans == null)
                        {
                            Console.WriteLine("Enter the amount of players and press the <enter> key");
                            string readLine = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(readLine))
                            {
                                Console.WriteLine("Please enter a valid number");
                            }
                            else
                            {
                                numHumans = int.Parse(readLine);
                            }
                        }

                        var humanNames = new List<string>(numHumans.Value);
                        for (int k = 0; k < numHumans; k++)
                        {
                            Console.WriteLine($"Player {k + 1} please enter your name and press the <enter> key");
                            string name = Console.ReadLine();
                            humanNames.Add(name);
                        }

                        foreach (var creature in creatureList)
                        {
                            Console.WriteLine($"{creature.CreatureId} - {creature.CreatureName}");
                        }

                        Console.WriteLine();
                        for (int j = 0; j < numHumans; j++)
                        {
                            Creature selectedPlayerCharacter = null;
                            while (selectedPlayerCharacter == null)
                            {
                                Console.WriteLine();
                                Console.WriteLine($"{humanNames[j]}, which character would you like to be?  Press the number for the character you would like to play and press the <enter> key.");
                                var choice = Console.ReadLine();
                                selectedPlayerCharacter = creatureList.FirstOrDefault(x => x.CreatureId == int.Parse(choice ?? string.Empty));
                                if (selectedPlayerCharacter == null)
                                {
                                    Console.WriteLine("Failed to determine character, let's try again.");
                                }
                            }

                            selectedPlayerCharacter.PlayerNumber = j + 1;
                            selectedPlayerCharacter.PlayerName = humanNames[j];
                            selectedPlayerCharacter.IsHuman = true;
                        }
                    }

                    GameEngine engine = new GameEngine(creatureList, useMightyPowers, writeToConsole);
                    var gameLoop = engine.GameLoop();
                    stats.Add(gameLoop);

                    if (i % 1000 == 0)
                    {
                        Console.WriteLine($"Playing game {i} in {sw2.Elapsed}");
                        sw2.Restart();
                    }
                }

                var results = new Dictionary<string, int>();
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

                sw1.Stop();

                Console.WriteLine($"Game complete in {sw1.Elapsed}.");
                Console.WriteLine();

                foreach (var (key, value) in results.OrderByDescending(x => x.Value))
                {
                    Console.WriteLine($"{key}: won {value} times! {value / (float)numGames:P}%");
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
    }
}
