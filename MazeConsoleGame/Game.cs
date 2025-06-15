using MazeConsoleGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace MazeConsoleApp
{
    public class Game
    {
        private Maze maze;
        private Player player;
        private string mazeFilePath = "maze.txt";
        public bool lost = false;

        public void Start()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            while (true)
            {
                ShowMenu();
            }
        }

        private void ShowMenu()
        {
            Console.Clear();
            Console.WriteLine("=== MAZE GAME ===\n");
            Console.WriteLine("1. Играй ниво (от файл)");
            Console.WriteLine("2. Генерирай лабиринт");
            Console.WriteLine("3. Изход\n");

            Console.Write("Избери опция: ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    PlayLevel();
                    break;
                case "2":
                    PlayRandomMaze();
                    break;
                case "3":
                    Console.WriteLine("Излизане...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("Невалиден избор. Натисни Enter.");
                    Console.ReadLine();
                    break;
            }
        }

        private void PlayLevel()
        {
            Console.Clear();
            Console.WriteLine("Избери ниво:");

            string[] files = Directory.GetFiles("Levels", "*.txt");

            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
            }

            Console.Write("Въведи номер на ниво: ");
            if (int.TryParse(Console.ReadLine(), out int selected) &&
                selected >= 1 && selected <= files.Length)
            {
                mazeFilePath = files[selected - 1];
                PlayFromFile(); // използваме вече готовия метод!
            }
            else
            {
                Console.WriteLine("Невалиден избор. Натисни Enter.");
                Console.ReadLine();
            }
        }

        private void PlayFromFile()
        {
            LoadMazeFromFile();
            PlayLoop();
        }

        private void PlayRandomMaze()
        {
            Console.Clear();
            Console.WriteLine("Избери трудност: 1 - easy / 2 - medium / 3 - hard");
            string difficulty = Console.ReadLine();
            switch(difficulty)
            {
                case "1":
                    difficulty = "easy";
                    break;
                case "2":
                    difficulty = "medium";
                    break;
                case "3":
                    difficulty = "hard";
                    break;
                default:
                    break;
            }

            maze = MazeGenerator.Generate(difficulty);
            player = new Player(maze.StartRow, maze.StartCol);

            PlayLoop();
        }

        private void LoadMazeFromFile()
        {
            var lines = File.ReadAllLines(mazeFilePath);
            maze = new Maze(lines);
            player = new Player(maze.StartRow, maze.StartCol);
        }

        private void PlayLoop()
        {
            while (true)
            {
                Console.Clear();
                maze.Print(player);
                Console.WriteLine("Move with W/A/S/D or ← ↑ ↓ →. Enter 0 to show solution. Q to quit.");
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Q)
                    break;

                if (key == ConsoleKey.D0)
                {
                    var path = maze.Solve(player.Row, player.Col);
                    if (path != null)
                    {
                        foreach (var step in path)
                        {
                            player.Row = step.Item1;
                            player.Col = step.Item2;
                            Console.Clear();
                            maze.Print(player);
                            Thread.Sleep(200);
                        }
                        lost = true;
                    }
                    else
                    {
                        Console.WriteLine("No solution found.");
                        Console.ReadKey();
                    }
                    continue;
                }

                player.Move(key, maze);
                if (player.Row == maze.EndRow && player.Col == maze.EndCol)
                {
                    Console.Clear();
                    maze.Print(player);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("\nПоздравления! Стигна до края на лабиринта!");
                    Console.ResetColor();
                    Console.WriteLine("Натисни Enter, за да се върнеш в менюто.");
                    Console.ReadLine();
                    break; // излиза от while цикъла и приключва текущата игра
                }
            }
        }
    }
}
