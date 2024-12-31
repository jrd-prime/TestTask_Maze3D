using System.Collections.Generic;
using Game.Scripts.Help;
using Game.Scripts.Manager;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Shared
{
    public interface IGameManager
    {
        public ICoinsManager CoinsManager { get; }
        public void CollectCoin(uint id, int points);
        void ShowScoresToClients();
    }

    public class GameManager : CustomNetworkBehaviour, IGameManager
    {
        [SerializeField] private PlayerCharacter playerPrefab;
        [SerializeField] private Dep dep;

        private readonly Dictionary<uint, int> _scores = new();

        public ICoinsManager CoinsManager { get; private set; }

        [Server]
        public void CollectCoin(uint playerId, int points)
        {
            if (points <= 0) return;

            // Проверяем наличие ключа в словаре
            if (!_scores.ContainsKey(playerId))
            {
                Debug.LogWarning($"Player {playerId} is not in the score list. Adding with initial score of 0.");
                _scores[playerId] = 0; // Добавляем игрока с начальным значением очков
            }

            // Обновляем очки игрока
            _scores[playerId] += points;

            Debug.LogWarning($"Player {playerId} collected {points} pts. Total: {_scores[playerId]} pts.");
        }

        [Server]
        public void ShowScoreToClient(NetworkConnection clientConnection, uint playerId)
        {
            if (_scores.TryGetValue(playerId, out int score))
            {
                ShowScoreForClientRpc(clientConnection, score);
            }
            else
            {
                Debug.LogWarning($"Player {playerId} has no score recorded.");
            }
        }

        [Server]
        public void ShowScoresToClients()
        {
            var scoresList = new List<(uint id, int score)>(_scores.Count);
            foreach (var pair in _scores)
            {
                scoresList.Add((pair.Key, pair.Value));
            }
        }

        [TargetRpc]
        private void ShowScoreForClientRpc(NetworkConnection target, int score)
        {
            Debug.LogWarning($"=== Score for You (Player {score}) ===");
            Debug.LogWarning($"Your Score: {score} pts.");
            Debug.LogWarning("=====================================");
        }

        protected override void LoadDependencies()
        {
            Debug.LogWarning("<color=red>InitDependencies in GameManager</color>");
            CoinsManager = dep.GetDependency<ICoinsManager>();
            Debug.LogWarning("<color=red>CoinsManager: " + CoinsManager + "</color>");
        }
    }
}
