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
                const int numGames = 1;
                const bool useMightyPowers = true;
                const bool writeToConsole = true;
                const bool isInteractive = true;

                Stopwatch sw1 = new Stopwatch();
                Stopwatch sw2 = new Stopwatch();

                sw1.Start();
                sw2.Start();

                var stats = new List<List<Creature>>();

                for (int i = 1; i <= numGames; i++)
                {
                    var id = 1;
                    var creatureList = new List<Creature>
                    {
                        new Creature(id++, "Azzan", "Azzan.json"),
                        new Creature(id++, "Blorp", "Blorp.json"),
                        new Creature(id++, "Delilah Deathray", "DelilahDeathray.json"),
                        new Creature(id++, "Dr. Tentaculous", "DrTentaculous.json"),
                        new Creature(id++, "Hoots McGoots", "HootsMcGoots.json"),
                        new Creature(id++, "Jaheira", "Jaheira.json"),
                        new Creature(id++, "Lia", "Lia.json"),
                        new Creature(id++, "Lord Cinderpuff", "LordCinderpuff.json"),
                        new Creature(id++, "Mimi LeChaise", "MimiLeChaise.json"),
                        new Creature(id++, "Minsc & Boo", "MinscAndBoo.json"),
                        new Creature(id++, "Oriax", "Oriax.json"),
                        new Creature(id++, "Sutha", "Sutha.json")
                    };

                    int? numHumans = null;
                    if (isInteractive)
                    {
                        while (numHumans == null)
                        {
                            Console.WriteLine($"Enter the amount of human players between 0 and {creatureList.Count} and press the <enter> key");
                            string readLine = Console.ReadLine();

                            if (string.IsNullOrWhiteSpace(readLine))
                            {
                                Console.WriteLine("Please enter a valid number");
                            }
                            else
                            {
                                numHumans = int.Parse(readLine);

                                if (numHumans < 0 || numHumans > creatureList.Count)
                                {
                                    Console.WriteLine("Invalid number, please try again.");
                                    numHumans = null;
                                }
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
                                Console.WriteLine($"{humanNames[j]}, which character would you like to be?  Press the number for the character you would like to play and press the <enter> key, or <r> for random.");
                                var choice = Console.ReadLine();
                                if (!string.IsNullOrWhiteSpace(choice))
                                {
                                    if (choice.ToLower() == "r")
                                    {
                                        creatureList.Shuffle();
                                        selectedPlayerCharacter = creatureList.FirstOrDefault();
                                    }
                                    else
                                    {
                                        selectedPlayerCharacter = creatureList.FirstOrDefault(x => x.CreatureId == int.Parse(choice));
                                    }

                                    if (selectedPlayerCharacter == null)
                                    {
                                        Console.WriteLine("Failed to determine character, let's try again.");
                                    }
                                }
                            }

                            selectedPlayerCharacter.PlayerNumber = j + 1;
                            selectedPlayerCharacter.PlayerName = humanNames[j];
                            selectedPlayerCharacter.IsHuman = true;
                        }

                        int cpuNumChoice;
                        int availableChoices = creatureList.Count - numHumans.Value;
                        while (true)
                        {
                            Console.WriteLine($"How many computer opponents would you prefer?  Enter a number between 0 and {availableChoices} and press the <enter> key.");
                            var input = Console.ReadLine();
                            if (!int.TryParse(input, out cpuNumChoice) || cpuNumChoice < 0 || cpuNumChoice > availableChoices)
                            {
                                Console.WriteLine("Failed to determine choice.  Please try again.");
                            }
                            else
                            {
                                break;
                            }
                        }

                        // @todo: determine which computer opponents (random or manual)

                        var tmpList = creatureList.Where(x => x.IsHuman == false).ToList();
                        tmpList.Shuffle();
                        creatureList.RemoveAll(x => x.IsHuman == false);
                        creatureList.AddRange(tmpList.GetRange(0, cpuNumChoice));
                    }

                    GameEngine engine = new GameEngine(creatureList, useMightyPowers, writeToConsole);
                    var gameLoop = engine.GameLoop(isInteractive);
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
