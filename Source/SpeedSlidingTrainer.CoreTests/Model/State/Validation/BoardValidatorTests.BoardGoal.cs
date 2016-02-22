using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.CoreTests.Model.State.Validation
{
    public partial class BoardValidatorTests
    {
        [TestMethod]
        public void BoardGoal_AllUnspecified_IsNotSolvable()
        {
            // There must be at least 1 specific value in a board goal, otherwise it's an unsolvable (or trivially completed) goal.

            // x x
            // x x
            BoardGoalAssert.IsNotSolvable(2, 2, new[] { 0, 0, 0, 0 });

            // x x x
            // x x x
            BoardGoalAssert.IsNotSolvable(3, 2, new[] { 0, 0, 0, 0, 0, 0 });

            // x x x
            // x x x
            // x x x
            BoardGoalAssert.IsNotSolvable(3, 3, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
        }

        [TestMethod]
        public void BoardGoal_ThreeOrMoreUnspecified_IsValid()
        {
            // A board goal with 3 or more unspecified value is always solvable, these can be completed even from an illegal board state.

            // 1 x
            // x x
            BoardGoalAssert.IsValid(2, 2, new[] { 1, 0, 0, 0 });

            // 1 4 5
            // x x x
            BoardGoalAssert.IsValid(3, 2, new[] { 1, 4, 5, 0, 0, 0 });

            // 1 x x
            // x x 2
            BoardGoalAssert.IsValid(3, 2, new[] { 1, 0, 0, 0, 0, 2 });

            // 1 2 3
            // 4 x x
            // 7 x 8
            BoardGoalAssert.IsValid(3, 3, new[] { 1, 2, 3, 4, 0, 0, 7, 0, 8 });

            // 1 2 3
            // x x x
            // x x x
            BoardGoalAssert.IsValid(3, 3, new[] { 1, 2, 3, 0, 0, 0, 0, 0, 0 });
        }

        [TestMethod]
        public void BoardGoal_TwoUnspecified_IsNotSolvable()
        {
            // The solvability of a board goal with 2 unspecified value from a legal state requires further study. For now they are considered as unsolvable.

            // 1 2
            // x x
            BoardGoalAssert.IsNotSolvable(2, 2, new[] { 1, 2, 0, 0 }); // This is clearly solvable by the way.

            // 1 x
            // 2 x
            BoardGoalAssert.IsNotSolvable(2, 2, new[] { 1, 0, 2, 0 });

            // 1 2 3
            // 4 x x
            BoardGoalAssert.IsNotSolvable(3, 2, new[] { 1, 2, 3, 4, 0, 0 });

            // 1 2 3
            // 4 5 x
            // 7 8 x
            BoardGoalAssert.IsNotSolvable(3, 3, new[] { 1, 2, 3, 4, 5, 0, 7, 8, 0 });
        }

        [TestMethod]
        public void BoardGoal_OneUnspecified()
        {
            // A board goal with 1 unspecified value perfectly defines the goal state (the unspecified value being the eye). The only question is the
            // solvability/legality of the state.

            // 1 2
            // 3
            BoardGoalAssert.IsValid(2, 2, new[] { 1, 2, 3, 0 });

            // 1 3
            //   2
            BoardGoalAssert.IsNotSolvable(2, 2, new[] { 1, 3, 0, 2 });

            // 1 2 3
            //   4 5
            BoardGoalAssert.IsValid(3, 2, new[] { 1, 2, 3, 0, 4, 5 });

            // 1 2 3
            //   5 4
            BoardGoalAssert.IsNotSolvable(3, 2, new[] { 1, 2, 3, 0, 5, 4 });

            // 1 2
            // 4 5 3
            // 7 8 6
            BoardGoalAssert.IsValid(3, 3, new[] { 1, 2, 0, 4, 5, 3, 7, 8, 6 });

            // 1 2 3
            // 4 5 8
            // 7 6
            BoardGoalAssert.IsNotSolvable(3, 3, new[] { 1, 2, 3, 4, 5, 8, 7, 6, 0 });
        }

        private static class BoardGoalAssert
        {
            public static void IsNotSolvable(int width, int height, int[] values)
            {
                // Arrange
                BoardValidationError[] errors;

                // Act
                bool isValid = BoardValidator.Validate(width, height, values, ValidationType.BoardGoal, out errors);

                // Assert
                Assert.IsFalse(isValid);
                Assert.AreEqual(1, errors.Length);
                Assert.AreEqual(BoardValidationErrorType.NotSolvable, errors[0].ErrorType);
                Assert.IsNull(errors[0].Position);
            }

            public static void IsValid(int width, int height, int[] values)
            {
                // Arrange
                BoardValidationError[] errors;

                // Act
                bool isValid = BoardValidator.Validate(width, height, values, ValidationType.BoardGoal, out errors);

                // Assert
                Assert.IsTrue(isValid);
            }
        }
    }
}
