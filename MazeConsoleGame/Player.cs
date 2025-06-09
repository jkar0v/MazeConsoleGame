
using System;

namespace MazeConsoleApp
{
    public class Player
    {
        public int Row { get; set; }
        public int Col { get; set; }

        public Player(int row, int col)
        {
            Row = row;
            Col = col;
        }

        public void Move(ConsoleKey key, Maze maze)
        {
            int newRow = Row, newCol = Col;
            switch (key)
            {
                case ConsoleKey.W: newRow--; break;
                case ConsoleKey.S: newRow++; break;
                case ConsoleKey.A: newCol--; break;
                case ConsoleKey.D: newCol++; break;
            }

            if (maze.IsFree(newRow, newCol))
            {
                Row = newRow;
                Col = newCol;
            }
        }
    }
}
