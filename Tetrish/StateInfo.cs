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

                    if (!PieceFits())
                    {
                        currentPiece.Move(-1, 0);
                    }
                }
            }
        }
        public enum StateMode
        {
            Menu = 0,
            Playing = 1,
            Paused = 2,
            GameOver = 3
        }
        public StateMode stateMode { get; private set; }

        public GameBoard GameBoard { get; private set; }
        public PiecePicker PiecePicker { get; private set; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public int Level { get; private set; }
        private int LinesCleared;
        public Piece? HeldPiece { get; private set; }
        public bool CanHold { get; private set; }

        public StateInfo()
        {
            currentPiece = new BlankPiece();
            GameBoard = new GameBoard();
            PiecePicker = new PiecePicker();
            stateMode = StateMode.Menu;
        }

        public void ToMenu()
        {
            GameOver = false;
            stateMode = StateMode.Menu;
        }

        public void Restart()
        {
            currentPiece = new BlankPiece();
            GameBoard = new GameBoard();
            PiecePicker = new PiecePicker();
            LinesCleared = 0;
            Score = 0;
            Level = 1; 
            HeldPiece = null;
            CanHold = true;
        }

        public void Newgame()
        {
            currentPiece = PiecePicker.NewPiece();
            stateMode = StateMode.Playing;
            GameOver = false;
        }

        public void Resume()
        {
            stateMode = StateMode.Playing;
        }

        public void Pause()
        {
            stateMode = StateMode.Paused;
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
        private void PlacePiece()
        {
            foreach (Position p in CurrentPiece.TilePositions())
            {
                GameBoard[p.row, p.column] = CurrentPiece.Id;
            }

            Score += CalculateScore();
            if (LinesCleared >= 10)
            {
                LevelUp();
            }


            if (IsGameEnd())
            {
                stateMode = StateMode.GameOver;
                GameOver = true;
            }
            else
            {
                CurrentPiece = PiecePicker.NewPiece();
                CanHold = true;
            }
        }
        private bool IsGameEnd()
        {
            return !(GameBoard.IsRowEmpty(0) && GameBoard.IsRowEmpty(1));
        }
        
        private void LevelUp()
        {
            Level++;
            LinesCleared %= 10;
        }
        private int CalculateScore()
        {
            int points = 0;
            int Cleared = GameBoard.ClearRows();
            LinesCleared += Cleared;
            switch (Cleared)
            {
                case 4: 
                    points = 1200;
                    break;
                case 3:
                    points = 300;
                    break;
                case 2:
                    points = 100;
                    break;
                case 1:
                    points = 40;
                    break;
            }
            return points * Level;
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
        public void DropPiece()
        {
            CurrentPiece.Move(PieceDropDistance(), 0);
            PlacePiece();
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
            int drop = GameBoard.ROWS;

            foreach(Position p in CurrentPiece.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }

            return drop;
        }

        
    }
}
