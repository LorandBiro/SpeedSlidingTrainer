using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedSlidingTrainer.Core.Model.State.Validation;
using SpeedSlidingTrainer.CoreTests.Utils;

namespace SpeedSlidingTrainer.CoreTests.Model.State.Validation
{
    [TestClass]
    public partial class BoardValidatorTests
    {
        [TestMethod]
        public void Common_ValuesIsNull_ThrowsException()
        {
            ValuesIsNull_ThrowsException(ValidationType.BoardGoal);
            ValuesIsNull_ThrowsException(ValidationType.BoardState);
            ValuesIsNull_ThrowsException(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void Common_HeightLessThanTwo_ThrowsException()
        {
            HeightLessThanTwo_ThrowsException(ValidationType.BoardGoal);
            HeightLessThanTwo_ThrowsException(ValidationType.BoardState);
            HeightLessThanTwo_ThrowsException(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void Common_WidthLessThanTwo_ThrowsException()
        {
            WidthLessThanTwo_ThrowsException(ValidationType.BoardGoal);
            WidthLessThanTwo_ThrowsException(ValidationType.BoardState);
            WidthLessThanTwo_ThrowsException(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void Common_TooBigValue_IsInvalid()
        {
            TooBigValue_IsInvalid(ValidationType.BoardGoal);
            TooBigValue_IsInvalid(ValidationType.BoardState);
            TooBigValue_IsInvalid(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void Common_NegativeValue_IsInvalid()
        {
            NegativeValue_IsInvalid(ValidationType.BoardGoal);
            NegativeValue_IsInvalid(ValidationType.BoardState);
            NegativeValue_IsInvalid(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void Common_Duplication_IsInvalid()
        {
            Duplication_IsInvalid(ValidationType.BoardGoal);
            Duplication_IsInvalid(ValidationType.BoardState);
            Duplication_IsInvalid(ValidationType.BoardTemplate);
        }

        private static void ValuesIsNull_ThrowsException(ValidationType validationType)
        {
            ExceptionAssert.Throws(
                () =>
                    {
                        // Arrange
                        BoardValidationError[] errors;

                        // Act
                        // ReSharper disable once AssignNullToNotNullAttribute
                        BoardValidator.Validate(3, 3, null, validationType, out errors);
                    },
                typeof(ArgumentNullException));
        }

        private static void HeightLessThanTwo_ThrowsException(ValidationType validationType)
        {
            ExceptionAssert.Throws(
                () =>
                    {
                        // Arrange
                        BoardValidationError[] errors;

                        // Act
                        BoardValidator.Validate(5, 1, new[] { 1, 2, 3, 4, 0 }, validationType, out errors);
                    },
                typeof(ArgumentException));
        }

        private static void WidthLessThanTwo_ThrowsException(ValidationType validationType)
        {
            ExceptionAssert.Throws(
                () =>
                    {
                        // Arrange
                        BoardValidationError[] errors;

                        // Act
                        BoardValidator.Validate(1, 5, new[] { 1, 2, 3, 4, 0 }, validationType, out errors);
                    },
                typeof(ArgumentException));
        }

        private static void TooBigValue_IsInvalid(ValidationType validationType)
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(3, 3, new[] { 1, 2, 3, 4, 5, 6, 7, 99, 0 }, validationType, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(errors.Length > 0);
            Assert.AreEqual(BoardValidationErrorType.ValueOutOfRange, errors[0].ErrorType);
            Assert.IsNotNull(errors[0].Position);
            Assert.AreEqual(1, errors[0].Position.X);
            Assert.AreEqual(2, errors[0].Position.Y);
        }

        private static void NegativeValue_IsInvalid(ValidationType validationType)
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(3, 3, new[] { 1, 2, 3, 4, 5, 6, 7, -99, 0 }, validationType, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(errors.Length > 0);
            Assert.AreEqual(BoardValidationErrorType.ValueOutOfRange, errors[0].ErrorType);
            Assert.IsNotNull(errors[0].Position);
            Assert.AreEqual(1, errors[0].Position.X);
            Assert.AreEqual(2, errors[0].Position.Y);
        }

        private static void Duplication_IsInvalid(ValidationType validationType)
        {
            // Arrange
            BoardValidationError[] errors;

            // Act
            bool isValid = BoardValidator.Validate(3, 3, new[] { 1, 2, 3, 4, 5, 6, 7, 7, 0 }, validationType, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsTrue(errors.Length > 0);
            Assert.AreEqual(BoardValidationErrorType.Duplication, errors[0].ErrorType);
            Assert.IsNotNull(errors[0].Position);
            Assert.AreEqual(1, errors[0].Position.X);
            Assert.AreEqual(2, errors[0].Position.Y);
        }
    }
}
