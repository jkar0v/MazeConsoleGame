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
                "easy" => 11,
                "medium" => 17,
                "hard" => 27,
                _ => GetRandomOdd(11, 55)
            };


            char[,] grid = new char[size, size];

            // 1. Запълваме всичко със стени
            for (int i = 0; i < size; i++)
                for (int j = 0; j < size; j++)
                    grid[i, j] = '#';

            // 2. Генерираме само една истинска пътека с DFS
            GenerateMazeDFS(grid, 1, 1);

            // 3. Старт и край
            (int sr, int sc) = FindRandomFreeCell(grid, size);
            (int er, int ec) = FindRandomFreeCell(grid, size, sr, sc);

            grid[sr, sc] = 'O';
            grid[er, ec] = 'X';

            Maze maze = new Maze(grid);

            // 4. Проверка за решение
            var solution = maze.Solve(sr, sc, er, ec);
            if (solution == null)
            {
                return Generate(difficulty);
            }

            // 5. Добавяне на фалшиви пътеки
            int fakePathCount = difficulty switch
            {
                "easy" => 10,
                "medium" => 20,
                "hard" => 30,
                _ => Random.Shared.Next(10, 50)
            };

            AddFakePaths(grid, fakePathCount);

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
        private static void AddFakePaths(char[,] maze, int count = 50)
        {
            int rows = maze.GetLength(0);
            int cols = maze.GetLength(1);

            for (int i = 0; i < count; i++)
            {
                int r = random.Next(1, rows - 1);
                int c = random.Next(1, cols - 1);

                // Ако до тази клетка има проход, я отваряме
                if (maze[r, c] == '#')
                {
                    // Проверяваме дали има поне един съседен проход
                    int[] dRows = { -1, 1, 0, 0 };
                    int[] dCols = { 0, 0, -1, 1 };

                    // Проверяваме всички четири посоки
                    for (int d = 0; d < 4; d++)
                    {
                        int nr = r + dRows[d];
                        int nc = c + dCols[d];
                        if (maze[nr, nc] == ' ')
                        {
                            maze[r, c] = ' ';
                            break;
                        }
                    }
                }
            }
        }
        private static void GenerateMazeDFS(char[,] grid, int row, int col)
        {
            // Проверка за стена на по-голямо разстояние
            int[] dRows = { -2, 2, 0, 0 };
            int[] dCols = { 0, 0, -2, 2 };

            // Създаваме списък с числата 0, 1, 2 и 3 — това са индекси за посоките
            List<int> directions = new List<int> { 0, 1, 2, 3 };

            // Разбъркваме списъка с помощта на случайния генератор
            for (int i = 0; i < directions.Count; i++)
            {
                int j = random.Next(i, directions.Count);
                int temp = directions[i];
                directions[i] = directions[j];
                directions[j] = temp;
            }


            grid[row, col] = ' ';

            foreach (int dir in directions)
            {
                int newRow = row + dRows[dir];
                int newCol = col + dCols[dir];

                if (newRow > 0 && newRow < grid.GetLength(0) - 1 &&
                    newCol > 0 && newCol < grid.GetLength(1) - 1 &&
                    grid[newRow, newCol] == '#')
                {
                    int wallRow = row + dRows[dir] / 2;
                    int wallCol = col + dCols[dir] / 2;
                    grid[wallRow, wallCol] = ' ';
                    GenerateMazeDFS(grid, newRow, newCol);
                }
            }
        }
        static int GetRandomOdd(int min, int max)
        {
            int number;
            do
            {
                number = Random.Shared.Next(11, 31);
            } while (number % 2 == 0);
            return number;
        }

    }
}
