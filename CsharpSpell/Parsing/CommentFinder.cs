using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CsharpSpell.Entities;

namespace CsharpSpell.Parsing
{
    /// <summary>
    /// Represents internal entity to find comments within text line.
    /// </summary>
    internal class CommentFinder
    {
        /// <summary>
        /// Pattern for finding comments of different types.
        /// </summary>
        private const string GroupCommentsPatternFormat = @"(?<{0}>(?<!(?<=\/\*)[a-z]*)\*\/)|(?<{1}>/\*(.|[\r\n])*?\*/)|(?<{2}>\/\*(?![a-z]*\*\/))|(?<{3}>//.*)";
        /// <summary>
        /// The current state of mutliline comment.
        /// </summary>
        private MultilineCommentState state;
        /// <summary>
        /// The builder for storing all parts of the comments within the line of text.
        /// </summary>
        private readonly StringBuilder commentBuilder;
        /// <summary>
        /// The count of matches for each type of the comment.
        /// </summary>
        private readonly Dictionary<CommentType, int> groupMatchesCount;
        /// <summary>
        /// The pattern for searching all types of the comments in the text.
        /// </summary>
        private readonly string groupCommentsPattern;

        /// <summary>
        /// Creates a new instance of the comment finder.
        /// </summary>
        public CommentFinder()
        {
            this.state = MultilineCommentState.Closed;
            this.commentBuilder = new StringBuilder();
            this.groupMatchesCount = new Dictionary<CommentType, int>();
            this.groupCommentsPattern = string.Format(GroupCommentsPatternFormat, 
                CommentType.Finished, CommentType.Completed, CommentType.Started, CommentType.Single);
        }

        /// <summary>
        /// Finds comment within received line of text.
        /// </summary>
        /// <param name="line">The line of text for finding.</param>
        /// <returns>The text with comments.</returns>
        public string FindComment(string line)
        {
            this.commentBuilder.Clear();
            this.groupMatchesCount.Clear();

            // Matching received line of the text with all types of the comments
            IEnumerable<Match> matches = Regex.Matches(line, groupCommentsPattern).Cast<Match>();

            IEnumerable<CommentType> commentTypes = Enum.GetValues(typeof(CommentType)).Cast<CommentType>();
            foreach (var commentType in commentTypes) // Filling count of matches by group
            {
                int matchesCount = matches.Count(m => !string.IsNullOrEmpty(m.Groups[commentType.ToString()].Value));
                this.groupMatchesCount.Add(commentType, matchesCount);
            }

            // Checking current state and reading appropriate group of matches
            if (this.state == MultilineCommentState.Closed)
            {
                if (this.groupMatchesCount[CommentType.Started] > 0)
                {
                    this.state = MultilineCommentState.Opened;
                    this.ReadGroup(line, matches, CommentType.Started);
                }
            }
            else if (this.state == MultilineCommentState.Opened)
            {
                if (this.groupMatchesCount[CommentType.Finished] > 0)
                {
                    this.state = MultilineCommentState.Closed;
                    this.ReadGroup(line, matches, CommentType.Finished);

                    if (this.groupMatchesCount[CommentType.Started] > 0)
                    {
                        this.state = MultilineCommentState.Opened;
                        this.ReadGroup(line, matches, CommentType.Started);
                    }
                }
                else
                {
                    this.commentBuilder.Append(line);
                    return this.commentBuilder.ToString();
                }
            }

            if (this.groupMatchesCount[CommentType.Completed] > 0)
            {
                this.ReadGroup(line, matches, CommentType.Completed);
            }

            if (this.groupMatchesCount[CommentType.Single] > 0)
            {
                this.ReadGroup(line, matches, CommentType.Single);
            }

            return this.commentBuilder.ToString();
        }

        /// <summary>
        /// Reads comments' content from the matched groups into the builder based of type of the comment.
        /// </summary>
        /// <param name="line">The line of text for finding comments.</param>
        /// <param name="matches">The found grouped matches in the current text line</param>
        /// <param name="commentType">The type of comment which should be read.</param>
        private void ReadGroup(string line, IEnumerable<Match> matches, CommentType commentType)
        {
            IEnumerable<Match> groupMatches = matches.Where(m => !string.IsNullOrEmpty(m.Groups[commentType.ToString()].Value));
            
            // such matches read from the text WITH comments' content
            if (commentType == CommentType.Completed || commentType == CommentType.Single)
            {
                foreach (Match groupMatch in groupMatches)
                {
                    this.commentBuilder.Append(groupMatch.Value);
                }
            }
            else
            {
                // NOTE: Must be only 1 match for such types of comments !
                // Such matches read from the text without content - only indexes
                Match commentMatch = groupMatches.First();
                if (commentType == CommentType.Started)
                {
                    this.commentBuilder.Append(line.Substring(commentMatch.Index));
                }
                else
                {
                    this.commentBuilder.Append(line.Substring(0, commentMatch.Index));
                }
            }
        }
    }
}