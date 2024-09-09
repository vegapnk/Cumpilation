using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Cumpilation
{
    public class ModLog
    {
        public static string ModId => "Cumpilation";

        /// <summary>
        /// Logs the given message with [SaveStorage.ModId] appended.
        /// </summary>
        public static void Error(string message)
        {
            Log.Error($"[{ModId}] {message}");
        }

        /// <summary>
        /// Logs the given message with [SaveStorage.ModId] appended.
        /// </summary>
        public static void Message(string message)
        {
            Log.Message($"[{ModId}] {message}");
        }

        /// <summary>
        /// Logs the given message with [SaveStorage.ModId] appended.
        /// </summary>
        public static void Warning(string message)
        {
            Log.Warning($"[{ModId}] {message}");
        }

        public static void Debug(string message)
        {
            if (Settings.Settings.EnableCumpilationDebugLogging)
            {
                Log.Message($"[{ModId}][debug] {message}");
            }
        }
    }
}
