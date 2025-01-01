using System;
using System.Collections.Generic;
using System.Diagnostics;
using Game.Scripts.Coins;
using Game.Scripts.Data.SO;
using Mirror;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace Game.Scripts.Manager
{
    public interface ICoinsManager
    {
    }

    public class CoinsManager : NetworkBehaviour, ICoinsManager
    {
        [SerializeField] private CoinsManagerSO settings;

        private readonly Queue<CoinBase> _coinsCache = new();

        private void Awake()
        {
            if (settings == null) throw new NullReferenceException(nameof(settings));
        }

        private void Start()
        {
            Debug.LogWarning("CoinsManager started.");
            InstantiateCoins();
            SpawnAllCoins();
        }

        [Server]
        private void SpawnAllCoins()
        {
            Debug.LogWarning("Spawning all coins.");
            var coinsList = _coinsCache.ToArray();
            for (var i = 0; i < coinsList.Length; i++) SpawnCoin();
        }

        private void InstantiateCoins()
        {
            Debug.LogWarning("Instantiating coins.");

            foreach (var spawnPoint in settings.spawnPoints)
            {
                var coin = Instantiate(settings.coinPrefab, spawnPoint.position, Quaternion.identity);
                coin.Initialize(settings.pointsPerCoin, transform);
                _coinsCache.Enqueue(coin);
            }
        }

        [Server]
        private void SpawnCoin()
        {
            var coin = _coinsCache.Dequeue();
            NetworkServer.Spawn(coin.gameObject);
        }


        [Server]
        public void UnSpawnCoin(CoinBase coinBase)
        {
            _coinsCache.Enqueue(coinBase);
            NetworkServer.UnSpawn(coinBase.gameObject);
        }

        [Server]
        public void AddPointsToPlayer(uint playerCharacterNetId, int points)
        {
            throw new NotImplementedException();
        }
    }
}
