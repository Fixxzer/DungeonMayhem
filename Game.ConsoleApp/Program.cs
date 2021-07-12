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
                const int numGames = 10;
                const bool useMightyPowers = true;
                const bool writeToConsole = true;
                const bool isInteractive = true;

                Dictionary<string, string> referenceList = new Dictionary<string, string>
                {
                    {"Azzan", "Azzan.json"},
                    {"Blorp", "Blorp.json"},
                    {"Delilah Deathray", "DelilahDeathray.json"},
                    {"Dr. Tentaculous", "DrTentaculous.json"},
                    {"Hoots McGoots", "HootsMcGoots.json"},
                    {"Jaheira", "Jaheira.json"},
                    {"Lia", "Lia.json"},
                    {"Lord Cinderpuff", "LordCinderpuff.json"},
                    {"Mimi LeChaise", "MimiLeChaise.json"},
                    {"Minsc & Boo", "MinscAndBoo.json"}
                };

                Stopwatch sw1 = new Stopwatch();
                Stopwatch sw2 = new Stopwatch();

                sw1.Start();
                sw2.Start();

                List<List<Creature>> stats = new List<List<Creature>>();

                for (int i = 1; i <= numGames; i++)
                {
                    var creatureList = referenceList.Select(kvp => new Creature(kvp.Key, kvp.Value)).ToList();

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

                        List<string> humanNames = new List<string>(numHumans.Value);
                        for (int k = 0; k < numHumans; k++)
                        {
                            Console.WriteLine($"Player {k + 1} please enter your name and press the <enter> key");
                            string name = Console.ReadLine();
                            humanNames.Add(name);
                        }

                        int cpuOptionCount = 1;
                        foreach (var kvp in referenceList)
                        {
                            Console.WriteLine($"{cpuOptionCount++} - {kvp.Key}");
                        }

                        Console.WriteLine();
                        for (int j = 0; j < numHumans; j++)
                        {
                            int selectedChar = 0;
                            while (selectedChar == 0)
                            {
                                Console.WriteLine();
                                Console.WriteLine($"{humanNames[j]}, which character would you like to be?  Press the number for the character you would like to play and press the <enter> key.");
                                var choice = Console.ReadLine();
                                selectedChar = int.Parse(choice);
                            }

                            var selectedPlayerCharacterName = referenceList.ElementAt(selectedChar - 1).Key;
                            var playerCharacter = creatureList.FirstOrDefault(x => x.CreatureName == selectedPlayerCharacterName);
                            playerCharacter.PlayerNumber = j + 1;
                            playerCharacter.PlayerName = humanNames[j];
                            playerCharacter.IsHuman = true;
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

                sw1.Stop();

                Console.WriteLine($"Game complete in {sw1.Elapsed}.");
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
    }
}
