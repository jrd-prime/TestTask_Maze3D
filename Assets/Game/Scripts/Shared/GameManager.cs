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

        private readonly Dictionary<uint, int> _scores = new();

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
        public void UnSpawn(CoinBase go) => coinsManager.UnSpawnCoin(go);
        //
        // [Server]
        // public void CollectCoin(uint playerId, int points)
        // {
        //     if (points <= 0) return;
        //
        //     // Проверяем наличие ключа в словаре
        //     if (!_scores.ContainsKey(playerId))
        //     {
        //         Debug.LogWarning($"Player {playerId} is not in the score list. Adding with initial score of 0.");
        //         _scores[playerId] = 0; // Добавляем игрока с начальным значением очков
        //     }
        //
        //     // Обновляем очки игрока
        //     _scores[playerId] += points;
        //
        //     Debug.LogWarning($"Player {playerId} collected {points} pts. Total: {_scores[playerId]} pts.");
        // }

        // [Server]
        // public void ShowScoreToClient(NetworkConnection clientConnection, uint playerId)
        // {
        //     if (_scores.TryGetValue(playerId, out int score))
        //     {
        //         ShowScoreForClientRpc(clientConnection, score);
        //     }
        //     else
        //     {
        //         Debug.LogWarning($"Player {playerId} has no score recorded.");
        //     }
        // }

        // [Server]
        // public void ShowScoresToClients()
        // {
        //     var scoresList = new List<(uint id, int score)>(_scores.Count);
        //     foreach (var pair in _scores)
        //     {
        //         scoresList.Add((pair.Key, pair.Value));
        //     }
        // }
        //


        [Server]
        public void CollectCoin(CoinBase coin, PlayerCharacter playerCharacter, int points)
        {
            Debug.LogWarning($"add points {points} to player {playerCharacter} on server");
            scoreManager.AddPointsToPlayer(playerCharacter.netId, points);
            UnSpawn(coin);

            if (!scoreManager.GetCurrentScore(playerCharacter.netId, out var currentScore)) return;

            playerCharacter.ShowScoreForClientRpc(playerCharacter.connectionToClient, currentScore);
        }
    }
}
