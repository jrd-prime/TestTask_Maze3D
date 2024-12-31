using System;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Coins
{
    public class Coin : CoinBase
    {
        private void OnTriggerEnter(Collider other)
        {
            if (!IsInitialized) throw new Exception("Coin is not initialized! Use Initialize() method first.");
            Debug.LogWarning("Coin " + gameObject.name + " triggered.");

            Debug.LogWarning(other);
            Debug.LogWarning(other.gameObject);
            Debug.LogWarning(other.gameObject.layer);


            if (((1 << other.gameObject.layer) & triggerMask) != 0)
            {
                Debug.LogWarning("collided with " + other.gameObject.layer);
                var player = other.gameObject.GetComponent<PlayerCharacter>();

                if (player == null)
                {
                    Debug.LogWarning("Player is null");
                    return;
                }

                // Если это не хост или не владелец объекта, не выполняем действия
                if (!player.authority)
                {
                    Debug.LogWarning("Player does not have authority to collect the coin.");
                    return;
                }

                player.CmdCollectCoin(Points);
            }
        }

        public void Initialize(int pointsPerCoin, Transform parent)
        {
            transform.parent = parent;
            Points = pointsPerCoin;
            IsInitialized = true;
        }

        private void OnDestroy() => IsInitialized = false;
    }
}
