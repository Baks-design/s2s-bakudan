using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Runtime.Utilities.Patterns.ServiceLocator
{
    public class ServiceManager
    {
        readonly Dictionary<Type, object> _services = new();

        /// <summary>
        /// Gets a collection of all registered services.
        /// </summary>
        public IEnumerable<object> RegisteredServices => _services.Values;

        /// <summary>
        /// Tries to get a service of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <param name="service">The retrieved service, or null if not found.</param>
        /// <returns>True if the service was found, otherwise false.</returns>
        public bool TryGet<T>(out T service) where T : class
        {
            if (_services.TryGetValue(typeof(T), out var obj))
            {
                service = obj as T;
                return service != null;
            }

            service = null;
            return false;
        }

        /// <summary>
        /// Gets a service of the specified type. Throws an exception if the service is not found.
        /// </summary>
        /// <typeparam name="T">The type of service to retrieve.</typeparam>
        /// <returns>The retrieved service.</returns>
        /// <exception cref="ArgumentException">Thrown if the service is not registered.</exception>
        public T Get<T>() where T : class
        {
            if (TryGet<T>(out var service))
                return service;

            throw new ArgumentException($"ServiceManager.Get: Service of type {typeof(T).FullName} not registered");
        }

        /// <summary>
        /// Registers a service using its type.
        /// </summary>
        /// <typeparam name="T">The type of service to register.</typeparam>
        /// <param name="service">The service instance to register.</param>
        /// <returns>The current ServiceManager instance for chaining.</returns>
        public ServiceManager Register<T>(T service)
        {
            if (service == null)
                throw new ArgumentNullException(nameof(service));

            var type = typeof(T);

            if (!_services.TryAdd(type, service))
                Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");

            return this;
        }

        /// <summary>
        /// Registers a service using a specific type.
        /// </summary>
        /// <param name="type">The type to use for registration.</param>
        /// <param name="service">The service instance to register.</param>
        /// <returns>The current ServiceManager instance for chaining.</returns>
        /// <exception cref="ArgumentException">Thrown if the service type does not match the provided type.</exception>
        public ServiceManager Register(Type type, object service)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));
            if (service == null)
                throw new ArgumentNullException(nameof(service));
            if (!type.IsInstanceOfType(service))
                throw new ArgumentException("Type of service does not match type of service interface", nameof(service));

            if (!_services.TryAdd(type, service))
                Debug.LogError($"ServiceManager.Register: Service of type {type.FullName} already registered");

            return this;
        }
    }
}