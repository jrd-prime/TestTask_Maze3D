using System;
using Game.Scripts.Client;
using Game.Scripts.Server.Coin;
using Game.Scripts.Server.Score;
using Game.Scripts.Shared.Coins;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Server
{
    public sealed class GameManager : NetworkBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private CoinsManager coinsManager;
        [SerializeField] private ScoreManager scoreManager;

        private GameManager()
        {
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            if (coinsManager == null) throw new NullReferenceException(nameof(coinsManager));
            if (scoreManager == null) throw new NullReferenceException(nameof(scoreManager));
        }

        [Server]
        public void UnSpawn(CoinBase go) => coinsManager.UnSpawnCoin();

        [Server]
        public void CollectCoin(CoinBase coin, PlayerCharacter playerCharacter, int points)
        {
            Debug.LogWarning($"add points {points} to player {playerCharacter} on server");
            scoreManager.AddPointsToPlayer(playerCharacter.netId, points);
            UnSpawn(coin);
            coinsManager.SpawnCoin();

            if (!scoreManager.GetCurrentScore(playerCharacter.netId, out var currentScore)) return;

            playerCharacter.ShowScoreForClientRpc(playerCharacter.connectionToClient, currentScore);
        }
    }
}
