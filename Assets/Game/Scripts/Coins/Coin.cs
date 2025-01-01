using System;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Coins
{
    public class Coin : CoinBase
    {
        [Client]
        private void OnTriggerEnter(Collider other)
        {
            if (isClient)
            {
                Debug.LogWarning("its client");
                Debug.LogWarning("authority: " + authority);
            }

            if (!IsInitialized) throw new Exception("Coin is not initialized! Use Initialize() method first.");
            
            Debug.LogWarning("Coin " + gameObject.name + " triggered.");

            if (((1 << other.gameObject.layer) & triggerMask) == 0) return;

            Debug.LogWarning("collided with " + other.gameObject.layer);
            var player = other.gameObject.GetComponent<PlayerCharacter>();

            if (player == null) throw new NullReferenceException("Player not found!");

            if (player.isLocalPlayer)
            {
                Debug.LogWarning("its local player");
                player.CmdCollectCoin(player.netId, Points);
                NetworkServer.Destroy(gameObject);
            }
            else
            {
                Debug.LogWarning("Cannot call command from a non-local player.");
            }
        }

        public void Initialize(int pointsPerCoin, Transform parent)
        {
            Debug.LogWarning("Coin " + gameObject.name + " initialized.");
            transform.parent = parent;
            Points = pointsPerCoin;
            IsInitialized = true;
        }

        private void OnDestroy() => IsInitialized = false;
    }
}
