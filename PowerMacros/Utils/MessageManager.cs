using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace PowerMacros.Utils
{
    public class MessageManager
    {
        private static MessageManager _instance;
        public static MessageManager Instance 
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MessageManager();
                }
                return _instance;
            }
        }

        private readonly ConcurrentDictionary<Type, SynchronizedCollection<Subscription>> _subscriptions;
        private readonly ConcurrentDictionary<Type, object> _messageStates;

        private MessageManager()
        {
            _subscriptions = new ConcurrentDictionary<Type, SynchronizedCollection<Subscription>>();
            _messageStates = new ConcurrentDictionary<Type, object>();
        }

        public void Subscribe<T>(object subscriber, Action<object> action)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (!_subscriptions.ContainsKey(typeof(T)))
            {
                _subscriptions.TryAdd(typeof(T), new SynchronizedCollection<Subscription>());
            }

            if (!_messageStates.ContainsKey(typeof(T)))
            {
                _messageStates.TryAdd(typeof(T), null);
            }

            var sub = new Subscription
            {
                Action = action,
                Subscriber = subscriber
            };

            _subscriptions[typeof(T)].Add(sub);
            sub.Action(_messageStates[typeof(T)]);
        }

        public void Unsubscribe<T>(object subscriber)
        {
            if (subscriber == null)
            {
                throw new ArgumentNullException(nameof(subscriber));
            }
            if (!_subscriptions.ContainsKey(typeof(T)))
            {
                return;
            }

            var sub = _subscriptions[typeof(T)].FirstOrDefault(s => s.Subscriber == subscriber);

            if (sub != null)
            {
                _subscriptions[typeof(T)].Remove(sub);
            }
        }

        public void Send<T>(T message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!_messageStates.ContainsKey(typeof(T)))
            {
                _messageStates.TryAdd(typeof(T), null);
            }

            _messageStates[typeof(T)] = message;

            if (_subscriptions.ContainsKey(typeof(T)))
            {
                foreach (var sub in _subscriptions[typeof(T)])
                {
                    sub.Action(message);
                }
            }
        }
    }

    public class Subscription
    {
        public Action<object> Action { get; set; }
        public object Subscriber { get; set; }
    }
}
