using System;
using UnityEngine;

namespace Utilities.Logger
{
    public static class Log
    {
        public static void Error(String message)
        {
            Debug.Log($"<color=red>ERROR</color> <color=teal>==></color> {message}");
        }

        public static void Value(String message)
        {
            Debug.Log($"<color=yellow>VALUE</color> <color=teal>==></color> {message}");
        }

        public static void Message(String className = "SYSTEM", String message = "message")
        {
            Debug.Log($"<color=cyan>{className}</color> <color=teal>==></color> {message}");
        }

        public static void Created(String className)
        {
            Message(className, "CREATED");
        }
    }
}
