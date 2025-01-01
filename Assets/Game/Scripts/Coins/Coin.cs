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


            if (isClient && NetworkClient.isConnected) // Убедитесь, что клиент активен и подключен
            {
                CmdCollectCoin();
            }
            else
            {
                Debug.LogWarning("Клиент не активен или не подключен.");
            }

            // if (player.isLocalPlayer)
            // {
            //     Debug.LogWarning("its local player");
            //     player.CmdCollectCoin(player.netId, Points);
            // }
            // else
            // {
            //     Debug.LogWarning("Cannot call command from a non-local player.");
            // }
        }

        [Command(requiresAuthority = false)]
        void CmdCollectCoin()
        {
            if (isServer) // Убедитесь, что код выполняется на сервере
            {
                Debug.LogWarning("Попытка вызвать серверную функцию на сервере.");
                GameManager.Instance.UnSpawn(this);
            }
            else
            {
                Debug.LogWarning("Попытка вызвать серверную функцию на клиенте.");
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
