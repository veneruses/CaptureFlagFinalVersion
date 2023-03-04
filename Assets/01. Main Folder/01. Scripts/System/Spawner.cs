using System;
using System.Collections;
using System.Collections.Generic;
using CaptureFlagGame;
using Mirror;
using Unity.Mathematics;
using UnityEngine;

namespace CaptureFlagGame
{
    public class Spawner : NetworkBehaviour
    {
        public static Spawner Instance;

        private void Awake()
        {
            //SINGLETON
            if (Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(this);
        }

        [Server]
        public List<Flag> ServerSpawnFlags(int count,Action<Flag> actionWithFlag)
        {
            List<Flag> flags = new List<Flag>();
            for (int spawnCounter = 0; spawnCounter < count; spawnCounter++)
            {
                Transform flagSpawnPoint = ServerNetworkManager.Instance.GetFlagSpawnPoint().transform;
                Flag newFlag = Instantiate(EntityManager.Instance.flagPrefab, flagSpawnPoint.position, quaternion.identity)
                    .GetComponent<Flag>();
                NetworkServer.Spawn(newFlag.gameObject);
                actionWithFlag?.Invoke(newFlag);
                flags.Add(newFlag);
            }

            return flags;
        }
    }
}
