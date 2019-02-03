using System;

namespace GTiff2Tiles.Console
{
    /// <summary>
    /// Implementation of <see cref="IProgress{T}"/> for console.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsoleProgress<T> : IProgress<T>
    {
        private readonly Action<T> _action;

        public ConsoleProgress(Action<T> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

        public void Report(T value) => _action(value);
    }
}
