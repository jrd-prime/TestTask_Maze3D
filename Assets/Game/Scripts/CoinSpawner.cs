using System;
using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Coins;
using Game.Scripts.Data.SO;
using Game.Scripts.Factory;
using Mirror;
using UnityEngine;

namespace Game.Scripts
{
    public sealed class CoinSpawner
    {
        private readonly List<Transform> _spawnPoints;
        private readonly CustomPool<Coin> _coinsPool;
        private readonly Dictionary<Transform, float> _distanceCache = new();

        private Vector3 _lastPoint = Vector3.zero;
        private Coin _currentCoin;
        private readonly CoinsManagerSO _settings;
        private readonly Transform _parent;

        public CoinSpawner(CoinsManagerSO settings, Transform parent)
        {
            _settings = settings;
            _parent = parent;

            _spawnPoints = _settings.spawnPoints;
            _coinsPool = new CustomPool<Coin>(_settings.coinPrefab, _settings.spawnPointsCount, _parent);
        }

        [Server]
        public void SpawnCoin()
        {
            _currentCoin = _coinsPool.Get();
            _currentCoin.Initialize(_settings.pointsPerCoin, _parent);
            _currentCoin.transform.position = GetSpawnPosition();
            NetworkServer.Spawn(_currentCoin.gameObject);
        }

        [Server]
        public void UnSpawnCoin()
        {
            _coinsPool.Return(_currentCoin);
            NetworkServer.UnSpawn(_currentCoin.gameObject);
        }

        private void CalcDistances(Vector3 targetPoint)
        {
            _distanceCache.Clear();

            foreach (var point in _spawnPoints) _distanceCache[point] = Vector3.Distance(targetPoint, point.position);
        }

        private Vector3 GetSpawnPosition()
        {
            if (_lastPoint == Vector3.zero) return _lastPoint = GetRandomFromList(_spawnPoints.ToArray());

            CalcDistances(_lastPoint);

            var availablePoints = _spawnPoints.Where(point => point.position != _lastPoint).ToList();

            availablePoints.Sort((a, b) => _distanceCache[a].CompareTo(_distanceCache[b]));

            if (availablePoints.Count > 3) availablePoints.RemoveRange(0, 3);

            return _lastPoint = GetRandomFromList(availablePoints.ToArray());
        }


        private static Vector3 GetRandomFromList(Transform[] points) =>
            points[UnityEngine.Random.Range(0, points.Length)].position;
    }
}
