using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.Mathematics;
using Unity.VisualScripting;
using Random = System.Random;

namespace CaptureFlagGame
{
    public class ServerNetworkManager : NetworkManager
    {
        public Action OnClientConnected;
        public Action OnClientDisconnected;
        public static ServerNetworkManager Instance;
        public List<FlagSpawnPosition> FlagSpawnPositions = new List<FlagSpawnPosition>();

        #region Monobehaviour methods

        public override void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(this);
        }

        public override void Start()
        {
            base.Start();
            OfflineDataHolder.Instance.OnIpChanged += (string ip) => { networkAddress = ip; };
        }

        #endregion

        #region NetworkBehaviour methods

        public override void OnStartServer()
        {
            base.OnStartServer();
            NetworkServer.RegisterHandler<InstantiateCharacterMessage>(OnCreateCharacter);
        }

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            InstantiateCharacterMessage instantiateCharacterMessage = new InstantiateCharacterMessage
            {
                Name = OfflineDataHolder.Instance.Name
            };
            NetworkClient.connection.Send(instantiateCharacterMessage);
            OnClientConnected?.Invoke();
        }

        public override void OnStopClient()
        {
            OnClientDisconnected?.Invoke();
        }
        
        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            base.OnServerDisconnect(conn);

            EntityManager.Instance.RemovePlayer(conn.connectionId);
            EntityManager.Instance.DestroyPlayersEntities(conn.connectionId);
        }
        #endregion
        
        
        private void OnCreateCharacter(NetworkConnectionToClient connection,
            InstantiateCharacterMessage instantiateCharacterMessage)
        
        {
            Transform spawner = GetStartPosition();
            Player player = Instantiate(playerPrefab, spawner.position, quaternion.identity).GetComponent<Player>();
            player.Name = instantiateCharacterMessage.Name;
            player.Text.text = instantiateCharacterMessage.Name;
            player.gameObject.name = instantiateCharacterMessage.Name;
            NetworkServer.AddPlayerForConnection(connection, player.gameObject);

            NetworkIdentity netIdentity = player.GetComponent<NetworkIdentity>();
            netIdentity.AssignClientAuthority(connection);

            DataHolderObject.Instance.SetPlayerConnectionId(connection, connection.connectionId);
            EntityManager.Instance.AddPlayer(player, connection.connectionId);

            Entity.EntityColorData playerColoredData =
                DataHolderObject.Instance.GetAvailableColoredData(EntityManager.Instance.GetPlayerCount());

            Spawner.Instance.ServerSpawnFlags(3, (flag) =>
            {
                flag.playerOwner = player;
                EntityManager.Instance.AddEntityOfPlayer(connection.connectionId, flag);
                flag.ServerSetColoredMaterial(playerColoredData);
            });

            player.ServerSetColoredMaterial(playerColoredData);
            Debug.Log(instantiateCharacterMessage.Name + " connected");
        }
        
        public void RegisterFlagSpawnPosition(FlagSpawnPosition flagSpawnPosition)
        {
            FlagSpawnPositions.Add(flagSpawnPosition);
        }

        public void UnregisterFlagSpawnPosition(FlagSpawnPosition flagSpawnPosition)
        {
            FlagSpawnPositions.Remove(flagSpawnPosition);
        }

        public FlagSpawnPosition GetFlagSpawnPoint()
        {
            foreach (var flagSpawn in FlagSpawnPositions)
            {
                if (flagSpawn.IsAvailable)
                {
                    flagSpawn.IsAvailable = false;
                    return flagSpawn;
                }
            }

            return null;
        }
    }
}