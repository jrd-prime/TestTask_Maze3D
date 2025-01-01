using System;
using Game.Scripts.Shared;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Coins
{
    public class Coin : CoinBase
    {
        [ClientCallback]
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


            if (isClient && NetworkClient.isConnected)
            {
                CmdCollectCoin(player, Points);
            }
            else
            {
                Debug.LogWarning("Клиент не активен или не подключен.");
            }
        }


        [Command(requiresAuthority = false)]
        void CmdCollectCoin(PlayerCharacter player, int points)
        {
            if (!isServer) return;
            GameManager.CollectCoin(this, player, points);
        }

        public void Initialize(int pointsPerCoin)
        {
            Points = pointsPerCoin;
            IsInitialized = true;
        }

        private void OnDestroy() => IsInitialized = false;
    }
}
