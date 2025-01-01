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

        [SerializeField] private CoinSpawner _coinSpawner;

        private void Awake()
        {
            if (settings == null) throw new NullReferenceException(nameof(settings));
            if (_coinSpawner == null) throw new NullReferenceException(nameof(_coinSpawner));
        }

        private void Start()
        {
            _coinSpawner.Initialize(settings);


            Debug.LogWarning("CoinsManager started.");
            StartCoroutine(SpawnFirstCoin());
        }

        private IEnumerator SpawnFirstCoin()
        {
            Debug.LogWarning("Spawning first coin pre delay.");
            yield return new WaitForSeconds(settings.onStartSpawnDelaySeconds);
            Debug.LogWarning("Spawning first coin.");
            SpawnCoin();
        }

        [Server]
        public void UnSpawnCoin() => _coinSpawner.UnSpawnCoin();

        [Server]
        public void SpawnCoin() => _coinSpawner.SpawnCoin();
    }
}
