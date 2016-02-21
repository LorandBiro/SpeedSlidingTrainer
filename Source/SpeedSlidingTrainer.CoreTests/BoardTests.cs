using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpeedSlidingTrainer.CoreTests
{
    [TestClass]
    public class BoardTests
    {
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
