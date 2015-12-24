using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.CoreTests
{
    [TestClass]
    public class StartDefinitionTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValuesNull_ThrowsException()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            BoardValidator.Validate(3, 3, null, ValidationType.BoardTemplate, out errors);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BoardTooLittle_ThrowsException()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            BoardValidator.Validate(5, 1, new[] { 0, 0, 0, 1, 2 }, ValidationType.BoardTemplate, out errors);
        }

        [TestMethod]
        public void TooBigValue()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(3, 3, new[] { 1, 2, 99, 0, 0, 0, 0, 0, 0 }, ValidationType.BoardTemplate, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.ValueOutOfRange, errors[0].ErrorType);
            Assert.IsNotNull(errors[0].Position);
            Assert.AreEqual(0, errors[0].Position.X);
            Assert.AreEqual(2, errors[0].Position.Y);
        }

        [TestMethod]
        public void TooLittleValue()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(3, 3, new[] { 1, 2, 3, 0, -1, 0, 0, 0, 0 }, ValidationType.BoardTemplate, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.ValueOutOfRange, errors[0].ErrorType);
            Assert.IsNotNull(errors[0].Position);
            Assert.AreEqual(1, errors[0].Position.X);
            Assert.AreEqual(1, errors[0].Position.Y);
        }

        [TestMethod]
        public void TooFewUnspecifiedValues()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(3, 3, new[] { 1, 4, 7, 2, 5, 8, 3, 0, 0 }, ValidationType.BoardTemplate, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.NotEnoughUnspecifiedTiles, errors[0].ErrorType);
            Assert.IsNull(errors[0].Position);
        }

        [TestMethod]
        public void Duplication()
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(3, 3, new[] { 1, 4, 7, 2, 5, 1, 0, 0, 0 }, ValidationType.BoardTemplate, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.Duplication, errors[0].ErrorType);
            Assert.IsNotNull(errors[0].Position);
            Assert.AreEqual(1, errors[0].Position.X);
            Assert.AreEqual(2, errors[0].Position.Y);
        }
    }
}
