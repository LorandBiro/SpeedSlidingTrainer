using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using JetBrains.Annotations;
using SpeedSlidingTrainer.Core.Model.State;

namespace SpeedSlidingTrainer.Desktop
{
    public partial class BoardControl
    {
        private static readonly DependencyProperty BoardProperty = DependencyProperty.Register(
            "Board",
            typeof(BoardStateBase),
            typeof(BoardControl),
            new PropertyMetadata(OnBoardChanged));

        private static readonly SolidColorBrush TileFill = new SolidColorBrush(Color.FromRgb(238, 238, 255));

        private static readonly SolidColorBrush TileStroke = new SolidColorBrush(Color.FromRgb(211, 211, 211));

        private readonly List<Tile> tiles = new List<Tile>();

        private int renderedWidth;

        private int renderedHeight;

        public BoardControl()
        {
            this.InitializeComponent();
        }

        public double TileSize { get; set; } = 100.0;

        public double TileMargin { get; set; } = 6.0;

        public BoardStateBase Board
        {
            get { return (BoardStateBase)this.GetValue(BoardProperty); }
            set { this.SetValue(BoardProperty, value); }
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);

            this.UpdateBoard(this.Board);
        }

        private static void OnBoardChanged([NotNull] DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            BoardControl boardControl = (BoardControl)sender;
            if (!boardControl.IsInitialized)
            {
                return;
            }

            BoardStateBase board = (BoardStateBase)e.NewValue;
            boardControl.UpdateBoard(board);
        }

        private void UpdateBoard([CanBeNull] BoardStateBase board)
        {
            if (board == null)
            {
                this.tiles.Clear();
                this.LayoutRoot.Children.Clear();
                this.LayoutRoot.Width = 0;
                this.LayoutRoot.Height = 0;
                this.renderedWidth = 0;
                this.renderedHeight = 0;

                return;
            }

            if (board.Width != this.renderedWidth || board.Height != this.renderedHeight)
            {
                this.tiles.Clear();
                this.LayoutRoot.Children.Clear();
                for (int i = 0; i < board.Width * board.Height; i++)
                {
                    Tile tile = this.CreateTile(i);
                    this.LayoutRoot.Children.Add(tile.TileGrid);
                    this.tiles.Add(tile);
                }

                this.renderedWidth = board.Width;
                this.renderedHeight = board.Height;

                this.LayoutRoot.Width = (board.Width * this.TileSize) + ((board.Width - 1) * this.TileMargin);
                this.LayoutRoot.Height = (board.Height * this.TileSize) + ((board.Height - 1) * this.TileMargin);
            }

            this.tiles.ForEach(x => x.SouldHide = true);
            for (int x = 0; x < board.Width; x++)
            {
                for (int y = 0; y < board.Height; y++)
                {
                    if (board[x, y] == 0)
                    {
                        continue;
                    }

                    Tile tile = this.tiles[board[x, y]];
                    tile.SouldHide = false;
                    tile.TileGrid.Opacity = 1.0;
                    tile.TranslateTransform.X = x * (this.TileSize + this.TileMargin);
                    tile.TranslateTransform.Y = y * (this.TileSize + this.TileMargin);
                }
            }

            foreach (Tile tileToHide in this.tiles.Where(x => x.SouldHide))
            {
                tileToHide.TileGrid.Opacity = 0.0;
            }
        }

        private Tile CreateTile(int label)
        {
            Rectangle rectangle = new Rectangle { Fill = TileFill, Stroke = TileStroke, StrokeThickness = 1 };

            TextBlock textBlock = new TextBlock
            {
                Text = label.ToString(),
                FontSize = this.FontSize,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            TranslateTransform translateTransform = new TranslateTransform();

            Grid grid = new Grid { Width = this.TileSize, Height = this.TileSize, RenderTransform = translateTransform };
            grid.Children.Add(rectangle);
            grid.Children.Add(textBlock);

            return new Tile { TileGrid = grid, TranslateTransform = translateTransform };
        }

        private class Tile
        {
            public Grid TileGrid { get; set; }

            public TranslateTransform TranslateTransform { get; set; }

            public bool SouldHide { get; set; }
        }
    }
}
