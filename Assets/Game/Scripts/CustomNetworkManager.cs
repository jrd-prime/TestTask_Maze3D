using Game.Scripts.Server;
using Mirror;
using UnityEngine;

namespace Game.Scripts
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private GameManager gameManager;
    }
}
