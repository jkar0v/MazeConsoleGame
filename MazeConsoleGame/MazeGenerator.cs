using MazeConsoleApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeConsoleGame
{
    public static class MazeGenerator
    {
        private static Random random = new Random();
        public static Maze Generate(string difficulty)
        {
            int size = difficulty switch
            {
                "easy" => 10,
                "medium" => 20,
                "hard" => 30,
                _ => 5
            };

            char[,] grid = new char[size, size];

            // 1. Запълваме с '#'
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    grid[i, j] = '#';

            // 2. Правим проходи (simple carve)
            for (int i = 1; i < size - 1; i++)
                for (int j = 1; j < size - 1; j++)
                    grid[i, j] = (Random.Shared.Next(0, 4) == 0) ? '#' : ' ';

            // 3. Генерираме стартова и крайна позиция на различни места
            (int sr, int sc) = FindRandomFreeCell(grid, size);
            (int er, int ec) = FindRandomFreeCell(grid, size, sr, sc);

            grid[sr, sc] = '@';
            grid[er, ec] = 'X';

            Maze maze = new Maze(grid);

            // 4. Проверка дали има решение
            var solution = maze.Solve(sr, sc);
            if (solution == null)
            {
                // Ако няма решение — рекурсивно генерираме ново ниво
                return Generate(difficulty);
            }
            //int fakePathCount = difficulty switch
            //{
            //    "easy" => 1,
            //    "medium" => 80,
            //    "hard" => 200,
            //    _ => 50
            //};

            //AddFakePaths(grid, fakePathCount);

            return maze;
        }

        private static (int, int) FindRandomFreeCell(char[,] grid, int size, int? avoidRow = null, int? avoidCol = null)
        {
            int row, col;
            do
            {
                row = Random.Shared.Next(1, size - 1);
                col = Random.Shared.Next(1, size - 1);
            }
            while (grid[row, col] != ' ' ||
                   (avoidRow.HasValue && avoidCol.HasValue && row == avoidRow && col == avoidCol));
            return (row, col);
        }
        //private static void AddFakePaths(char[,] maze, int count = 50)
        //{
        //    int rows = maze.GetLength(0);
        //    int cols = maze.GetLength(1);

        //    for (int i = 0; i < count; i++)
        //    {
        //        int r = random.Next(1, rows - 1);
        //        int c = random.Next(1, cols - 1);

        //        if (maze[r, c] == '#')
        //        {
        //            // Ако до тази клетка има проход, я отваряме
        //            int[] dRows = { -1, 1, 0, 0 };
        //            int[] dCols = { 0, 0, -1, 1 };

        //            for (int d = 0; d < 4; d++)
        //            {
        //                int nr = r + dRows[d];
        //                int nc = c + dCols[d];
        //                if (maze[nr, nc] == ' ')
        //                {
        //                    maze[r, c] = ' ';
        //                    break;
        //                }
        //            }
        //        }
        //    }
        //}
    }
}
