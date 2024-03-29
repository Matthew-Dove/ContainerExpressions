﻿using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers.Internal
{
    // Jit will keep these methods cold, until they are used (as long as they only throw exceptions as a single instruction).
    internal static class Throw
    {
        public static void Exception<T>(T ex) where T : Exception => throw ex;
        public static void Exception(ExceptionDispatchInfo ex) => ex.Throw();

        public static void ArgumentOutOfRangeException(string name) => throw new ArgumentOutOfRangeException(name);
        public static void ArgumentOutOfRangeException(string name, string message) => throw new ArgumentOutOfRangeException(name, message);

        public static void ArgumentNullException(string name) => throw new ArgumentNullException(name);
        public static void ArgumentNullException(string name, string message) => throw new ArgumentNullException(name, message);

        public static void InvalidOperationException(string message) => throw new InvalidOperationException(message);

        public static void TaskCanceledException(Task task) => throw new TaskCanceledException(task);
        public static void TaskCanceledException(string message) => throw new TaskCanceledException(message);
    }
}
