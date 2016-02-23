using System.Windows;
using System.Windows.Input;
using SpeedSlidingTrainer.Application.Services.Game;
using SpeedSlidingTrainer.Application.Services.SessionStatistics;
using SpeedSlidingTrainer.Application.Services.Solver;
using SpeedSlidingTrainer.Application.Services.SolveState;

namespace SpeedSlidingTrainer.Desktop
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            this.DataContext = this;
            this.InitializeComponent();
        }

        public IGameService GameService { get; } = ServiceLocator.GameService;

        public ISolveStateService SolveStateService { get; } = ServiceLocator.SolveStateService;

        public ISolverService SolverService { get; } = ServiceLocator.SolverService;

        public ISessionStatisticsService SessionStatisticsService { get; } = ServiceLocator.SessionStatisticsService;

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.IsRepeat)
            {
                return;
            }

            switch (e.Key)
            {
                case Key.Escape:
                    this.GameService.Reset();
                    break;
                case Key.Space:
                    this.GameService.Scramble();
                    break;
                case Key.S:
                    this.SolverService.StartSolveCurrentBoard();
                    break;
                case Key.Left:
                    this.GameService.SlideLeft();
                    break;
                case Key.Up:
                    this.GameService.SlideUp();
                    break;
                case Key.Right:
                    this.GameService.SlideRight();
                    break;
                case Key.Down:
                    this.GameService.SlideDown();
                    break;
            }
        }

        private void OnResetButtonClick(object sender, RoutedEventArgs e)
        {
            this.GameService.Reset();
        }

        private void OnScrambleButtonClick(object sender, RoutedEventArgs e)
        {
            this.GameService.Scramble();
        }

        private void OnChooseDrillButtonClick(object sender, RoutedEventArgs e)
        {
            DrillBrowser drillBrowser = new DrillBrowser { Owner = this };
            drillBrowser.ShowDialog();
        }

        private void OnSolveButtonClick(object sender, RoutedEventArgs e)
        {
            this.SolverService.StartSolveCurrentBoard();
        }
    }
}
