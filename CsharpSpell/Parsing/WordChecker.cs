using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;

using CsharpSpell.Entities;
using CsharpSpell.Logging;

namespace CsharpSpell.Parsing
{
    /// <summary>
    /// Represents entity to check the found words in the dictionary.
    /// </summary>
    public class WordChecker
    {
        private readonly string dictionaryPath;
        private readonly BlockingCollection<Word> words;
        private readonly ILogger logger;
        private HashSet<string> dictionary; 

        /// <summary>
        /// Creates a new instance of the word checker.
        /// </summary>
        /// <param name="dictionaryPath">The path to the dictionary file.</param>
        /// <param name="words">The collection with words for checking.</param>
        /// <param name="logger">The logger to print program's results.</param>
        public WordChecker(string dictionaryPath, BlockingCollection<Word> words, ILogger logger)
        {
            this.dictionaryPath = dictionaryPath;
            this.words = words;
            this.logger = logger;
        }

        /// <summary>
        /// Starts the checking process. 
        /// During this process the checker will wait for new words, receive and check it, and print results for incorrect words.
        /// </summary>
        public void StartChecking()
        {
            //this.logger.LogFormat("CHECKER -> Started with thread: [{0}]", Thread.CurrentThread.ManagedThreadId);

            this.LoadDictionary();

            foreach (Word word in words.GetConsumingEnumerable()) // wait for new words from analizer
            {
                if (!this.Check(word))
                {
                    this.logger.Log(word.ToString());
                }
                //this.logger.LogFormat("CHECKER -> Thread: [{0}], Word: [{1}]", Thread.CurrentThread.ManagedThreadId, word);
            }
        }

        /// <summary>
        /// Loads the dictionary content into the memory.
        /// </summary>
        private void LoadDictionary()
        {
            IEnumerable<string> lines = File.ReadLines(dictionaryPath).Select(line => line.ToLower());
            this.dictionary = new HashSet<string>(lines);
        }

        /// <summary>
        /// Checks the word in the dictionary.
        /// </summary>
        /// <param name="word">The received word from analizer.</param>
        /// <returns>The result of checking, 'true' for correct words.</returns>
        private bool Check(Word word)
        {
            return this.dictionary.Contains(word.Content);
        }
    }
}