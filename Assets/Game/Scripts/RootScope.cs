using System;
using Game.Scripts.Shared;
using Mirror;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game.Scripts
{
    public class RootScope : LifetimeScope
    {
        [SerializeField] private NetworkManager networkManager;
        [SerializeField] private SpawnManager spawnManager;


        protected override void Configure(IContainerBuilder builder)
        {
            base.Configure(builder);

            if (networkManager == null) throw new NullReferenceException($"NetworkManager is null. {name}");
            if (spawnManager == null) throw new NullReferenceException($"SpawnManager is null. {name}");

            builder.RegisterComponent(networkManager).AsSelf();
            builder.RegisterComponent(spawnManager).As<ISpawnManager>();
        }
    }
}
