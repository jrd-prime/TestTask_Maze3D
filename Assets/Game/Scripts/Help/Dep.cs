using System;
using System.Collections.Generic;
using Game.Scripts.Scope;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Help
{
    public class Dep : MonoBehaviour
    {
        private RootScope _rootScope;
        private readonly Dictionary<Type, object> _depCache = new();

        private T Resolve<T>()
        {
            _rootScope = FindFirstObjectByType<RootScope>();

            if (_rootScope == null) throw new NullReferenceException($"Can't find {nameof(RootScope)}");

            var value = (T)_rootScope.Container.Resolve(typeof(T));

            if (value == null) throw new Exception($"Can't resolve {typeof(T)}");

            _depCache.TryAdd(typeof(T), value);
            return value;
        }

        public T GetDependency<T>()
        {
            Debug.LogWarning("_rootScope: " + _rootScope);

            return _depCache.ContainsKey(typeof(T))
                ? (T)_depCache[typeof(T)]
                : Resolve<T>();
        }

        private void OnDestroy() => _depCache.Clear();
    }
}
