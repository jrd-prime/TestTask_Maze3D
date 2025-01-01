using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using Object = UnityEngine.Object;

namespace Game.Scripts.Factory
{
    public class CustomPool<T> where T : MonoBehaviour
    {
        private readonly T _prefab;
        private readonly bool _allowGrowth;
        private readonly int _poolSize;
        private readonly Transform _parent;
        private readonly Queue<T> _cache = new();
        private readonly HashSet<T> _activeObjects = new(); // Для проверки активных объектов

        public CustomPool(T prefab, int poolSize, Transform parent, bool allowGrowth = false)
        {
            _prefab = prefab;
            _poolSize = poolSize;
            _parent = parent;
            _allowGrowth = allowGrowth;

            Initialize();
        }

        private void Initialize()
        {
            for (var i = 0; i < _poolSize; i++) CreateObject();
        }

        private void CreateObject()
        {
            var go = Object.Instantiate(_prefab, _parent);
            go.gameObject.SetActive(false);
            _cache.Enqueue(go);
        }

        public T Get()
        {
            if (_cache.Count > 0)
            {
                var obj = _cache.Dequeue();
                obj.gameObject.SetActive(true);
                _activeObjects.Add(obj);
                return obj;
            }

            if (!_allowGrowth) throw new InvalidOperationException("Pool is empty and growth is not allowed!");

            CreateObject();
            var newObj = Get();
            return newObj;
        }

        public void Return(T obj)
        {
            if (obj is null) return;

            if (!_activeObjects.Contains(obj)) return;
            obj.gameObject.SetActive(false);
            _activeObjects.Remove(obj);
            _cache.Enqueue(obj);
        }

        public void ReturnAll()
        {
            foreach (var activeObject in _activeObjects.ToList()) Return(activeObject);
        }
    }
}
