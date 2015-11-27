namespace SpeedSlidingTrainer.CoreTests
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using SpeedSlidingTrainer.Core.Model.State;
    using SpeedSlidingTrainer.Core.Model.State.Validation;

    [TestClass]
    public class BoardGoalTests
    {
        [TestMethod]
        public void ValuesNull_ThrowsException()
        {
            // Arrange
            BoardValidationError[] errors;
            int[] values = { 1, 2, 3, 4, 5, 0, 7, 0, 8 };

            // Act
            bool valid = BoardGoal.Validate(3, 3, values, out errors);

            // Assert
            Assert.IsTrue(valid);
        }
    }
}
