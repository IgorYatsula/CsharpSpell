using System.Collections.Concurrent;
using System.Collections.Generic;
using CsharpSpell.Entities;
using CsharpSpell.Logging;
using CsharpSpell.Parsing;
using Moq;
using NUnit.Framework;

namespace CsharpSpell.Tests
{
    [TestFixture]
    public class WordCheckerTests
    {
        [Test]
        public void StartChecking_CheckingWords_ChecksFilesAndLogIncorrectFiles()
        {
            // Arrange
            const string dictionaryPath = "TestFiles/CorrectFiles/testDictionary.txt";
            var existWords = new List<Word>
                                 {
                                     new Word { Row = 1, Column = 1, Content = "existword1" },
                                     new Word { Row = 3, Column = 3, Content = "existword2" }
                                 };
            var unknownWords = new List<Word>
                                   {
                                       new Word { Row = 2, Column = 2, Content = "unknowword1" },
                                       new Word { Row = 4, Column = 4, Content = "unknownword2" }
                                   };
            var loggerMock = new Mock<ILogger>();
            var words = new BlockingCollection<Word>()
                            {
                                existWords[0],
                                unknownWords[0],
                                existWords[1],
                                unknownWords[1]
                            };
            words.CompleteAdding();
            var checker = new WordChecker(dictionaryPath, words, loggerMock.Object);

            // Act
            checker.StartChecking();

            // Assert
            loggerMock.Verify(logger => logger.Log(unknownWords[0].ToString()), Times.Once());
            loggerMock.Verify(logger => logger.Log(unknownWords[1].ToString()), Times.Once());
        }
    }
}