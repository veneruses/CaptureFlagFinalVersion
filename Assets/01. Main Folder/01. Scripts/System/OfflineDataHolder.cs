using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CaptureFlagGame
{
    public class OfflineDataHolder : MonoBehaviour
    {
        public static OfflineDataHolder Instance;
        public Action<string> OnIpChanged;
        public string Name
        {
            get { return m_name; }
            private set { m_name = value; }
        }
    
        public string IP
        {
            get { return m_ip; }
            private set
            {
                m_ip = value;
                OnIpChanged?.Invoke(value);
            }
        }
        [SerializeField] private string m_name;
        [SerializeField] private string m_ip;
        [SerializeField] private string m_ipDefault = "localhost";
        [SerializeField] private string m_nameDefault = "Player guest";
        
        private void Awake()
        {
            //SINGLETON
            if (Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(this);
            IP = m_ipDefault;
            Name = m_nameDefault;
        }
    
    
        public void SetName(string name)
        {
            Name = name;
        }
    
        public void SetIP(string ip)
        {
            IP = ip;
        }
        
        
    }
}

