
using System;
using System.Collections.Generic;

namespace MazeConsoleApp
{
    public class Maze
    {
        private char[,] grid;
        public int Rows { get; }
        public int Cols { get; }
        public int StartRow { get; }
        public int StartCol { get; }
        public int EndRow, EndCol;

        //  онструктор от текстов масив
        public Maze(string[] lines)
        {
            Rows = lines.Length;
            Cols = lines[0].Length;
            grid = new char[Rows, Cols];

            for (int i = 0; i < Rows; i++)
            {
                for (int j = 0; j < Cols; j++)
                {
                    grid[i, j] = lines[i][j];
                    if (grid[i, j] == '@') { StartRow = i; StartCol = j; }
                    if (grid[i, j] == 'X') { EndRow = i; EndCol = j; }
                }
            }
        }

        //  онструктор от двумерен масив (за автоматично генериране)
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

        public List<(int, int)> Solve(int startRow, int startCol)
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
                if (grid[r, c] == 'X')
                {
                    var path = new List<(int, int)>();
                    while (parent[r, c] != null)
                    {
                        path.Add((r, c));
                        (r, c) = parent[r, c].Value;
                    }
                    path.Reverse();
                    return path;
                }

                for (int d = 0; d < 4; d++)
                {
                    int nr = r + dr[d], nc = c + dc[d];
                    if (IsFree(nr, nc) && !visited[nr, nc])
                    {
                        queue.Enqueue((nr, nc));
                        visited[nr, nc] = true;
                        parent[nr, nc] = (r, c);
                    }
                }
            }

            return null;
        }
    }
}
