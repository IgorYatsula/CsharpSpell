namespace CsharpSpell.Logging
{
    /// <summary>
    /// Represents interface for logging.
    /// </summary>
    public interface ILogger
    {
        void Log(string message);

        void LogFormat(string format, params object[] messages);
    }
}