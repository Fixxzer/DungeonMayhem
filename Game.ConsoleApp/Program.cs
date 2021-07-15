using System;
using System.Collections;
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
                const int numGames = 100;
                const bool useMightyPowers = true;
                const bool writeToConsole = true;
                const bool isInteractive = false;

                Stopwatch sw1 = new Stopwatch();
                Stopwatch sw2 = new Stopwatch();

                sw1.Start();
                sw2.Start();

                var stats = new List<List<Creature>>();

                for (int i = 1; i <= numGames; i++)
                {
                    var masterCreatureList = new List<Creature>
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
                        new Creature(10, "Minsc & Boo", "MinscAndBoo.json"),
                        new Creature(11, "Oriax", "Oriax.json"),
                        new Creature(12, "Sutha", "Sutha.json")
                    };

                    var creatures = new List<Creature>();
                    if (isInteractive)
                    {
                        Console.WriteLine();
                        Console.WriteLine("Welcome to Dungeon Mayhem!");

                        var availableCreatures = new List<Creature>(masterCreatureList);

                        var numberOfHumanPlayers = DetermineNumberOfHumanPlayers(availableCreatures);
                        var humanNames = DetermineHumanNames(numberOfHumanPlayers);
                        var humanList = AssignHumansToCreatures(numberOfHumanPlayers, humanNames, availableCreatures);
                        creatures.AddRange(humanList);

                        var numberOfComputerPlayers = DetermineNumberOfComputerPlayers(availableCreatures);
                        var computerList = AssignComputerToCreatures(numberOfComputerPlayers, availableCreatures);
                        creatures.AddRange(computerList);
                    }
                    else
                    {
                        creatures.AddRange(masterCreatureList);
                    }

                    GameEngine engine = new GameEngine(creatures, useMightyPowers, writeToConsole, isInteractive);
                    var winOrder = engine.GameLoop();
                    stats.Add(winOrder);

                    if (i % 500 == 0)
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

        private static int DetermineNumberOfHumanPlayers(ICollection availableCreatures)
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine($"Enter the amount of human players you would like between 0 and {availableCreatures.Count} and press the <enter> key");
                string readLine = Console.ReadLine();
                if (int.TryParse(readLine, out var result) && result >= 0 && result <= availableCreatures.Count)
                {
                    return result;
                }

                Console.WriteLine("Invalid number, please try again.");
            }
        }

        private static int DetermineNumberOfComputerPlayers(IReadOnlyCollection<Creature> availableCreatures)
        {
            DisplayCreatureList(availableCreatures);
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine($"Enter the amount of computer players you would like between 0 and {availableCreatures.Count} and press the <enter> key");
                string readLine = Console.ReadLine();
                if (int.TryParse(readLine, out var result) && result >= 0 && result <= availableCreatures.Count)
                {
                    return result;
                }
                else
                {
                    Console.WriteLine("Invalid number, please try again.");
                }
            }
        }

        private static List<string> DetermineHumanNames(int numberOfHumans)
        {
            var humanNames = new List<string>(numberOfHumans);
            for (int i = 0; i < numberOfHumans; i++)
            {
                string name = null;
                while (string.IsNullOrWhiteSpace(name))
                {
                    Console.WriteLine();
                    Console.WriteLine($"Player {i + 1} please enter your name and press the <enter> key");
                    name = Console.ReadLine();
                }
                
                humanNames.Add(name);
            }

            return humanNames;
        }

        private static IEnumerable<Creature> AssignHumansToCreatures(int numberOfPlayers, IReadOnlyList<string> humanNameList, IList<Creature> availableCreatures)
        {
            var playerList = new List<Creature>(numberOfPlayers);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                DisplayCreatureList(availableCreatures);
                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine($"{humanNameList[i]}, which character would you like to be?  Press the number for the character you would like to play and press the <enter> key, or <r> for random.");
                    var choice = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(choice))
                    {
                        if (choice.ToLower() == "r")
                        {
                            int randomNumber = new Random().Next(0, availableCreatures.Count);
                            availableCreatures[randomNumber].PlayerNumber = i + 1;
                            availableCreatures[randomNumber].PlayerName = humanNameList[i];
                            availableCreatures[randomNumber].IsHuman = true;
                            
                            playerList.Add(availableCreatures[randomNumber]);
                            availableCreatures.RemoveAt(randomNumber);
                            break;
                        }

                        if (int.TryParse(choice, out var result))
                        {
                            result -= 1;
                            if (result >= 0 && result < availableCreatures.Count)
                            {
                                availableCreatures[result].PlayerNumber = i + 1;
                                availableCreatures[result].PlayerName = humanNameList[i];
                                availableCreatures[result].IsHuman = true;

                                playerList.Add(availableCreatures[result]);
                                availableCreatures.RemoveAt(result);
                                break;
                            }
                        }
                    }

                    Console.WriteLine("Failed to determine character, let's try again.");
                }
            }

            return playerList;
        }

        private static IEnumerable<Creature> AssignComputerToCreatures(int numberOfPlayers, IList<Creature> availableCreatures)
        {
            var playerList = new List<Creature>(numberOfPlayers);

            for (int i = 0; i < numberOfPlayers; i++)
            {
                DisplayCreatureList(availableCreatures);
                while (true)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Which character would you like the computer player {i + 1} to be?  Press the number for the character you would like to play and press the <enter> key, or <r> for random, or <a> to randomize all of them.");
                    var choice = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(choice))
                    {
                        if (choice.ToLower() == "r")
                        {
                            int randomNumber = new Random().Next(0, availableCreatures.Count);
                            playerList.Add(availableCreatures[randomNumber]);
                            availableCreatures.Remove(availableCreatures[randomNumber]);
                            break;
                        }

                        if (choice.ToLower() == "a")
                        {
                            int count = 0;
                            while (availableCreatures.Any() && count < numberOfPlayers)
                            {
                                int randomNumber = new Random().Next(0, availableCreatures.Count);
                                playerList.Add(availableCreatures[randomNumber]);
                                availableCreatures.Remove(availableCreatures[randomNumber]);
                                count++;
                            }

                            return playerList;
                        }

                        if (int.TryParse(choice, out var result))
                        {
                            result -= 1;
                            if (result >= 0 && result < availableCreatures.Count)
                            {
                                playerList.Add(availableCreatures[result]);
                                availableCreatures.Remove(availableCreatures[result]);
                                break;
                            }
                        }
                    }

                    Console.WriteLine("Failed to determine character, let's try again.");
                }
            }

            return playerList;
        }

        private static void DisplayCreatureList(IEnumerable<Creature> masterCreatureList, IEnumerable<Creature> availableCreatures = null)
        {
            Console.WriteLine();
            int count = 1;
            foreach (var creature in availableCreatures ?? masterCreatureList)
            {
                Console.WriteLine($"{count++} - {creature.CreatureName}");
            }
            Console.WriteLine();
        }
    }
}
