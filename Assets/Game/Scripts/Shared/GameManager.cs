using System;
using System.Collections.Generic;
using Game.Scripts.Coins;
using Game.Scripts.Help;
using Game.Scripts.Manager;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Game.Scripts.Shared
{
    public interface IGameManager
    {
    }

    public class GameManager : CustomNetworkBehaviour, IGameManager
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
