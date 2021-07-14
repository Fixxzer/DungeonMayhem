using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DungeonMayhem.Library;

namespace Game.ConsoleApp
{
    internal class Program
    {
        private static readonly List<Creature> MasterCreatureList = new List<Creature>
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
                    List<Creature> creatures = new List<Creature>();
                    if (isInteractive)
                    {
                        var numberOfHumanPlayers = DetermineNumberOfHumanPlayers();
                        var humanNames = DetermineHumanNames(numberOfHumanPlayers);
                        var humanList = AssignHumansToCreatures(numberOfHumanPlayers, humanNames);
                        creatures.AddRange(humanList);

                        var numberOfComputerPlayers = DetermineNumberOfComputerPlayers();
                        var computerList = AssignComputerToCreatures(numberOfComputerPlayers);
                        creatures.AddRange(computerList);
                    }
                    else
                    {
                        creatures.AddRange(MasterCreatureList);
                    }

                    GameEngine engine = new GameEngine(creatures, useMightyPowers, writeToConsole, isInteractive);
                    var winOrder = engine.GameLoop();
                    stats.Add(winOrder);

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

        private static int DetermineNumberOfHumanPlayers()
        {
            while (true)
            {
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
                    Console.WriteLine($"Player {i + 1} please enter your name and press the <enter> key");
                    name = Console.ReadLine();
                }
                
                humanNames.Add(name);
            }

            return humanNames;
        }

        private static IEnumerable<Creature> AssignHumansToCreatures(int numberOfHumans, IReadOnlyList<string> humanNameList)
        {
            DisplayCreatureList();

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
                            int randomNumber = new Random().Next(0, MasterCreatureList.Count);
                            selectedPlayerCharacter = MasterCreatureList[randomNumber];
                        }
                        else
                        {
                            selectedPlayerCharacter = MasterCreatureList.FirstOrDefault(x => x.CreatureId == int.Parse(choice));
                        }

                        if (selectedPlayerCharacter == null)
                        {
                            Console.WriteLine("Failed to determine character, let's try again.");
                        }
                    }
                }

                selectedPlayerCharacter.PlayerNumber = i + 1;
                selectedPlayerCharacter.PlayerName = humanNameList[i];
                selectedPlayerCharacter.IsHuman = true;

                humanList.Add(selectedPlayerCharacter);
            }

            return humanList;
        }

        private static IEnumerable<Creature> AssignComputerToCreatures(int numberOfComputerPlayers)
        {
            DisplayCreatureList();

            var computerList = new List<Creature>(numberOfComputerPlayers);
            for (int i = 0; i < numberOfComputerPlayers; i++)
            {
                Creature selectedCharacter = null;
                while (selectedCharacter == null)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Which character would you like the computer player {i} to be?  Press the number for the character you would like to play and press the <enter> key, or <r> for random.");
                    var choice = Console.ReadLine();
                    if (!string.IsNullOrWhiteSpace(choice))
                    {
                        if (choice.ToLower() == "r")
                        {
                            int randomNumber = new Random().Next(0, MasterCreatureList.Count);
                            selectedCharacter = MasterCreatureList[randomNumber];
                        }
                        else
                        {
                            selectedCharacter = MasterCreatureList.FirstOrDefault(x => x.CreatureId == int.Parse(choice));
                        }

                        if (selectedCharacter == null)
                        {
                            Console.WriteLine("Failed to determine character, let's try again.");
                        }
                    }
                }

                computerList.Add(selectedCharacter);
            }

            return computerList;
        }

        private static void DisplayCreatureList()
        {
            foreach (var creature in MasterCreatureList)
            {
                Console.WriteLine($"{creature.CreatureId} - {creature.CreatureName}");
            }
            Console.WriteLine();
        }
    }
}
