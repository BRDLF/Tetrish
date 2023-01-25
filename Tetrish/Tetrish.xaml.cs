using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Tetrish
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ImageSource[] tileSources = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/Empty.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Red.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Orange.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Yellow.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Green.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Blue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/DBlue.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Purple.png", UriKind.Relative)),
        };

        private readonly ImageSource[] pieceSources = new ImageSource[]
        {
            new BitmapImage(new Uri("Assets/None-Piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Line-Piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/RBend-Piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/LBend-Piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Square-Piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Plus-Piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Zig-Piece.png", UriKind.Relative)),
            new BitmapImage(new Uri("Assets/Zag-Piece.png", UriKind.Relative)),
        };

        private readonly Image[,] imageControls;
        private readonly int maxDelay = 1000;
        private readonly int minDelay = 75;
        private readonly int delayDecrease = 25;

        private StateInfo stateInfo = new StateInfo();
        MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(stateInfo.GameBoard);
        }

        private Image[,] SetupGameCanvas(GameBoard grid)
        {
            Image[,] imageControls = new Image[grid.Rows, grid.Columns];
            int cellSize = 16;

            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++) 
                {
                    Image imageControl = new Image
                    {
                        Width = cellSize,
                        Height = cellSize,
                    };
                    Canvas.SetTop(imageControl, (r - 2) * cellSize + 6);
                    Canvas.SetLeft(imageControl, c * cellSize);
                    GameBoard.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }

            return imageControls;
        }

        private void DrawGrid(GameBoard grid)
        {
            for (int r = 0; r < grid.Rows; r++)
            {
                for (int c = 0; c < grid.Columns; c++)
                {
                    int id = grid[r, c];
                    imageControls[r, c].Opacity = 1;
                    imageControls[r, c].Source = tileSources[id];
                }
            }
        }
        private void DrawPiece(Piece piece)
        {
            foreach (Position p in piece.TilePositions())
            {
                imageControls[p.row, p.column].Opacity = 1;
                imageControls[p.row, p.column].Source = tileSources[piece.Id];
            }
        }

        private void DrawGhostPiece(Piece piece)
        {
            int dropDistance = stateInfo.PieceDropDistance();

            foreach(Position p in piece.TilePositions() )
            {
                imageControls[p.row + dropDistance, p.column].Opacity= 0.5;
                imageControls[p.row + dropDistance, p.column].Source= tileSources[piece.Id];
            }
        }

        private void DrawNextPiece(PiecePicker piecePicker)
        {
            Piece next = piecePicker.NextPiece;
            NextImage.Source = pieceSources[next.Id];
        }

        private void Draw(StateInfo stateInfo)
        {
            DrawGrid(stateInfo.GameBoard);
            DrawGhostPiece(stateInfo.CurrentPiece);
            DrawPiece(stateInfo.CurrentPiece);
            DrawNextPiece(stateInfo.PiecePicker);
            DrawHeld(stateInfo.HeldPiece);
            ScoreCounter.Text = $"{stateInfo.Score}";
        }

        private void DrawHeld(Piece heldPiece)
        {
            if (heldPiece == null)
            {
                HoldImage.Source = pieceSources[0];
            }
            else
            {
                HoldImage.Source = pieceSources[heldPiece.Id];
            }
            
        }

        private async Task GameLoop()
        {
            Draw(stateInfo);

            while (!stateInfo.GameOver)
            {
                int delay = Math.Max(minDelay, maxDelay - (stateInfo.Score * delayDecrease));
                await Task.Delay(delay);
                stateInfo.MovePieceDown();
                Draw(stateInfo);
            }

            GameOverScreen.Visibility = Visibility.Visible;
            FinalScore.Text = $"Score: {stateInfo.Score}";
        }

        private async void PlayAgain_ButtonClick(object sender, RoutedEventArgs e)
        {
            stateInfo = new StateInfo();
            GameOverScreen.Visibility = Visibility.Hidden;
            await GameLoop();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (stateInfo.GameOver)
            {
                return;
            }


            switch (e.Key)
            {
                case Key.A:
                case Key.Left:
                    stateInfo.MovePieceLeft();
                    break;
                case Key.D:
                case Key.Right:
                    stateInfo.MovePieceRight();
                    break;
                case Key.E:
                case Key.Up:
                    stateInfo.RotatePieceClock();
                    break;
                case Key.S:
                case Key.Down:
                    stateInfo.MovePieceDown();
                    break;
                case Key.Q:
                case Key.Z:
                    stateInfo.RotatePieceAntiClock();
                    break;
                case Key.C:
                    stateInfo.HoldPiece();
                    break;
                case Key.Space:
                    stateInfo.DropPiece();
                    break;
                default:
                    return;
            }

            Draw(stateInfo);
        }

        private async void GameBoard_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }
    }
}
