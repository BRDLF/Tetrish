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
        public int ROWS { get => 22; }
        public int COLS { get => 10; }
        private int[,] grid { get; set; }
        //public int Rows { get; private set; }
        //public int Columns { get; private set; }

        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }
        
        public GameBoard()
        { 
            grid = new int[ROWS, COLS];
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

            for (int r = ROWS - 1; r >= 0; r--)
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
