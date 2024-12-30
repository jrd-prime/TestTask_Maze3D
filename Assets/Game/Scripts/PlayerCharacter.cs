using System;
using Mirror;
using UnityEngine;

namespace Game.Scripts
{
    public sealed class PlayerCharacter : NetworkBehaviour
    {
        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;

            var x = Input.GetAxis("Horizontal");
            var z = Input.GetAxis("Vertical");
            transform.Translate(new Vector3(x, 0, z) * Time.fixedDeltaTime * 5);
        }
    }
}
