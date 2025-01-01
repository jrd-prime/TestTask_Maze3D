using System.Collections.Generic;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Server
{
    public class ScoreManager : NetworkBehaviour
    {
        private readonly Dictionary<uint, int> _playersScores = new();

        [Server]
        public void AddPointsToPlayer(uint id, int points)
        {
            if (_playersScores.ContainsKey(id))
            {
                Debug.LogWarning($"In cache. Add points {points} to player {id} on server");
                _playersScores[id] += points;
            }
            else
            {
                Debug.LogWarning($"Not in cache. Add points {points} to player {id} on server");
                _playersScores.Add(id, points);
            }
        }

        [Server]
        public bool GetCurrentScore(uint id, out int score) => _playersScores.TryGetValue(id, out score);
    }
}
