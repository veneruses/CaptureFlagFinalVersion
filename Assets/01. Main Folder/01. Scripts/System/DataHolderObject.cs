using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace CaptureFlagGame
{
    public class DataHolderObject : NetworkBehaviour
    {
        public static DataHolderObject Instance;
        [SerializeField] private int m_playerConnectionId;
        [SerializeField] private List<MaterialById> m_materialsWithId = new List<MaterialById>();
        [SerializeField] private List<Entity.EntityColorData> m_entityColorData = new List<Entity.EntityColorData>();
        
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

        public void SetPlayerId(int id)
        {
            m_playerConnectionId = id;
        }
        
        public int GetPlayerId()
        {
            return m_playerConnectionId;
        }
        public Material GetMaterialById(ushort id)
        {
            foreach (var current  in m_materialsWithId)
            {
                if (current.Id == id)
                {
                    return current.Material;
                }
            }

            return null;
        }
        
        
        [Server]
        public Entity.EntityColorData GetAvailableColoredData(int playerOrder)
        {
            if (playerOrder >= m_materialsWithId.Count)
            {
                return m_entityColorData[m_materialsWithId.Count - 1];
            }
            
            return m_entityColorData[playerOrder-1];
        }
        
        [Serializable]
        public struct MaterialById
        {
            public ushort Id;
            public Material Material;
        }
        
        [TargetRpc]
        public void SetPlayerConnectionId(NetworkConnectionToClient conn, int id)
        {
            SetPlayerId(id);
        }
    }
}