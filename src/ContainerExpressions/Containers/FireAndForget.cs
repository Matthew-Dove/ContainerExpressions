using ContainerExpressions.Expressions.Models;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace ContainerExpressions.Containers
{
    /// <summary>
    /// A place you can push tasks to, and optionally await them all at some later point.
    /// <para>Useful for "fire and forget" semantics.</para>
    /// <para>Tasks will remove themselves from internal storage once completed (cancelled, or faulted).</para>
    /// </summary>
    public static class FireAndForget
    {
        private static readonly ConcurrentDictionary<int, Task<bool>> _tasks = new();

        /// <summary>Add many tasks to be waited on at some future point.</summary>
        public static void Push(
            IEnumerable<Task> tasks,
            [CallerArgumentExpression(nameof(tasks))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            foreach (var task in tasks.ThrowIfNull("Attempted to push a null collection to the fire and forget container.", argument, caller, path, line))
            {
                Push(task, argument, caller, path, line);
            }
        }

        /// <summary>Add a task to be waited on at some future point.</summary>
        public static void Push(
            Task task,
            [CallerArgumentExpression(nameof(task))] string argument = "",
            [CallerMemberName] string caller = "",
            [CallerFilePath] string path = "",
            [CallerLineNumber] int line = 0
            )
        {
            var value = task.ThrowIfNull("Attempted to push a null task to the fire and forget container.", argument, caller, path, line)
            .ContinueWith(t => {
                if (t.Status == TaskStatus.Faulted)
                {
                    t.Exception.LogError(
                        "Error in fire and forget container task: {error}.".WithArgs(t.Exception.InnerException.Message),
                        argument, caller, path, line
                    );
                }
                else if (t.Status == TaskStatus.Canceled)
                {
                    t.Status.LogErrorValue(
                        "Cancelation in fire and forget container task.",
                        argument, caller, path, line
                    );
                }
                return _tasks.TryRemove(task.Id, out _);
            });

            _tasks.TryAdd(task.Id, value);
            if (task.Status == TaskStatus.Created) { task.Start(); }
        }

        /// <summary>Waits for all tasks that have been pushed to finish.</summary>
        public static async Task<CompletedTasks> WhenAll()
        {
            var tasks = _tasks.ToArray();

            if (tasks.Length > 0)
            {
                var tasksRemoved = await Task.WhenAll(tasks.Select(x => x.Value));
                if (tasksRemoved.Count(x => x) != tasks.Length)
                {
                    foreach (var task in tasks) { _tasks.TryRemove(task.Key, out _); }
                }
            }

            return tasks.Length;
        }
    }
}
