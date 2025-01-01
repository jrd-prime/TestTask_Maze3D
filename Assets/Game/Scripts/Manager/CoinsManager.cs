using System;
using Game.Scripts.Data.SO;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Manager
{
    public interface ICoinsManager
    {
    }

    public class CoinsManager : NetworkBehaviour, ICoinsManager
    {
        [SerializeField] private CoinsManagerSO settings;

        private void Awake()
        {
            if (settings == null) throw new NullReferenceException(nameof(settings));
        }

        private void Start()
        {
            Debug.LogWarning("CoinsManager started.");

            foreach (var spawnPoint in settings.spawnPoints)
            {
                var coin = Instantiate(settings.coinPrefab, spawnPoint.position, Quaternion.identity);
                coin.Initialize(settings.pointsPerCoin, transform);
                NetworkServer.Spawn(coin.gameObject);

                Debug.Log($"Coin spawned with netId: {coin.netId}");
            }
        }
    }
}
