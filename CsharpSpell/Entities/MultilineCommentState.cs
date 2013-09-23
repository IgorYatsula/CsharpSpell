namespace CsharpSpell.Entities
{
    /// <summary>
    /// Represents enumeration to determine the state of a previous line comment.
    /// </summary>
    internal enum MultilineCommentState
    {
        /// <summary>
        /// Multiline comment is opened.
        /// </summary>
        Opened,

        /// <summary>
        /// Multiline comment is closed.
        /// </summary>
        Closed
    }
}