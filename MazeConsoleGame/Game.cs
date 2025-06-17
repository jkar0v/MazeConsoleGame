using MazeConsoleGame;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
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
            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    PlayLevel();
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    PlayRandomMaze();
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    Console.WriteLine("Излизане...");
                    Environment.Exit(0);
                    break;
                default:
                    Console.WriteLine("\nНевалиден избор!");
                    Thread.Sleep(1000); // малко изчакване за ефект
                    break;
            }
        }

        private void PlayLevel()
        {
            Console.Clear();
            Console.WriteLine("Избери ниво:");

            string[] files = Directory.GetFiles(@"..\..\..\Levels", "*.txt");

            // Правим си нов масив със същите файлове, но ще го сортираме ръчно
            for (int i = 0; i < files.Length - 1; i++)
            {
                for (int j = i + 1; j < files.Length; j++)
                {
                    int num1 = GetLevelNumber(files[i]);
                    int num2 = GetLevelNumber(files[j]);

                    if (num1 > num2)
                    {
                        // Разменяме местата
                        string temp = files[i];
                        files[i] = files[j];
                        files[j] = temp;
                    }
                }
            }

            for (int i = 0; i < files.Length; i++)
            {
                Console.WriteLine($"{i + 1}. {Path.GetFileName(files[i])}");
            }

            // Този метод взима номера от името
            int GetLevelNumber(string filePath)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                string numberPart = fileName.Replace("level ", "");
                return int.Parse(numberPart);
            }

            Console.Write("Въведи номер на ниво: ");
            if (int.TryParse(Console.ReadLine(), out int selected) &&
                selected >= 1 && selected <= files.Length)
            {
                mazeFilePath = files[selected - 1];
                PlayFromFile();
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

        private void LoadMazeFromFile()
        {
            List<string> lines = new List<string>();

            using (StreamReader reader = new StreamReader(mazeFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    lines.Add(line);
                }
            }
            Console.WriteLine("Край на файла");


            maze = new Maze(lines.ToArray());
            player = new Player(maze.StartRow, maze.StartCol);
        }


        private void PlayRandomMaze()
        {
            Console.Clear();
            Console.WriteLine("Избери трудност:");
            Console.WriteLine("1 - easy");
            Console.WriteLine("2 - medium");
            Console.WriteLine("3 - hard\n");
            Console.Write("Избери опция: ");

            string difficulty = null;

            ConsoleKey key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.D1:
                case ConsoleKey.NumPad1:
                    difficulty = "easy";
                    break;
                case ConsoleKey.D2:
                case ConsoleKey.NumPad2:
                    difficulty = "medium";
                    break;
                case ConsoleKey.D3:
                case ConsoleKey.NumPad3:
                    difficulty = "hard";
                    break;
                default:
                    Console.Clear();
                    Console.WriteLine("Невалиден избор. Избираме случайно...");
                    Thread.Sleep(1000); // малко изчакване за ефект
                    break;
            }

            maze = MazeGenerator.Generate(difficulty);
            player = new Player(maze.StartRow, maze.StartCol);

            PlayLoop();
        }



        private void PlayLoop()
        {

            while (maze.Cols > Console.WindowWidth || maze.Rows > Console.WindowHeight - 10)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Моля, пуснете играта на цел екран!");
                Console.ResetColor();
                Console.WriteLine("Натиснете Enter за да опитате пак.");
                Console.ReadLine();
            }
            Console.Clear();
            maze.Print();


            Console.SetCursorPosition(player.Col, player.Row);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write('@');
            Console.ResetColor();

            bool done = false;
            while (true)
            {
                if (!done)
                {
                    Console.SetCursorPosition(0, maze.Rows + 1);
                    ShowRules();
                }
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.Q)
                    break;

                if (key == ConsoleKey.D0 || key == ConsoleKey.NumPad0)
                {
                    maze.ShowHelpPaths(player.Row, player.Col); // Оцветява пътя от началото и от човека до края
                    Console.Clear();
                    maze.Print(player);
                    continue;
                }
                if (key == ConsoleKey.P)
                {
                    var path = maze.Solve(maze.StartRow, maze.StartCol, maze.EndRow, maze.EndCol);
                    if (path != null)
                    {
                        foreach (var (r, c) in path)
                        {
                            player.Row = r;
                            player.Col = c;
                            Console.Clear();
                            maze.Print(player);
                            Thread.Sleep(100);
                        }
                        done = true;
                    }
                    else
                    {
                        Console.WriteLine("No solution found.");
                        Console.ReadKey();
                    }
                    continue;
                }

                // Изтриваме стария играч
                Console.SetCursorPosition(player.Col, player.Row);
                char oldCell = maze.GetCell(player.Row, player.Col); // Вземаме какво има под играча
                if (oldCell == '#') Console.ForegroundColor = ConsoleColor.DarkGray;
                else if (oldCell == 'X') Console.ForegroundColor = ConsoleColor.Green;
                else if (oldCell == '.') Console.ForegroundColor = ConsoleColor.Magenta;
                else if (oldCell == '*') Console.ForegroundColor = ConsoleColor.Blue;
                else Console.ResetColor();

                Console.Write(oldCell);
                Console.ResetColor();

                // Преместваме играча
                player.Move(key, maze);

                // Рисуваме новата позиция
                Console.SetCursorPosition(player.Col, player.Row);
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write('@');
                Console.ResetColor();

                if (player.Row == maze.EndRow && player.Col == maze.EndCol)
                {
                    Console.SetCursorPosition(0, maze.Rows + 2);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Поздравления! Стигна до края на лабиринта!");
                    Console.ResetColor();
                    Console.WriteLine("Натисни Enter, за да се върнеш в менюто.");
                    Console.ReadLine();
                    break;
                }
            }
        }
        private void ShowRules()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("=== GAME RULES ===");
            Console.ResetColor();

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Movement:");
            Console.ResetColor();
            Console.WriteLine("W / A / S / D   or   ← ↑ ↓ →");

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Options:");
            Console.ResetColor();
            Console.WriteLine("0  -  Show solution");
            Console.WriteLine("P  -  Give up");
            Console.WriteLine("Q  -  Quit");
        }
    }
}
