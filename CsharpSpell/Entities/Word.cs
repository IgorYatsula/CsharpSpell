namespace CsharpSpell.Entities
{
    /// <summary>
    /// Represents entity to store information about found word in the comments.
    /// </summary>
    public struct Word
    {
        /// <summary>
        /// The word's content.
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// The row number of the current word.
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// The column number of the current word.
        /// </summary>
        public int Column { get; set; }

        /// <summary>
        /// Converts the current word to string representation in the required format.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}:{2}", Row, Column, Content);
        }
    }
}