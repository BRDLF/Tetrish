using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Tetrish
{
    internal class GameBoard
    {
        const int ROWS = 22;
        const int COLS = 10;
        private readonly int[,] grid;
        public int Rows { get; }
        public int Columns { get; }

        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }
        
        public GameBoard()
        { 
            Rows = ROWS; Columns = COLS;
            grid = new int[Rows, Columns];
        }

        private bool InBoard(int r, int c)
        {
            return r >= 0 && r < ROWS && c >= 0 && c < COLS;
        }

        public bool IsEmpty(int r, int c)
        {
            return InBoard(r, c) && grid[r, c] == 0;
        }

        private bool IsRowFull(int r)
        {
            for (int c = 0; c < COLS; c++)
            {
                if (grid[r, c] == 0)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsRowEmpty(int r)
        {
            for (int c = 0; c < COLS; c++)
            {
                if (grid[r, c] != 0)
                {
                    return false;
                }
            }
            return true;
        }

        private void ClearRow(int r)
        {
            for (int c = 0; c < COLS; c++)
            {
                grid[r, c] = 0;
            }
        }

        private void MoveRow(int r, int n)
        {
            for (int c = 0; c < COLS; c++)
            {
                grid[r + n, c] = grid[r, c];
            }
        }

        public int ClearRows()
        {
            int cleared = 0;

            for (int r = Rows - 1; r >= 0; r--)
                {
                if (IsRowFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRow(r, cleared);
                }
            }

            return cleared;
        }
    }
}
