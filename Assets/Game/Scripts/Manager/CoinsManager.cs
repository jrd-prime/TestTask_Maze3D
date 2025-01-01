using System;
using System.Collections;
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
        private CoinSpawner coinSpawner;

        private readonly Queue<CoinBase> _coinsCache = new();

        private void Awake()
        {
            if (settings == null) throw new NullReferenceException(nameof(settings));
            coinSpawner = new CoinSpawner(settings, transform);
        }

        private void Start()
        {
            Debug.LogWarning("CoinsManager started.");

            StartCoroutine(SpawnFirstCoin());

            InstantiateCoins();
        }

        private IEnumerator SpawnFirstCoin()
        {
            Debug.LogWarning("Spawning first coin pre delay.");
            yield return new WaitForSeconds(settings.onStartSpawnDelaySeconds);
            Debug.LogWarning("Spawning first coin.");
            coinSpawner.SpawnCoin();
        }


        private void InstantiateCoins()
        {
            Debug.LogWarning("Instantiating coins.");

            foreach (var spawnPoint in settings.spawnPoints)
            {
                var coin = Instantiate(settings.coinPrefab, spawnPoint.position, Quaternion.identity);
                _coinsCache.Enqueue(coin);
            }
        }

        [Server]
        public void UnSpawnCoin() => coinSpawner.UnSpawnCoin();
    }
}
