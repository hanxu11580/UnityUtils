using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace RobotCat {

    /// <summary>
    /// ����
    /// ��Ҫְ����
    ///     ��ȡ��ע�ᡢ�ͷŷ���
    /// </summary>
    public class RobotCatContainer : IServiceProvider , IDisposable{

        // ������
        internal readonly RobotCatContainer _root;

        internal readonly ConcurrentDictionary<Type, RobotCatServiceRegistry> _registries;

        internal readonly ConcurrentDictionary<RobotCatKey, object> _services;

        // �ͷŷ���
        internal readonly ConcurrentBag<IDisposable> _disposables;

        // ���߱�����������ֶο��ܻᱻ���߳����� ��Ҫ�Ż�����
        private volatile bool _disposed;

        public RobotCatContainer() {
            _root = this;
            _registries = new ConcurrentDictionary<Type, RobotCatServiceRegistry>();
            _services = new ConcurrentDictionary<RobotCatKey, object>();
            _disposables = new ConcurrentBag<IDisposable>();
        }

        /// <summary>
        /// ��ҹ���һ��Root
        /// </summary>
        /// <param name="parent"></param>
        internal RobotCatContainer(RobotCatContainer parent) {
            _root = parent._root;
            _registries = parent._registries;
            _services = new ConcurrentDictionary<RobotCatKey, object>();
            _disposables = new ConcurrentBag<IDisposable>();
        }

        public void Dispose() {
            _disposed = true;
            foreach (var disposable in _disposables) {
                disposable.Dispose();
            }

            while (!_disposables.IsEmpty) {
                _disposables.TryTake(out _);
            }

            _services.Clear();
        }

        public object GetService(Type serviceType) {
            EnsureNotDisposed();
            // ��ȡ���������������
            if(serviceType == typeof(RobotCatContainer) || serviceType == typeof(IServiceProvider)) {
                return this;
            }

            RobotCatServiceRegistry registry;
            if(serviceType.IsGenericType && serviceType.GetGenericTypeDefinition() == typeof(IEnumerable<>)) {
                var elementType = serviceType.GetGenericArguments()[0];
                if (!_registries.TryGetValue(serviceType, out registry)) {
                    return Array.CreateInstance(elementType, 0);
                }
                var registies = registry.AsEnumerable();
                var services = registies.Select(it => GetServiceCore(it, Type.EmptyTypes)).ToArray();
                Array array = Array.CreateInstance(elementType, services.Length);
                services.CopyTo(array, 0);
                return array;
            }

            // ����
            if (serviceType.IsGenericType && _registries.ContainsKey(serviceType)) {
                Type definition = serviceType.GetGenericTypeDefinition();
                if (_registries.TryGetValue(definition, out registry)) {
                    return GetServiceCore(registry, serviceType.GetGenericArguments());
                }
                else {
                    return null;
                }
            }

            //Normal
            if(_registries.TryGetValue(serviceType, out registry)) {
                return GetServiceCore(registry, Type.EmptyTypes);
            }
            else {
                return null;
            }
        }

        private object GetServiceCore(RobotCatServiceRegistry registry, Type[] genericArguments) {
            var key = new RobotCatKey(registry, genericArguments);
            //var serviceType = registry.ServiceType;
            switch (registry.Lifetime) {
                case Lifetime.Self:
                    return GetOrCreate(_services, _disposables);
                case Lifetime.Root:
                    return GetOrCreate(_root._services, _root._disposables);
                default: {
                        // �����Ĳ����棬������ֱ�Ӷ���_disposables
                        var service = registry.Factory(this, genericArguments);
                        if (service is IDisposable disposable) {
                            _disposables.Add(disposable);
                        }
                        return service;
                    }
            }

            // ����������
            object GetOrCreate(ConcurrentDictionary<RobotCatKey, object> services, ConcurrentBag<IDisposable> disposables) {
                if (services.TryGetValue(key, out var service)) {
                    return service;
                }
                service = registry.Factory(this, genericArguments);
                services[key] = service;
                if (service is IDisposable disposable) {
                    disposables.Add(disposable);
                }
                return service;
            }
        }

        public void Register(RobotCatServiceRegistry registry) {
            EnsureNotDisposed();
            if(_registries.TryGetValue(registry.ServiceType, out var existing)) {
                // �ŵ�ͷ��
                _registries[registry.ServiceType] = registry;
                registry.Next = existing;
            }
            else {
                _registries[registry.ServiceType] = registry;
            }
        }


        private void EnsureNotDisposed() {
            if (_disposed) {
                throw new ObjectDisposedException("RobotCatContainer");
            }
        }
    }


    public static class RobotCatContainerExtensions {
        public static RobotCatContainer Register(this RobotCatContainer cat, Type from, Type to, Lifetime lifetime) {
            Func<RobotCatContainer, Type[], object> factory = (_, argument) => Create(_, to, argument);
            cat.Register(new RobotCatServiceRegistry(from, lifetime, factory));
            return cat;
        }


        public static RobotCatContainer Register<TFrom, TTo>(this RobotCatContainer cat, Lifetime lifetime) where TTo : TFrom {
            return cat.Register(typeof(TFrom), typeof(TTo), lifetime);
        }

        public static RobotCatContainer Register(this RobotCatContainer cat, Type serviceType, object instance) {
            Func<RobotCatContainer, Type[], object> factory = (_, arguments) => instance;
            cat.Register(new RobotCatServiceRegistry(serviceType, Lifetime.Root, factory));
            return cat;
        }

        public static RobotCatContainer Register<TService>(this RobotCatContainer cat, TService instance) {
            Func<RobotCatContainer, Type[], object> factory = (_, arguments) => instance;
            cat.Register(new RobotCatServiceRegistry(typeof(TService), Lifetime.Root, factory));
            return cat;
        }

        public static RobotCatContainer Register(this RobotCatContainer cat, Type serviceType, Func<RobotCatContainer, object> factory, Lifetime lifetime) {
            cat.Register(new RobotCatServiceRegistry(serviceType, lifetime, (_, argument) => factory(_)));
            return cat;
        }

        public static RobotCatContainer Regsiter<TService>(this RobotCatContainer cat, Func<RobotCatContainer, TService> factory, Lifetime lifetime) {
            cat.Register(new RobotCatServiceRegistry(typeof(TService), lifetime, (_, arguments) => factory(_)));
            return cat;
        }

        public static RobotCatContainer CreateChild(this RobotCatContainer cat) {
            return new RobotCatContainer(cat);//����internal�Ĺ��캯��
        }

        private static object Create(RobotCatContainer cat, Type type, Type[] genericArguments) {
            if (genericArguments.Length > 0) {
                type = type.MakeGenericType(genericArguments);
            }

            var constructors = type.GetConstructors();
            if (constructors.Length == 0) {
                throw new InvalidOperationException($"canot create a instance of {type} which does not hava a public constructor");
            }
            //var constructor = constructors.FirstOrDefault(it => it.GetCustomAttributes(false).OfType<InjectionAttribute>().Any());
            //constructor ??= constructors.First();
            var constructor = constructors.First();
            ParameterInfo[] parameters = constructor.GetParameters();
            if (parameters.Length == 0) {
                return Activator.CreateInstance(type);
            }
            var arguments = new object[parameters.Length];
            for (int index = 0; index < arguments.Length; index++) {
                arguments[index] = cat.GetService(parameters[index].ParameterType);
            }
            return constructor.Invoke(arguments);
        }
    }
}