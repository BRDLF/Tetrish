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
        private const int maxDelay = 1000;
        private const int minDelay = 100;
        private const int delayDecrease = 50;

        private StateInfo stateInfo = new StateInfo();
        MainWindow()
        {
            InitializeComponent();
            imageControls = SetupGameCanvas(stateInfo.GameBoard);
        }

        private Image[,] SetupGameCanvas(GameBoard grid)
        {
            Image[,] imageControls = new Image[grid.ROWS, grid.COLS];
            int cellSize = 16;

            for (int r = 0; r < grid.ROWS; r++)
            {
                for (int c = 0; c < grid.COLS; c++) 
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

        //DRAW METHODS
        private void Draw(StateInfo stateInfo)
        {
            DrawGrid(stateInfo.GameBoard);
            DrawGhostPiece(stateInfo.CurrentPiece);
            DrawPiece(stateInfo.CurrentPiece);
            DrawNextPiece(stateInfo.PiecePicker);
            DrawHeld(stateInfo.HeldPiece);
            ScoreCounter.Text = $"{stateInfo.Score}";
            LevelCounter.Text = $"{stateInfo.Level}";
        }
        private void DrawGrid(GameBoard grid)
        {
            for (int r = 0; r < grid.ROWS; r++)
            {
                for (int c = 0; c < grid.COLS; c++)
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
            if(stateInfo.stateMode == StateInfo.StateMode.Playing || stateInfo.stateMode == StateInfo.StateMode.Paused)
            {
                Piece next = piecePicker.NextPiece;
                NextImage.Source = pieceSources[next.Id];
            }
            else
            {
                NextImage.Source = pieceSources[0];
            }
        }
        private void DrawHeld(Piece? heldPiece)
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

            while (true)
            {
                
                switch (stateInfo.stateMode)
                {
                    case StateInfo.StateMode.Paused:
                        await Task.Delay(1000);
                        break;
                    case StateInfo.StateMode.Playing:
                        int delay = Math.Max(minDelay, maxDelay - ((stateInfo.Level - 1) * delayDecrease));
                        stateInfo.MovePieceDown();
                        Draw(stateInfo);
                        await Task.Delay(delay);
                        break;
                    case StateInfo.StateMode.Menu:
                        await Task.Delay(1000);
                        Draw(stateInfo);
                        break;
                    case StateInfo.StateMode.GameOver:
                        FinalScore.Text = $"Score: {stateInfo.Score}";
                        GameOverScreen.Visibility = Visibility.Visible;
                        await Task.Delay(1000);
                        Draw(stateInfo);
                        break;
                }
            }

            
        }


        //EVENT HANDLERS
        
        private void Restart_ButtonClick(object sender, RoutedEventArgs e)
        {
            GameOverScreen.Visibility = Visibility.Hidden;
            PauseScreen.Visibility = Visibility.Hidden;
            stateInfo.Restart();
            stateInfo.Newgame();
        }
        private void Menu_ButtonClick(object sender, RoutedEventArgs e)
        {
            PauseScreen.Visibility = Visibility.Hidden;
            GameOverScreen.Visibility = Visibility.Hidden;
            MenuScreen.Visibility = Visibility.Visible;
            stateInfo.Restart(); 
            stateInfo.ToMenu();
            Draw(stateInfo); 
        }
        private void Resume_ButtonClick(object sender, RoutedEventArgs e)
        {
            PauseScreen.Visibility = Visibility.Hidden; 
            stateInfo.Resume();
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (stateInfo.stateMode != StateInfo.StateMode.Playing)
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
                case Key.X:
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
                case Key.Escape:
                    stateInfo.Pause();
                    PauseScreen.Visibility= Visibility.Visible;
                    break;
                default:
                    return;
            }

            Draw(stateInfo);
        }
        private void Start_ButtonClick(object sender, RoutedEventArgs e)
        {
            stateInfo.Restart();
            stateInfo.Newgame();
            MenuScreen.Visibility= Visibility.Hidden;
        }

        private async void Game_Loaded(object sender, RoutedEventArgs e)
        {
            await GameLoop();
        }
    }
}
