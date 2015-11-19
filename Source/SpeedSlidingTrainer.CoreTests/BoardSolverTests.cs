using System;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedSlidingTrainer.Core.Model;
using SpeedSlidingTrainer.Core.Model.State;
using SpeedSlidingTrainer.Core.Services.BoardGenerator;
using SpeedSlidingTrainer.Core.Services.BoardSolver;

namespace SpeedSlidingTrainer.CoreTests
{
    [TestClass]
    public class BoardSolverTests
    {
        [TestMethod]
        public void Test1()
        {
            // Arrange
            BoardState state = new BoardState(3, 3, new[] { 1, 2, 0, 4, 5, 3, 7, 8, 6 });
            BoardGoal goal = BoardGoal.CreateCompleted(3, 3);
            IBoardSolverService solver = new BoardSolverService();

            // Act
            Step[] steps = solver.GetSolution(state, goal, CancellationToken.None);

            // Assert
            CollectionAssert.AreEqual(new[] { Step.Up, Step.Up }, steps);
        }

        [TestMethod]
        public void RandomTests()
        {
            BoardTemplate template = BoardTemplate.CreateEmpty(3, 3);
            BoardGoal goal = BoardGoal.CreateCompleted(3, 3);

            IBoardGeneratorService generator = new BoardGeneratorService();
            IBoardSolverService solver = new BoardSolverService();

            for (int i = 0; i < 100; i++)
            {
                BoardState state = generator.Generate(template);
                Step[] solution = solver.GetSolution(state, goal, CancellationToken.None);
                foreach (Step step in solution)
                {
                    switch (step)
                    {
                        case Step.Left:
                            state = state.SlideLeft();
                            break;
                        case Step.Up:
                            state = state.SlideUp();
                            break;
                        case Step.Right:
                            state = state.SlideRight();
                            break;
                        case Step.Down:
                            state = state.SlideDown();
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }

                Assert.IsTrue(state.Satisfies(goal));
            }
        }
    }
}
