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
    }

    public class GameManager : CustomNetworkBehaviour, IGameManager
    {
        [SerializeField] private PlayerCharacter playerPrefab;
        [SerializeField] private Dep dep;

        private readonly Dictionary<uint, int> _scores = new();

        public ICoinsManager CoinsManager { get; private set; }

        [Server]
        public void CollectCoin(uint id, int points)
        {
            Debug.LogWarning(GetHashCode());
            if (points <= 0) return;

            if (!_scores.TryAdd(id, points)) _scores[id] += points;

            foreach (var pair in _scores)
            {
                Debug.LogWarning($"Player {pair.Key} collected coin({pair.Value} pts). Now: {_scores[pair.Key]} pts.");
            }
        }


        [ClientRpc]
        private void ShowScoresRpc()
        {
            foreach (var pair in _scores)
            {
                Debug.LogWarning("===");
                Debug.LogWarning("Player " + pair.Key + " collected coin(" + pair.Value + "pts). Now: " +
                                 _scores[pair.Key] + " pts.");
                Debug.LogWarning("===");
            }
        }

        protected override void LoadDependencies()
        {
            Debug.LogWarning("<color=red>InitDependencies in GameManager</color>");
            CoinsManager = dep.GetDependency<ICoinsManager>();
            Debug.LogWarning("<color=red>CoinsManager: " + CoinsManager + "</color>");
        }
    }
}
