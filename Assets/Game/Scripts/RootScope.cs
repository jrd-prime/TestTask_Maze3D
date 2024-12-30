using System;
using Mirror;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;


        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            if (networkManager == null) throw new NullReferenceException($"NetworkManager is null. {name}");

            builder.RegisterComponent(networkManager).AsSelf();
        }
    }
}
