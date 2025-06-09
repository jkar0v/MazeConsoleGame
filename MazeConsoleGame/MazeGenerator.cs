using MazeConsoleApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MazeConsoleGame
{
    internal class MazeGenerator
    {
        public static Maze Generate(string difficulty)
        {
            string[] lines;

            switch (difficulty)
            {
                case "easy":
                    lines = new string[]
                    {
                        "########",
                        "#@.....#",
                        "###.####",
                        "#......#",
                        "#.######",
                        "#.....X#",
                        "########"
                    };
                    break;

                case "medium":
                    lines = new string[]
                    {
                        "##########",
                        "#@......##",
                        "##.#######",
                        "#.......X#",
                        "##########"
                    };
                    break;

                case "hard":
                    lines = new string[]
                    {
                        "############",
                        "#@#......#.#",
                        "#.#.####.#.#",
                        "#.#....#.#.#",
                        "#.######.#.#",
                        "#.........X#",
                        "############"
                    };
                    break;

                default:
                    lines = new string[]
                    {
                        "#####",
                        "#@.X#",
                        "#####"
                    };
                    break;
            }

            return new Maze(lines);
        }
    }
}
