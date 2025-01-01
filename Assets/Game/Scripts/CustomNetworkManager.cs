using Mirror;
using UnityEngine;

namespace Game.Scripts.Server
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private GameManager gameManager;
    }
}
