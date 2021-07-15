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
                const int numGames = 1000;
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

                        var numberOfHumanPlayers = DetermineNumberOfHumanPlayers();
                        var humanNames = DetermineHumanNames(numberOfHumanPlayers);
                        var humanList = AssignHumansToCreatures(numberOfHumanPlayers, humanNames, masterCreatureList);
                        creatures.AddRange(humanList);

                        var numberOfComputerPlayers = DetermineNumberOfComputerPlayers();
                        var computerList = AssignComputerToCreatures(numberOfComputerPlayers, masterCreatureList);
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

        private static int DetermineNumberOfHumanPlayers()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Enter the amount of human players you would like and press the <enter> key");
                string readLine = Console.ReadLine();
                if (!int.TryParse(readLine, out var result) || result < 0)
                {
                    Console.WriteLine("Invalid number, please try again.");
                }
                else
                {
                    return result;
                }
            }
        }

        private static int DetermineNumberOfComputerPlayers()
        {
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("Enter the amount of computer players you would like and press the <enter> key");
                string readLine = Console.ReadLine();
                if (!int.TryParse(readLine, out var result) || result < 0)
                {
                    Console.WriteLine("Invalid number, please try again.");
                }
                else
                {
                    return result;
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

        private static IEnumerable<Creature> AssignHumansToCreatures(int numberOfHumans, IReadOnlyList<string> humanNameList, IReadOnlyList<Creature> masterCreatureList)
        {
            DisplayCreatureList(masterCreatureList);

            var humanList = new List<Creature>(numberOfHumans);

            for (int i = 0; i < numberOfHumans; i++)
            {
                Creature selectedPlayerCharacter = null;
                while (selectedPlayerCharacter == null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"{humanNameList[i]}, which character would you like to be?  Press the number for the character you would like to play and press the <enter> key, or <r> for random.");
                    var choice = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(choice))
                    {
                        if (choice.ToLower() == "r")
                        {
                            while (!humanList.Contains(selectedPlayerCharacter))
                            {
                                int randomNumber = new Random().Next(0, masterCreatureList.Count);
                                selectedPlayerCharacter = masterCreatureList[randomNumber];
                            }
                        }
                        else
                        {
                            selectedPlayerCharacter = masterCreatureList.FirstOrDefault(x => x.CreatureId == int.Parse(choice));
                        }

                        if (selectedPlayerCharacter == null)
                        {
                            Console.WriteLine("Failed to determine character, let's try again.");
                        }
                    }

                    if (humanList.Contains(selectedPlayerCharacter))
                    {
                        Console.WriteLine("That player has already been selected, please select another.");
                        selectedPlayerCharacter = null;
                    }
                }

                selectedPlayerCharacter.PlayerNumber = i + 1;
                selectedPlayerCharacter.PlayerName = humanNameList[i];
                selectedPlayerCharacter.IsHuman = true;

                humanList.Add(selectedPlayerCharacter);
            }

            return humanList;
        }

        private static IEnumerable<Creature> AssignComputerToCreatures(int numberOfComputerPlayers, IEnumerable<Creature> masterCreatureList)
        {
            var availableCreatures = masterCreatureList.Where(x => !x.IsHuman).ToList();
            DisplayCreatureList(availableCreatures);

            var playerList = new List<Creature>(numberOfComputerPlayers);
            for (int i = 0; i < numberOfComputerPlayers; i++)
            {
                Creature selectedCharacter = null;
                while (selectedCharacter == null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Which character would you like the computer player {i+1} to be?  Press the number for the character you would like to play and press the <enter> key, or <r> for random.");
                    var choice = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(choice))
                    {
                        if (choice.ToLower() == "r")
                        {
                            while (!playerList.Contains(selectedCharacter))
                            {
                                int randomNumber = new Random().Next(0, availableCreatures.Count);
                                selectedCharacter = availableCreatures[randomNumber];
                            }
                        }
                        else
                        {
                            selectedCharacter = availableCreatures.FirstOrDefault(x => x.CreatureId == int.Parse(choice));
                        }

                        if (selectedCharacter == null)
                        {
                            Console.WriteLine("Failed to determine character, let's try again.");
                        }
                    }

                    if (playerList.Contains(selectedCharacter))
                    {
                        Console.WriteLine("That player has already been selected, please select another.");
                        selectedCharacter = null;
                    }
                }

                playerList.Add(selectedCharacter);
            }

            return playerList;
        }

        private static void DisplayCreatureList(IEnumerable<Creature> masterCreatureList, IEnumerable<Creature> availableCreatures = null)
        {
            Console.WriteLine();
            foreach (var creature in availableCreatures ?? masterCreatureList)
            {
                Console.WriteLine($"{creature.CreatureId} - {creature.CreatureName}");
            }
            Console.WriteLine();
        }
    }
}
