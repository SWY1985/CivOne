namespace CivOne
{
    public static class Logger
    {
        public delegate void LogOperation(string text, params object[] parameters);

        public static void Log(string text, params object[] parameters)
        {
            logOperation(text, parameters);
        }

        public static void SetLogger(LogOperation logOperation)
        {
            Logger.logOperation = logOperation;
        }

        private static LogOperation logOperation;
    }
}