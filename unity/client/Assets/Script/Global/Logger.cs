using System;
using UnityEngine;

namespace Assets.Script
{
    public interface IAppLog
    {
        void LogDebug(string format, params object[] args);
        void LogInfo(string format, params object[] args);
        void LogError(string format, params object[] args);
        void LogErrorIfNull(object obj, string context);
        void LogErrorIfFalse(bool expr, string format, params object[] args);
    }

    // TODO: Remove statics
    public class AppLog : IAppLog
    {
        public void LogDebug(string format, params object[] args)
        {
            StaticLogInfo(format, args);
        }

        public void LogError(string format, params object[] args)
        {
            StaticLogError(format, args);
        }

        public void LogInfo(string format, params object[] args)
        {
            StaticLogInfo(format, args);
        }

        public static void StaticLogError(string format, params object[] args)
        {
            string line = string.Format(GetTimestamp() + "> " + format, args);
            Debug.LogErrorFormat(line);
        }

        public static void StaticLogInfo(string format, params object[] args)
        {
            string line = string.Format(GetTimestamp() + "> " + format, args);
            Debug.LogFormat(line);
        }

        // ISO 8061 timestamp
        private static string GetTimestamp()
        {
            return DateTime.UtcNow.ToString("o", System.Globalization.CultureInfo.InvariantCulture);
        }

        public void LogErrorIfNull(object obj, string context)
        {
            if (obj != null)
                return;

            LogError("Unexpected null, context = {0}", context);
        }

        public void LogErrorIfFalse(bool expr, string format, params object[] args)
        {
            if (expr)
                return;

            LogError(format, args);
        }
    }
}
