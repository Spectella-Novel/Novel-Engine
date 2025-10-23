using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenDisco
{
    internal class ControlFlowProcessor
    {
        private readonly ConcurrentDictionary<Type, IControlFlowHandler> _handlers = new();

        // Зарегистрировать обработчик для определённого типа сигнала
        public void Register<TSignal>(IControlFlowHandler handler) where TSignal : ControlFlowSignal
        {
            var type = typeof(TSignal);
            if (!_handlers.TryAdd(type, handler))
            {
                throw new InvalidOperationException(
                    $"Обработчик для сигнала типа {type.Name} уже зарегистрирован.");
            }
        }

        // Обработать сигнал
        public bool Process(ControlFlowSignal signal, Action resumeExecution)
        {
            if (signal == null) return false;

            var type = signal.GetType();
            if (_handlers.TryGetValue(type, out var handler))
            {
                handler.Handle(signal, resumeExecution);
                return true;
            }

            return false; // Обработчик не найден
        }
    }
}
