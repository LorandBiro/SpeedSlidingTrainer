using SpeedSlidingTrainer.Application.Infrastructure;
using SpeedSlidingTrainer.Application.Repositories;
using SpeedSlidingTrainer.Application.Services.Drills;
using SpeedSlidingTrainer.Application.Services.Game;
using SpeedSlidingTrainer.Application.Services.Solver;
using SpeedSlidingTrainer.Application.Services.Statistics;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Services.BoardGenerator;
using SpeedSlidingTrainer.Core.Services.BoardSolver;
using SpeedSlidingTrainer.Desktop.BoardFormatting;
using SpeedSlidingTrainer.Desktop.Infrastructure;

namespace SpeedSlidingTrainer.Desktop
{
    public static class ServiceLocator
    {
        // Domain Services
        public static IBoardSolverService BoardSolverService { get; } = new BoardSolverService();

        public static IBoardGeneratorService BoardGeneratorService { get; } = new BoardGeneratorService();

        // Application compontents
        public static ITimerFactory TimerFactory { get; } = new TimerAdapterFactory();

        public static IDataStorage DataStorage { get; } = new FileDataStorage();

        public static IDispatcher Dispatcher { get; } = new WpfDispatcher();

        public static IRepository<Drill> DrillRepository { get; } = new DrillRepository(DataStorage);

        // Application services
        public static IGameService GameService { get; } = new GameService(BoardGeneratorService);

        public static ISolverService SolverService { get; } = new SolverService(GameService, BoardSolverService, Dispatcher);

        public static IStatisticsService StatisticsService { get; } = new StatisticsService(GameService, SolverService, TimerFactory);

        public static IDrillService DrillService { get; } = new DrillService(DrillRepository);

        // Presentation compontents
        public static IBoardFormatter BoardFormatter { get; } = new BoardFormatter();
    }
}
