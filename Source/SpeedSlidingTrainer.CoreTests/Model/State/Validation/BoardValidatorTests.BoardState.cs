using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.CoreTests.Model.State.Validation
{
    public partial class BoardValidatorTests
    {
        [TestMethod]
        public void BoardState_NotSolvable_IsInvalid()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(4, 2, new[] { 0, 7, 2, 1, 4, 6, 3, 5 }, ValidationType.BoardState, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.NotSolvable, errors[0].ErrorType);
            Assert.IsNull(errors[0].Position);
        }
    }
}
