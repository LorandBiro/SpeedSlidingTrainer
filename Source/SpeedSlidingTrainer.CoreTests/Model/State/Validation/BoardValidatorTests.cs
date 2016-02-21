using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedSlidingTrainer.Core.Model.State.Validation;
using SpeedSlidingTrainer.CoreTests.Utils;

namespace SpeedSlidingTrainer.CoreTests.Model.State.Validation
{
    [TestClass]
    public class BoardValidatorTests
    {
        #region BoardGoal

        [TestMethod]
        public void BoardGoal_ValuesIsNull_ThrowsException()
        {
            ValuesIsNull_ThrowsException(ValidationType.BoardGoal);
        }

        [TestMethod]
        public void BoardGoal_HeightLessThanTwo_ThrowsException()
        {
            HeightLessThanTwo_ThrowsException(ValidationType.BoardGoal);
        }

        [TestMethod]
        public void BoardGoal_WidthLessThanTwo_ThrowsException()
        {
            WidthLessThanTwo_ThrowsException(ValidationType.BoardGoal);
        }

        [TestMethod]
        public void BoardGoal_TooBigValue_IsInvalid()
        {
            TooBigValue_IsInvalid(ValidationType.BoardGoal);
        }

        [TestMethod]
        public void BoardGoal_NegativeValue_IsInvalid()
        {
            NegativeValue_IsInvalid(ValidationType.BoardGoal);
        }

        [TestMethod]
        public void BoardGoal_Duplication_IsInvalid()
        {
            Duplication_IsInvalid(ValidationType.BoardGoal);
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

        #endregion

        #region BoardState

        [TestMethod]
        public void BoardState_ValuesIsNull_ThrowsException()
        {
            ValuesIsNull_ThrowsException(ValidationType.BoardState);
        }

        [TestMethod]
        public void BoardState_HeightLessThanTwo_ThrowsException()
        {
            HeightLessThanTwo_ThrowsException(ValidationType.BoardState);
        }

        [TestMethod]
        public void BoardState_WidthLessThanTwo_ThrowsException()
        {
            WidthLessThanTwo_ThrowsException(ValidationType.BoardState);
        }

        [TestMethod]
        public void BoardState_TooBigValue_IsInvalid()
        {
            TooBigValue_IsInvalid(ValidationType.BoardState);
        }

        [TestMethod]
        public void BoardState_NegativeValue_IsInvalid()
        {
            NegativeValue_IsInvalid(ValidationType.BoardState);
        }

        [TestMethod]
        public void BoardState_Duplication_IsInvalid()
        {
            Duplication_IsInvalid(ValidationType.BoardState);
        }

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

        #endregion

        #region BoardTemplate

        [TestMethod]
        public void BoardTemplate_ValuesIsNull_ThrowsException()
        {
            ValuesIsNull_ThrowsException(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void BoardTemplate_HeightLessThanTwo_ThrowsException()
        {
            HeightLessThanTwo_ThrowsException(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void BoardTemplate_WidthLessThanTwo_ThrowsException()
        {
            WidthLessThanTwo_ThrowsException(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void BoardTemplate_TooBigValue_IsInvalid()
        {
            TooBigValue_IsInvalid(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void BoardTemplate_NegativeValue_IsInvalid()
        {
            NegativeValue_IsInvalid(ValidationType.BoardTemplate);
        }

        [TestMethod]
        public void BoardTemplate_Duplication_IsInvalid()
        {
            Duplication_IsInvalid(ValidationType.BoardTemplate);
        }

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

        #endregion

        #region Common

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

        #endregion
    }
}
