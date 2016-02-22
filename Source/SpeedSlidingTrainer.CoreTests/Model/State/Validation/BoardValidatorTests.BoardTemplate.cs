using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.CoreTests.Model.State.Validation
{
    public partial class BoardValidatorTests
    {
        [TestMethod]
        public void BoardTemplate_TwoUnspecifiedValue_IsInvalid()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(3, 3, new[] { 1, 2, 3, 4, 5, 6, 7, 0, 0 }, ValidationType.BoardTemplate, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.NotEnoughUnspecifiedTiles, errors[0].ErrorType);
            Assert.IsNull(errors[0].Position);
        }
    }
}
