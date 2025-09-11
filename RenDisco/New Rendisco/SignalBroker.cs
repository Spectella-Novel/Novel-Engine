using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Collections.Generic;

namespace RenDisco
{
    public static class SignalBroker
    {
        private static readonly ConcurrentDictionary<string, ConcurrentBag<Action>> _signalHandlers =
            new ConcurrentDictionary<string, ConcurrentBag<Action>>();

        private static SynchronizationContext _syncContext;
        private static bool _isInitialized = false;
        private static readonly object _initLock = new object();

        // Инициализация контекста синхронизации
        public static void Initialize(SynchronizationContext syncContext = null)
        {
            lock (_initLock)
            {
                if (!_isInitialized)
                {

                    _syncContext = syncContext ?? SynchronizationContext.Current ?? new SynchronizationContext();
                    _isInitialized = true;
                    Console.WriteLine($"Initialized on thread: {Thread.CurrentThread.Name ?? Thread.CurrentThread.ManagedThreadId.ToString()}");
                }
            }
        }

        // Ленивая инициализация
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                lock (_initLock)
                {
                    if (!_isInitialized)
                    {
                        Initialize();
                    }
                }
            }
        }

        // Подписаться на сигнал
        public static void On(string signal, Action handler)
        {
            if (string.IsNullOrEmpty(signal))
                throw new ArgumentException("Сигнал не может быть пустым");

            if (handler == null)
                throw new ArgumentNullException(nameof(handler));

            Console.WriteLine("Signal saved: " + signal);
            EnsureInitialized();

            var handlers = _signalHandlers.GetOrAdd(signal, _ => new ConcurrentBag<Action>());
            handlers.Add(handler);
        }

        // Отписаться от сигнала
        public static void Off(string signal, Action handler)
        {
            if (string.IsNullOrEmpty(signal) || handler == null)
                return;

            if (_signalHandlers.TryGetValue(signal, out var handlers))
            {
                var newHandlers = new ConcurrentBag<Action>();
                foreach (var h in handlers)
                {
                    if (h != handler)
                    {
                        newHandlers.Add(h);
                    }
                }
                _signalHandlers[signal] = newHandlers;
            }
        }

        // Отписаться от всех обработчиков сигнала
        public static void Off(string signal)
        {
            if (!string.IsNullOrEmpty(signal))
            {
                _signalHandlers.TryRemove(signal, out _);
            }
        }

        // Отправить сигнал
        public static void Emit(string signal)
        {
            if (string.IsNullOrEmpty(signal))
                return;

            Console.WriteLine($"Emit called on thread: {Thread.CurrentThread.Name ?? Thread.CurrentThread.ManagedThreadId.ToString()}");
            Console.WriteLine("Signal Emit: " + signal);

            EnsureInitialized();

            if (_signalHandlers.TryGetValue(signal, out var handlers))
            {
                Console.WriteLine("handlers count: " + handlers.Count);

                // Создаем копию списка для безопасного перебора
                var handlersList = new List<Action>(handlers);
                Console.WriteLine($"Found {handlersList.Count} handlers");

                // Выполняем обработчики
                foreach (var handler in handlersList)
                {
                    try
                    {
                        // Если мы уже в правильном контексте, вызываем напрямую
                        if (_syncContext == null || SynchronizationContext.Current == _syncContext)
                        {
                            Console.WriteLine("Invoking directly");
                            handler?.Invoke();
                        }
                        else
                        {
                            Console.WriteLine("Posting to sync context");
                            _syncContext.Post(_ =>
                            {
                                try
                                {
                                    Console.WriteLine("Executing handler in sync context");
                                    handler?.Invoke();
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Handler exception: {ex}");
                                }
                            }, null);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error posting to sync context: {ex}");
                    }
                }
            }
        }

        // Асинхронная версия Emit для избежания блокировок
        public static void EmitAsync(string signal)
        {
            if (string.IsNullOrEmpty(signal))
                return;

            ThreadPool.QueueUserWorkItem(_ => Emit(signal));
        }

        // Очистить все обработчики
        public static void Clear()
        {
            _signalHandlers.Clear();
        }

        // Проверить, инициализирован ли контекст
        public static bool IsInitialized => _isInitialized;
    }
}