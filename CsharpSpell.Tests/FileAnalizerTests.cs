using System.Collections.Concurrent;
using System.Collections.Generic;
using CsharpSpell.Entities;
using CsharpSpell.Logging;
using CsharpSpell.Parsing;
using Moq;
using NUnit.Framework;
using SharpTestsEx;

namespace CsharpSpell.Tests
{
    [TestFixture]
    public class FileAnalizerTests
    {
        [TestCase("TestFiles/WithNotClosedComments/incorrectFile1.txt")]
        [TestCase("TestFiles/WithNotClosedComments/incorrectFile2.txt")]
        [TestCase("TestFiles/WithNotClosedComments/incorrectFile3.txt")]
        public void Analize_IncorrectFiles_LogErrorMessage(string testFileName)
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var words = new BlockingCollection<Word>();
            var analizer = new FileAnalizer(testFileName, words, loggerMock.Object);
            
            // Act
            analizer.Analize();

            // Assert
            loggerMock.Verify(logger => logger.Log(It.IsAny<string>()), Times.Once());
        }

        [TestCase("TestFiles/CorrectFiles/MultilineComment1.txt", 3)]
        [TestCase("TestFiles/CorrectFiles/MultilineComment2.txt", 4)]
        [TestCase("TestFiles/CorrectFiles/MultilineComment3.txt", 6)]
        [TestCase("TestFiles/CorrectFiles/MultilineComment4.txt", 2)]
        [TestCase("TestFiles/CorrectFiles/MultilineComment5.txt", 7)]
        [TestCase("TestFiles/CorrectFiles/SinglelineComment1.txt", 6)]
        [TestCase("TestFiles/CorrectFiles/SinglelineComment2.txt", 10)]
        public void Analize_CommentsAnalizing_FindsCorrectNumberOfWordsInTheComments(string testFileName, int correctNumber)
        {
            // Arrange
            var loggerMock = new Mock<ILogger>();
            var words = new BlockingCollection<Word>();
            var analizer = new FileAnalizer(testFileName, words, loggerMock.Object);

            // Act
            analizer.Analize();

            // Assert
            words.Count.Should().Be.EqualTo(correctNumber);
        }

        [Test]
        public void Analize_WordsAnalizing_FindsCorrectWords()
        {
            // Arrange
            const string testFileName = "TestFiles/CorrectFiles/MultilineCommentWordsDetails.txt";
            var loggerMock = new Mock<ILogger>();
            var words = new BlockingCollection<Word>();
            var expectedWords = new List<Word>
                {
                    new Word {Row = 2, Column = 3, Content = "first"},
                    new Word {Row = 2, Column = 9, Content = "string"},
                    new Word {Row = 3, Column = 3, Content = "multi"},
                    new Word {Row = 3, Column = 9, Content = "line"},
                    new Word {Row = 4, Column = 3, Content = "comment"}
                };
            var analizer = new FileAnalizer(testFileName, words, loggerMock.Object);

            // Act
            analizer.Analize();

            // Assert
            words.Should().Have.SameSequenceAs(expectedWords);
        }
    }
}