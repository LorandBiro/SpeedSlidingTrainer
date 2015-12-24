using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SpeedSlidingTrainer.Core.Model.State.Validation;

namespace SpeedSlidingTrainer.CoreTests
{
    [TestClass]
    public class BoardTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ValuesNull_ThrowsException()
        {
            // Arrange
            BoardValidationError[] errors;
            int[] values = null;

            // Act
            // ReSharper disable once AssignNullToNotNullAttribute
            BoardValidator.Validate(4, 4, values, ValidationType.BoardState, out errors);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void BoardTooLittle_ThrowsException()
        {
            // Arrange
            BoardValidationError[] errors;
            int[] values = { 0, 0, 0, 1, 2 };

            // Act
            BoardValidator.Validate(1, 5, values, ValidationType.BoardState, out errors);
        }

        [TestMethod]
        public void TooBigValue()
        {
            // Arrange
            BoardValidationError[] errors;
            int[] values = { 1, 2, 3, 4, 5, 6, 9, 8, 0 };

            // Act
            bool isValid = BoardValidator.Validate(3, 3, values, ValidationType.BoardState, out errors);

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
            int[] values = { 1, 2, 3, 4, -5, 6, 7, 8, 0 };

            // Act
            bool isValid = BoardValidator.Validate(3, 3, values, ValidationType.BoardState, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.ValueOutOfRange, errors[0].ErrorType);
            Assert.IsNotNull(errors[0].Position);
            Assert.AreEqual(1, errors[0].Position.X);
            Assert.AreEqual(1, errors[0].Position.Y);
        }

        [TestMethod]
        public void Duplication()
        {
            // Arrange
            BoardValidationError[] errors;
            int[] values = { 1, 2, 3, 1, 5, 6, 7, 8, 0 };

            // Act
            bool isValid = BoardValidator.Validate(3, 3, values, ValidationType.BoardState, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.Duplication, errors[0].ErrorType);
            Assert.IsNotNull(errors[0].Position);
            Assert.AreEqual(0, errors[0].Position.X);
            Assert.AreEqual(1, errors[0].Position.Y);
        }

        [TestMethod]
        public void NotSolvable()
        {
            // Arrange
            BoardValidationError[] errors;
            int[] values = { 0, 7, 2, 1, 4, 6, 3, 5 };

            // Act
            bool isValid = BoardValidator.Validate(4, 2, values, ValidationType.BoardState, out errors);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreEqual(1, errors.Length);
            Assert.AreEqual(BoardValidationErrorType.NotSolvable, errors[0].ErrorType);
            Assert.IsNull(errors[0].Position);
        }

        // [TestMethod]
        // public void SlideLeft()
        // {
        //    Board board = new Board(new[] { 1, 2, 3, 4, 5, 6, 0, 7, 8 });

        // Assert.IsTrue(board.CanSlideLeft);

        // board.SlideLeft();
        //    CollectionAssert.AreEqual(new[,] { { 1, 4, 7 }, { 2, 5, 0 }, { 3, 6, 8 } }, board.State.GetValues());
        //    Assert.IsTrue(board.CanSlideLeft);

        // board.SlideLeft();
        //    CollectionAssert.AreEqual(new[,] { { 1, 4, 7 }, { 2, 5, 8 }, { 3, 6, 0 } }, board.State.GetValues());
        //    Assert.IsFalse(board.CanSlideLeft);
        // }

        // [TestMethod]
        // public void SlideUp()
        // {
        //    Board board = new Board(new[,] { { 1, 4, 7 }, { 2, 5, 8 }, { 0, 3, 6 } });

        // Assert.IsTrue(board.CanSlideUp);

        // board.SlideUp();
        //    CollectionAssert.AreEqual(new[,] { { 1, 4, 7 }, { 2, 5, 8 }, { 3, 0, 6 } }, board.State.GetValues());
        //    Assert.IsTrue(board.CanSlideUp);

        // board.SlideUp();
        //    CollectionAssert.AreEqual(new[,] { { 1, 4, 7 }, { 2, 5, 8 }, { 3, 6, 0 } }, board.State.GetValues());
        //    Assert.IsFalse(board.CanSlideUp);
        // }

        // [TestMethod]
        // public void SlideRight()
        // {
        //    Board board = new Board(new[,] { { 1, 4, 7 }, { 2, 5, 8 }, { 3, 6, 0 } });

        // Assert.IsTrue(board.CanSlideRight);

        // board.SlideRight();
        //    CollectionAssert.AreEqual(new[,] { { 1, 4, 7 }, { 2, 5, 0 }, { 3, 6, 8 } }, board.State.GetValues());
        //    Assert.IsTrue(board.CanSlideRight);

        // board.SlideRight();
        //    CollectionAssert.AreEqual(new[,] { { 1, 4, 0 }, { 2, 5, 7 }, { 3, 6, 8 } }, board.State.GetValues());
        //    Assert.IsFalse(board.CanSlideRight);
        // }

        // [TestMethod]
        // public void SlideDown()
        // {
        //    Board board = new Board(new[,] { { 1, 4, 7 }, { 2, 5, 8 }, { 3, 6, 0 } });

        // Assert.IsTrue(board.CanSlideDown);

        // board.SlideDown();
        //    CollectionAssert.AreEqual(new[,] { { 1, 4, 7 }, { 2, 5, 8 }, { 3, 0, 6 } }, board.State.GetValues());
        //    Assert.IsTrue(board.CanSlideDown);

        // board.SlideDown();
        //    CollectionAssert.AreEqual(new[,] { { 1, 4, 7 }, { 2, 5, 8 }, { 0, 3, 6 } }, board.State.GetValues());
        //    Assert.IsFalse(board.CanSlideDown);
        // }

        // [TestMethod]
        // public void Incomplete()
        // {
        //    // Arrange
        //    Board board = new Board(new[,] { { 1, 4, 7 }, { 2, 6, 5 }, { 3, 8, 0 } });

        // // Assert
        //    Assert.IsFalse(board.IsComplete);
        // }

        // [TestMethod]
        // public void Complete()
        // {
        //    // Arrange
        //    Board board = new Board(new[,] { { 1, 4, 7 }, { 2, 5, 8 }, { 3, 6, 0 } });

        // // Assert
        //    Assert.IsTrue(board.IsComplete);
        // }
    }
}
