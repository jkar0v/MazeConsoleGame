
using System;
using System.Collections.Generic;

namespace MazeConsoleApp
{
    public class Maze
    {
        private char[,] grid;
        public int Rows { get; }
        public int Cols { get; }
        public int StartRow { get; private set; }
        public int StartCol { get; private set; }

        public int EndRow, EndCol;

        // Конструктор от текстов масив
        public Maze(string[] lines)
        {
            Rows = lines.Length;
            Cols = lines[0].Length;
            grid = new char[Rows, Cols];

            bool foundStart = false, foundEnd = false;

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    grid[i, j] = lines[i][j];
                    if (grid[i, j] == '@')
                    {
                        StartRow = i;
                        StartCol = j;
                        foundStart = true;
                    }
                    else if (grid[i, j] == 'X')
                    {
                        EndRow = i;
                        EndCol = j;
                        foundEnd = true;
                    }

                    if (foundStart && foundEnd)
                        break;
                }
                if (foundStart && foundEnd)
                    break;
            }

        }

        // Конструктор от двумерен масив (за автоматично генериране)
        public Maze(char[,] customGrid)
        {
            Rows = customGrid.GetLength(0);
            Cols = customGrid.GetLength(1);
            grid = new char[Rows, Cols];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    grid[i, j] = customGrid[i, j];
                    if (grid[i, j] == '@') { StartRow = i; StartCol = j; }
                    if (grid[i, j] == 'X') { EndRow = i; EndCol = j; }
                }
            }
        }

        public void Print(Player player)
        {
            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    if (i == player.Row && j == player.Col)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write('@');
                    }
                    else if (grid[i, j] == '#')
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write('#');
                    }
                    else if (grid[i, j] == 'X')
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write('X');
                    }
                    else if (grid[i, j] == '.')
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write('.');
                    }
                    else if (grid[i, j] == '*')
                    {
                        Console.ForegroundColor = ConsoleColor.Blue;
                        Console.Write('*');
                    }
                    else
                    {
                        Console.ResetColor();
                        Console.Write(grid[i, j]);
                    }
                }
                Console.ResetColor();
                Console.WriteLine();
            }
        }

        public bool IsFree(int row, int col)
        {
            return row >= 0 && row < Rows &&
                   col >= 0 && col < Cols &&
                   grid[row, col] != '#';
        }

        public List<(int, int)> Solve(int startRow, int startCol, int endRow, int endCol)
        {
            var queue = new Queue<(int, int)>();
            var visited = new bool[Rows, Cols];
            var parent = new (int, int)?[Rows, Cols];
            queue.Enqueue((startRow, startCol));
            visited[startRow, startCol] = true;

            int[] dr = { -1, 1, 0, 0 };
            int[] dc = { 0, 0, -1, 1 };

            while (queue.Count > 0)
            {
                var (r, c) = queue.Dequeue();
                if (r == endRow && c == endCol)
                {
                    var path = new List<(int, int)>();
                    while (r != startRow || c != startCol)
                    {
                        path.Add((r, c));
                        (r, c) = parent[r, c].Value;
                    }
                    path.Reverse();
                    return path;
                }

                for (int i = 0; i < 4; i++)
                {
                    int nr = r + dr[i], nc = c + dc[i];
                    if (IsFree(nr, nc) && !visited[nr, nc])
                    {
                        visited[nr, nc] = true;
                        parent[nr, nc] = (r, c);
                        queue.Enqueue((nr, nc));
                    }
                }
            }
            return null;
        }
        public void ShowHelpPaths(int playerRow, int playerCol)
        {
            // 1. Изчистване на стари символи
            for (int i = 0; i < Rows; i++)
                for (int j = 0; j < Cols; j++)
                    if (grid[i, j] == '.' || grid[i, j] == '*')
                        grid[i, j] = ' ';

            // 2. Път от начало до край
            var pathToEnd = Solve(StartRow, StartCol, EndRow, EndCol);

            // 3. Път от текущата позиция до началото
            var pathToStart = Solve(playerRow, playerCol, StartRow, StartCol);

            // 4. Маркиране на пътя до края с '.'
            if (pathToEnd != null)
            {
                foreach (var (r, c) in pathToEnd)
                {
                    if (grid[r, c] == ' ')
                        grid[r, c] = '.';
                }
            }

            // 5. Маркиране на пътя до началото с '*'
            if (pathToStart != null)
            {
                foreach (var (r, c) in pathToStart)
                {
                    if (grid[r, c] == ' ')
                        grid[r, c] = '*';
                }
            }
        }
    }
}
