using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.CoreTests.Model.State.Validation
{
    public partial class BoardValidatorTests
    {
        [TestMethod]
        public void BoardGoal_AllUnspecified_IsInvalid()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(3, 3, new[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, ValidationType.BoardGoal, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.NotSolvable, errors[0].ErrorType);
            Assert.IsNull(errors[0].Position);
        }

        [TestMethod]
        public void BoardGoal_TwoUnspecifiedCase_IsValid()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool valid = BoardValidator.Validate(3, 3, new[] { 1, 2, 3, 4, 5, 0, 7, 0, 8 }, ValidationType.BoardGoal, out errors);

            // Assert
            Assert.IsTrue(valid);
        }
    }
}
