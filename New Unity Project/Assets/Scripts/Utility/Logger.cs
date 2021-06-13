using System;
using UnityEngine;

namespace Utilities.Logger
{
    public static class Log
    {
        private static void ShowLog(String message)
        {
            Debug.Log(message);
        }

        public static void Error(String message, bool hideErrorsInFile = false)
        {
            if (!hideErrorsInFile)
                Debug.Log($"<color=red>ERROR</color> <color=teal>==></color> {message}");
        }

        public static void Value(String message, bool hideValuesInFile = false)
        {
            if (!hideValuesInFile)
                Debug.Log($"<color=yellow>VALUE</color> <color=teal>==></color> {message}");
        }

        public static void Message(String className = "SYSTEM", String message = "message", bool hideMessagesInFile = false)
        {
            if (!hideMessagesInFile)
                Debug.Log($"<color=cyan>{className}</color> <color=teal>==></color> {message}");
        }

        public static void Created(String className, bool hideCreatedInFile = false)
        {
            if (hideCreatedInFile)
                Message(className, "CREATED");
        }
    }
}
