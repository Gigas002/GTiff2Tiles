using System;

namespace GTiff2Tiles.NetCore.Console
{
    /// <inheritdoc />
    /// <summary>
    /// Implementation of <see cref="T:System.IProgress`1" /> for console.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal sealed class ConsoleProgress<T> : IProgress<T>
    {
        private readonly Action<T> _action;

        public ConsoleProgress(Action<T> action) => _action = action ?? throw new ArgumentNullException(nameof(action));

        public void Report(T value) => _action(value);
    }
}
