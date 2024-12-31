using System;
using Mirror;
using UnityEngine;

namespace Game.Scripts.Coins
{
    public class Coin : CoinBase
    {
        [ServerCallback]
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

                // В любом случае отправляем команду на сервер для начисления очков.
                // Игрок может быть с authority или без.
                CmdCollectCoin(player.netIdentity, Points);
            }
        }

        public void Initialize(int pointsPerCoin, Transform parent)
        {
            transform.parent = parent;
            Points = pointsPerCoin;
            IsInitialized = true;
        }

// Команда на сервер для начисления очков.
        [Command(requiresAuthority = false)]
        public void CmdCollectCoin(NetworkIdentity player, int points)
        {
            var playerCharacter = player.GetComponent<PlayerCharacter>();
            if (playerCharacter != null)
            {
                playerCharacter.AddPoints(points);
            }

// Только для локального клиента выводим сообщение.
            if (isLocalPlayer)
            {
                Debug.LogWarning($"Player collected coin! ");
            }

            // Можно удалять монетку после того, как её собрали.
            NetworkServer.Destroy(gameObject);
        }

        private void OnDestroy() => IsInitialized = false;
    }
}
