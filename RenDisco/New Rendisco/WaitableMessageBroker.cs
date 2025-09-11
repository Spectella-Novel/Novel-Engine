using System;
using System.Collections.Concurrent;
using System.Threading;
namespace RenDisco
{
    public class WaitableMessageBroker
    {
        // Хранит сообщения по топикам
        private static readonly ConcurrentDictionary<string, ConcurrentQueue<object>> _topics =
            new ConcurrentDictionary<string, ConcurrentQueue<object>>();

        // Хранит события ожидания по топикам
        private static readonly ConcurrentDictionary<string, ManualResetEventSlim> _waitEvents =
            new ConcurrentDictionary<string, ManualResetEventSlim>();


        /// <summary>
        /// Публикует сообщение в топик
        /// </summary>
        public static void Publish(string topic, object message)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Топик не может быть пустым");

            var queue = _topics.GetOrAdd(topic, _ => new ConcurrentQueue<object>());
            queue.Enqueue(message);

            // Сигнализируем всем ожидающим
            var waitEvent = _waitEvents.GetOrAdd(topic, _ => new ManualResetEventSlim(false));
            waitEvent.Set();
        }

        /// <summary>
        /// Ждет сообщение из топика (бесконечно)
        /// </summary>
        public static object WaitForMessage(string topic)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Топик не может быть пустым");

            // Проверяем, есть ли уже сообщения
            if (_topics.TryGetValue(topic, out var queue) && !queue.IsEmpty)
            {
                if (queue.TryDequeue(out var existingMessage))
                    return existingMessage;
            }

            // Создаем или получаем событие ожидания
            var waitEvent = _waitEvents.GetOrAdd(topic, _ => new ManualResetEventSlim(false));

            // Ждем нового сообщения
            waitEvent.Wait();

            // Сбрасываем событие для следующего ожидания
            waitEvent.Reset();

            // Возвращаем сообщение
            if (_topics.TryGetValue(topic, out var messageQueue) &&
                messageQueue.TryDequeue(out var message))
            {
                return message;
            }

            return null; // На случай гонки условий
        }

        /// <summary>
        /// Ждет сообщение из топика с таймаутом
        /// </summary>
        public static (bool success, object message) WaitForMessage(string topic, int timeoutMs)
        {
            if (string.IsNullOrEmpty(topic))
                throw new ArgumentException("Топик не может быть пустым");

            // Проверяем существующие сообщения
            if (_topics.TryGetValue(topic, out var queue) && !queue.IsEmpty)
            {
                if (queue.TryDequeue(out var existingMessage))
                    return (true, existingMessage);
            }

            var waitEvent = _waitEvents.GetOrAdd(topic, _ => new ManualResetEventSlim(false));

            bool signaled = waitEvent.Wait(timeoutMs);

            if (signaled)
            {
                waitEvent.Reset();

                if (_topics.TryGetValue(topic, out var messageQueue) &&
                    messageQueue.TryDequeue(out var message))
                {
                    return (true, message);
                }
            }

            return (false, null);
        }

        /// <summary>
        /// Проверяет, есть ли сообщения в топике (неблокирующий)
        /// </summary>
        public static bool HasMessages(string topic)
        {
            return _topics.TryGetValue(topic, out var queue) && !queue.IsEmpty;
        }

        /// <summary>
        /// Получает количество сообщений в топике
        /// </summary>
        public static int GetMessageCount(string topic)
        {
            if (_topics.TryGetValue(topic, out var queue))
                return queue.Count;
            return 0;
        }

        /// <summary>
        /// Очищает все сообщения в топике
        /// </summary>
        public static void ClearTopic(string topic)
        {
            if (_topics.TryGetValue(topic, out var queue))
            {
                while (queue.TryDequeue(out _)) { }
            }
        }

        /// <summary>
        /// Освобождает ресурсы
        /// </summary>
        public static void Dispose()
        {
            foreach (var waitEvent in _waitEvents.Values)
            {
                waitEvent?.Dispose();
            }
            _waitEvents.Clear();
            _topics.Clear();
        }
    }
}
