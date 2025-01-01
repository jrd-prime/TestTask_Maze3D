using Mirror;
using UnityEngine;

namespace Game.Scripts.Shared
{
    public class CustomNetworkManager : NetworkManager
    {
        [SerializeField] private GameManager gameManager;
    }
}
