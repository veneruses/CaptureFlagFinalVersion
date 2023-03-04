using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using System.Linq;
using UnityEngine.Serialization;

namespace CaptureFlagGame
{
    public class EntityManager : NetworkBehaviour
    {
        public static EntityManager Instance;
        public Flag flagPrefab;
        
        //Players with key as NetworkConnectionToClient.connectionId
        private Dictionary<int, Player> m_players = new Dictionary<int, Player>();

        //Objects connected to players;
        private Dictionary<int, List<Entity>> m_playersEntities = new Dictionary<int, List<Entity>>(); 

        //Minigame controller of player with key as NetworkConnectionToClient.connectionId
        private Dictionary<int, MinigameController> m_playersMiniGameControllers = new Dictionary<int, MinigameController>();

        #region Monobehavior methods

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

        #endregion

        #region EntityManager methods

        [Server]
        public void AddPlayer(Player player, int connectionId)
        {
            if (!m_players.ContainsKey(connectionId))
            {
                m_players.Add(connectionId, player);
            }
        }

        [Server]
        public void RemovePlayer(int connectionId)
        {
            if (m_players.ContainsKey(connectionId))
            {
                m_players.Remove(connectionId);
            }
        }

        [Server]
        public List<Player> GetAllPlayers()
        {
            List<Player> playersToReturn = new List<Player>();
            foreach (var player in this.m_players.Values)
            {
                playersToReturn.Add(player);
            }

            return playersToReturn;
        }


        [Server]
        public Player GetPlayer(int connectionId)
        {
            bool contains = m_players.ContainsKey(connectionId);
            if (m_players.ContainsKey(connectionId))
            {
                return m_players[connectionId];
            }

            return null;
        }

        [Server]
        public void AddEntityOfPlayer(int id, Entity entity)
        {
            if (m_playersEntities.ContainsKey(id))
            {
                m_playersEntities[id].Add(entity);
            }
            else
            {
                m_playersEntities.Add(id, new List<Entity>());
                m_playersEntities[id].Add(entity);
            }
        }

        [Server]
        public void DestroyPlayersEntities(int id)
        {
            if (m_playersEntities.ContainsKey(id))
            {
                m_playersEntities[id].ForEach((Entity entity) => { NetworkServer.Destroy(entity.gameObject); });
            }

            m_playersEntities.Remove(id);
        }

        [Server]
        public List<Flag> GetPlayerFlags(int id)
        {
            List<Flag> playersFlags = new List<Flag>();
            if (m_playersEntities.ContainsKey(id))
            {
                m_playersEntities[id].ForEach((Entity entity) =>
                {
                    if (entity is Flag) playersFlags.Add((Flag)entity);
                });
            }

            return playersFlags;
        }

        [Server]
        public MinigameController GetPlayerMinigameController(int id)
        {
            if (m_playersMiniGameControllers.ContainsKey(id))
            {
                return m_playersMiniGameControllers[id];
            }

            return null;
        }

        [Server]
        public void AddMinigameController(int id, MinigameController minigameController)
        {
            if (m_playersMiniGameControllers.ContainsKey(id))
            {
                Debug.Log($"Player {m_players[id].Name} already has minigameController");
                return;
            }

            m_playersMiniGameControllers.Add(id, minigameController);
        }

        [Server]
        public int GetPlayerCount()
        {
            return m_players.Count;
        }

        #endregion
    }
}