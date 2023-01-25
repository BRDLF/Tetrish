using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetrish
{
    internal class StateInfo
    {
        private Piece currentPiece;

        public Piece CurrentPiece
        {
            get => currentPiece;
            private set
            {
                currentPiece = value;
                currentPiece.Reset();

                for (int i = 0; i < 2; i++)
                {
                    currentPiece.Move(1, 0);

                    if(!PieceFits())
                    {
                        currentPiece.Move(-1, 0);
                    }
                }
            }
        }

        public GameBoard GameBoard { get; }
        public PiecePicker PiecePicker { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public Piece HeldPiece { get; private set; }
        public bool CanHold { get; private set; }

        public StateInfo()
        {
            GameBoard = new GameBoard();
            PiecePicker = new PiecePicker();
            currentPiece = PiecePicker.NewPiece();
            CanHold = true;
        }

        private bool PieceFits()
        {
            foreach(Position p in CurrentPiece.TilePositions())
            {
                if (!GameBoard.IsEmpty(p.row, p.column))
                {
                    return false;
                }
            }
            return true;
        }

        public void HoldPiece()
        {
            if (!CanHold)
            {
                return;
            }
            if (HeldPiece == null)
            {
                HeldPiece = CurrentPiece;
                CurrentPiece = PiecePicker.NewPiece();
            }
            else
            {
                Piece tmp = CurrentPiece;
                CurrentPiece = HeldPiece;
                HeldPiece= tmp;
            }

            CanHold = false;
        }

        public void RotatePieceClock()
        {
            CurrentPiece.RotateClockwise();

            if (!PieceFits())
            {
                CurrentPiece.RotateAntiClockwise();
            }
        }

        public void RotatePieceAntiClock()
        {
            CurrentPiece.RotateAntiClockwise();

            if (!PieceFits())
            {
                CurrentPiece.RotateClockwise();
            }
        }

        private bool IsGameEnd()
        {
            return !(GameBoard.IsRowEmpty(0) && GameBoard.IsRowEmpty(1));
        }

        private void PlacePiece()
        {
            foreach(Position p in CurrentPiece.TilePositions())
            {
                GameBoard[p.row, p.column] = CurrentPiece.Id;
            }

            Score += GameBoard.ClearRows();

            if (IsGameEnd())
            {
                GameOver = true;
            }
            else
            {
                CurrentPiece = PiecePicker.NewPiece();
                CanHold = true;
            }
        }

        public void MovePieceDown()
        {
            CurrentPiece.Move(1, 0);

            if (!PieceFits())
            {
                CurrentPiece.Move(-1, 0);
                PlacePiece();
            }
        }

        public void MovePieceLeft()
        {
            CurrentPiece.Move(0, -1);

            if (!PieceFits())
            {
                CurrentPiece.Move(0, 1);
            }
        }

        public void MovePieceRight()
        {
            CurrentPiece.Move(0, 1);

            if (!PieceFits())
            {
                CurrentPiece.Move(0, -1);
            }
        }

        private int TileDropDistance(Position p)
        {
            int drop = 0;

            while (GameBoard.IsEmpty(p.row + drop + 1, p.column))
            {
                drop++;
            }

            return drop;
        }

        public int PieceDropDistance()
        {
            int drop = GameBoard.Rows;

            foreach(Position p in CurrentPiece.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }

            return drop;
        }

        public void DropPiece()
        {
            CurrentPiece.Move(PieceDropDistance(), 0);
            PlacePiece();
        }
    }
}
