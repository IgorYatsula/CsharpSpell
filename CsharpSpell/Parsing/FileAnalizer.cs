using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

using CsharpSpell.Entities;
using CsharpSpell.Logging;

namespace CsharpSpell.Parsing
{
    /// <summary>
    /// Represents entity to analize source file, find comments and words.
    /// </summary>
    public class FileAnalizer
    {
        private const string NotClosedCommentPattern    = @"\/\*(?![a-zA-Z\d\s\*-_\(\)$\|%\{\}&!~@#]*\*\/)",
                             WordPattern                = @"[a-zA-Z\d]+";

        private readonly string filePath;
        private readonly BlockingCollection<Word> words;
        private readonly ILogger logger;
        private readonly CommentFinder finder;
        
        /// <summary>
        /// Creates a new instance of analizer.
        /// </summary>
        /// <param name="filePath">The path to the source file.</param>
        /// <param name="words">The collection to store found words and checking spelling.</param>
        /// <param name="logger">The logger to print program's results.</param>
        public FileAnalizer(string filePath, BlockingCollection<Word> words, ILogger logger)
        {
            this.filePath = filePath;
            this.words = words;
            this.logger = logger;
            this.finder = new CommentFinder();
        }

        /// <summary>
        /// Analizes the file and store found words into the collection for future checking.
        /// </summary>
        public void Analize()
        {
            //this.logger.LogFormat("ANALIZER -> Started with thread: [{0}]", Thread.CurrentThread.ManagedThreadId);
            // Load file content into the memory.
            string fileContent = File.ReadAllText(this.filePath);

            if (Regex.IsMatch(fileContent, NotClosedCommentPattern)) // check if the file can be processed
            {
                this.logger.Log("File is not correct, there is not closed comment!");
                this.words.CompleteAdding();
                return;
            }

            IList<string> lines = fileContent.Split(new[] { "\n" }, StringSplitOptions.None).ToList();
            for (var lineIndex = 0; lineIndex < lines.Count; lineIndex++)
            {
                string line = lines[lineIndex];

                string comment = this.finder.FindComment(line);
                if (comment != string.Empty)
                {
                    MatchCollection matches = Regex.Matches(comment, WordPattern);
                    foreach (Match match in matches)
                    {
                        int columnIndex = line.IndexOf(match.Value) + 1;
                        this.words.Add(new Word { Row = lineIndex + 1, Column = columnIndex, Content = match.Value });
                    }
                }
                //this.logger.LogFormat("ANALIZER -> Thread: [{0}], Line: [{1}]", Thread.CurrentThread.ManagedThreadId, lineIndex + 1);
            }
            
            this.words.CompleteAdding();
        }
    }
}