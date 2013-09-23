namespace CsharpSpell.Entities
{
    /// <summary>
    /// Represents enumeration to determine the type of comment.
    /// </summary>
    internal enum CommentType
    {
        /// <summary>
        /// Multiline comment was opened and closed on the same line of text, single comment.
        /// Example: some_code /* comment_text */ another_code
        /// Note: Finder reads such comments with content.
        /// </summary>
        Completed,

        /// <summary>
        /// Multiline comment was started and not finished.
        /// Example: some_code /* some_comment
        /// </summary>
        Started,

        /// <summary>
        /// Multiline comment was finished.
        /// Example: some_symbols */
        /// </summary>
        Finished,

        /// <summary>
        /// Sngleline comment.
        /// Example: some_code // comment_text
        /// </summary>
        Single
    }
}