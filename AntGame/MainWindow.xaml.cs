using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace AntSimulator
{
    public partial class MainWindow : Window
    {
        private int gridWidth = 40;
        private int gridHeight = 40;
        private const int cellSize = 15;

        private bool[,] grid;
        private Ant ant;
        private DispatcherTimer timer;

        private Rectangle[,] cellRects;
        private Ellipse antEllipse;

        private List<string> blackRules = new List<string>();
        private List<string> whiteRules = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(100);
            timer.Tick += Timer_Tick;

            whiteRules.Add("Повернуть направо");
            whiteRules.Add("Перекрасить клетку");

            blackRules.Add("Повернуть налево");
            blackRules.Add("Перекрасить клетку");

            UpdateRulesUI();
            CreateGrid(gridWidth, gridHeight);
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            MoveAnt();
        }

        private void CreateGrid(int w, int h)
        {
            gridWidth = w;
            gridHeight = h;

            GameCanvas.Children.Clear();

            grid = new bool[gridWidth, gridHeight];
            cellRects = new Rectangle[gridWidth, gridHeight];

            GameCanvas.Width = gridWidth * cellSize;
            GameCanvas.Height = gridHeight * cellSize;

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    Rectangle rect = new Rectangle();
                    rect.Width = cellSize;
                    rect.Height = cellSize;
                    rect.Stroke = Brushes.LightGray;
                    rect.StrokeThickness = 0.5;
                    rect.Fill = Brushes.White;

                    Canvas.SetLeft(rect, x * cellSize);
                    Canvas.SetTop(rect, y * cellSize);

                    GameCanvas.Children.Add(rect);
                    cellRects[x, y] = rect;
                }
            }

            antEllipse = new Ellipse();
            antEllipse.Width = cellSize - 4;
            antEllipse.Height = cellSize - 4;
            antEllipse.Fill = Brushes.Red;

            GameCanvas.Children.Add(antEllipse);
            ResetGame();
        }

        private void ResetGame()
        {
            timer.Stop();
            BtnStart.IsEnabled = true;
            BtnPause.IsEnabled = false;
            LblStatus.Text = "Статус: Остановлен";

            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    grid[x, y] = false;
                    cellRects[x, y].Fill = Brushes.White;
                }
            }

            ant = new Ant(gridWidth / 2, gridHeight / 2);
            UpdateAntVisual();
        }

        private void MoveAnt()
        {
            int x = ant.X;
            int y = ant.Y;
            bool isBlack = grid[x, y];

            List<string> currentRules;
            if (isBlack == true)
            {
                currentRules = blackRules;
            }
            else
            {
                currentRules = whiteRules;
            }

            if (currentRules.Count == 0)
            {
                return;
            }

            string action = currentRules[0];

            if (action == "Повернуть налево")
            {
                ant.TurnLeft();
            }
            else if (action == "Повернуть направо")
            {
                ant.TurnRight();
            }
            else if (action == "Перекрасить клетку")
            {
                if (grid[x, y] == true)
                {
                    grid[x, y] = false;
                }
                else
                {
                    grid[x, y] = true;
                }

                if (grid[x, y] == true)
                {
                    cellRects[x, y].Fill = Brushes.Black;
                }
                else
                {
                    cellRects[x, y].Fill = Brushes.White;
                }
            }

            currentRules.RemoveAt(0);
            currentRules.Add(action);

            ant.MoveForward();

            if (ant.X < 0)
            {
                ant.X = gridWidth - 1;
            }
            if (ant.X >= gridWidth)
            {
                ant.X = 0;
            }
            if (ant.Y < 0)
            {
                ant.Y = gridHeight - 1;
            }
            if (ant.Y >= gridHeight)
            {
                ant.Y = 0;
            }

            UpdateAntVisual();
        }

        private void UpdateAntVisual()
        {
            Canvas.SetLeft(antEllipse, ant.X * cellSize + 2);
            Canvas.SetTop(antEllipse, ant.Y * cellSize + 2);
        }

        private void BtnResize_Click(object sender, RoutedEventArgs e)
        {
            int w;
            int h;

            if (int.TryParse(TxtWidth.Text, out w) == false)
            {
                MessageBox.Show("Введите целое число для ширины.");
                return;
            }
            if (int.TryParse(TxtHeight.Text, out h) == false)
            {
                MessageBox.Show("Введите целое число для высоты.");
                return;
            }

            if (w < 5) { w = 5; }
            if (w > 100) { w = 100; }
            if (h < 5) { h = 5; }
            if (h > 100) { h = 100; }

            TxtWidth.Text = w.ToString();
            TxtHeight.Text = h.ToString();

            CreateGrid(w, h);
        }

        private void BtnAddBlackRule_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)CmbBlackRules.SelectedItem;
            if (item != null)
            {
                string text = item.Content.ToString();
                blackRules.Add(text);
                UpdateRulesUI();
            }
        }

        private void BtnAddWhiteRule_Click(object sender, RoutedEventArgs e)
        {
            ComboBoxItem item = (ComboBoxItem)CmbWhiteRules.SelectedItem;
            if (item != null)
            {
                string text = item.Content.ToString();
                whiteRules.Add(text);
                UpdateRulesUI();
            }
        }

        private void BtnRemoveBlackRule_Click(object sender, RoutedEventArgs e)
        {
            int index = LstBlackRules.SelectedIndex;
            if (index >= 0)
            {
                blackRules.RemoveAt(index);
                UpdateRulesUI();
            }
        }

        private void BtnRemoveWhiteRule_Click(object sender, RoutedEventArgs e)
        {
            int index = LstWhiteRules.SelectedIndex;
            if (index >= 0)
            {
                whiteRules.RemoveAt(index);
                UpdateRulesUI();
            }
        }

        private void UpdateRulesUI()
        {
            LstBlackRules.ItemsSource = null;
            LstBlackRules.ItemsSource = blackRules;

            LstWhiteRules.ItemsSource = null;
            LstWhiteRules.ItemsSource = whiteRules;
        }

        private void BtnStart_Click(object sender, RoutedEventArgs e)
        {
            if (whiteRules.Count == 0 && blackRules.Count == 0)
            {
                MessageBox.Show("Добавьте хотя бы одно правило!");
                return;
            }

            timer.Start();
            BtnStart.IsEnabled = false;
            BtnPause.IsEnabled = true;
            LblStatus.Text = "Статус: Работает";
        }

        private void BtnPause_Click(object sender, RoutedEventArgs e)
        {
            timer.Stop();
            BtnStart.IsEnabled = true;
            BtnPause.IsEnabled = false;
            LblStatus.Text = "Статус: Пауза";
        }

        private void BtnReset_Click(object sender, RoutedEventArgs e)
        {
            ResetGame();
        }
    }

    public class Ant
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Direction Facing { get; set; }

        public Ant(int startX, int startY)
        {
            X = startX;
            Y = startY;
            Facing = Direction.Up;
        }

        public void TurnRight()
        {
            if (Facing == Direction.Up) { Facing = Direction.Right; }
            else if (Facing == Direction.Right) { Facing = Direction.Down; }
            else if (Facing == Direction.Down) { Facing = Direction.Left; }
            else if (Facing == Direction.Left) { Facing = Direction.Up; }
        }

        public void TurnLeft()
        {
            if (Facing == Direction.Up) { Facing = Direction.Left; }
            else if (Facing == Direction.Left) { Facing = Direction.Down; }
            else if (Facing == Direction.Down) { Facing = Direction.Right; }
            else if (Facing == Direction.Right) { Facing = Direction.Up; }
        }

        public void MoveForward()
        {
            if (Facing == Direction.Up) { Y = Y - 1; }
            else if (Facing == Direction.Right) { X = X + 1; }
            else if (Facing == Direction.Down) { Y = Y + 1; }
            else if (Facing == Direction.Left) { X = X - 1; }
        }
    }

    public enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }
}