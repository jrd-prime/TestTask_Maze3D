using System.Collections.Generic;
using System.Linq;
using Game.Scripts.Data.SO;
using Game.Scripts.Help;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Server.Coin
{
    public sealed class CoinSpawner : NetworkBehaviour
    {
        private List<Transform> _spawnPoints;

        private CustomPool<Shared.Coins.Coin> _coinsPool;
        private Dictionary<Transform, float> _distanceCache = new();

        private Vector3 _lastPoint = Vector3.zero;
        private Shared.Coins.Coin _currentCoin;
        private CoinsManagerSO _settings;

        public void Initialize(CoinsManagerSO settings)
        {
            _settings = settings;

            _spawnPoints = _settings.spawnPoints;
            _coinsPool = new CustomPool<Shared.Coins.Coin>(_settings.coinPrefab, _settings.spawnPointsCount,
                new GameObject("CoinsPool").transform);
        }

        [Server]
        public void SpawnCoin()
        {
            Debug.LogWarning("Coin Spawner: Spawn coin");
            _currentCoin = _coinsPool.Get();
            _currentCoin.Initialize(_settings.pointsPerCoin);

            var position = GetSpawnPosition();
            Debug.LogWarning("spawn position: " + position + " for coin: " + _currentCoin.gameObject.name);
            _currentCoin.transform.position = position + new Vector3(0, 1, 0);

            NetworkServer.Spawn(_currentCoin.gameObject);
        }

        [Server]
        public void UnSpawnCoin()
        {
            Debug.LogWarning("Coin Spawner: Unspawn coin");
            _coinsPool.Return(_currentCoin);
            NetworkServer.UnSpawn(_currentCoin.gameObject);
            _currentCoin = null;
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
            points[Random.Range(0, points.Length)].position;
    }
}
