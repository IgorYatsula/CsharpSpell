using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsharpSpell.Entities;
using CsharpSpell.Logging;
using CsharpSpell.Parsing;

namespace CsharpSpell
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Main thread: [{0}]", Thread.CurrentThread.ManagedThreadId);
            string dictionaryPath, filePath;
            var logger = new ConsoleLogger();

            if (args.Count() == 2)
            {
                dictionaryPath = args[0];
                filePath = args[1];
            }
            else
            {
                logger.Log("Incorrect number of received parameters.");
                return;
            }

            if (!File.Exists(dictionaryPath))
            {
                logger.Log("Can't find dictionary file.");
                return;
            }

            if (!File.Exists(filePath))
            {
                logger.Log("Can't find source file.");
                return;
            }

            var words = new BlockingCollection<Word>();
            var analizer = new FileAnalizer(filePath, words, logger);
            var checker = new WordChecker(dictionaryPath, words, logger);

            var analizerTask = Task.Factory.StartNew(analizer.Analize);
            var checkerTask = Task.Factory.StartNew(checker.StartChecking);

            try
            {
                Task.WaitAll(analizerTask, checkerTask);
            }
            catch (AggregateException aggregateException) // TODO: need to refactor in the future to use concrete exceptions
            {
                words.CompleteAdding(); // to be sure that checkerTask won't wait new words
                aggregateException.InnerExceptions.ToList().ForEach(ex => logger.Log(ex.Message));
            }
        }
    }
}
