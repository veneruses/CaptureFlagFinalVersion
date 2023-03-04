using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Mirror;
using Unity.VisualScripting;
using UnityEngine.Serialization;
using Random = System.Random;

namespace CaptureFlagGame
{
    public class Flag : Entity
    {
        #region Fields

        [Header("Flag capturing data")] [SerializeField]
        public bool IsCaptured;

        [SerializeField] [SyncVar] public float HoldCapturingTime;
        [SerializeField] private float m_range = 3f;
        [SerializeField] private float m_captureTime = 5f;
        [SerializeField] private int m_maxMinigamesPerFlag = 1;
        [SerializeField] private int m_currentMinigamesCount;
        [SerializeField] private float m_minMinigameStartTime = 2f;
        [SerializeField] private float m_timeFromCaptureBegin;
        [SerializeField] private float m_capturingPercent;
        private Coroutine m_startMinigameCoroutine;

        [Header("References")] 
        [SerializeField] public Player playerOwner;

        [SerializeField] private MinigameController m_miniGameWindow;
        [SerializeField] private Image m_fillImage;
        [SerializeField] private SpriteRenderer m_radiusImage;
        [SerializeField] private SpriteRenderer m_CircleTopImage;

        #endregion


        #region Monobehaviour methods

        private void Start()
        {
            if (isServer)
            {
                m_miniGameWindow =
                    EntityManager.Instance.GetPlayerMinigameController(playerOwner.connectionToClient.connectionId);
                
            }
        }

        private void Update()
        {
            if (!isServer) return;
            if (playerOwner)
            {
                if (HoldCapturingTime > 0)
                {
                    HoldCapturingTime -= Time.deltaTime;
                }
                else
                {
                    if (OwnerAndFlagDistance() >= m_range) return;

                    m_capturingPercent = ServerCalculateFlagCapturing();
                    if (m_capturingPercent >= 1 && IsCaptured == false)
                    {
                        IsCaptured = true;
                        OnFlagCaptured();
                    }

                    if (m_capturingPercent >= 1 && m_miniGameWindow.isMinigameStarted)
                    {
                        ServerStopMinigame(playerOwner.connectionToClient.connectionId);
                    }

                    RpcDrawFlagCapturingOnImage(m_capturingPercent);
                    ServerMinigameStarting();
                }
            }
        }

        private void OnValidate()
        {
            m_radiusImage.transform.localScale = new Vector3(m_range * 2, m_range * 2, 1);
        }

#if UNITY_EDITOR_WIN
        private void OnDrawGizmos()
        {
            Color c = Color.red;
            Handles.color = c;
            Handles.DrawWireDisc(transform.position, transform.up, m_range);
            c.a = 0.02f;
            Handles.color = c;
            Handles.DrawSolidDisc(transform.position, transform.up, m_range);
        }
#endif

        #endregion

        [Server]
        public void ServerStopMinigame(int playerId)
        {
            var playerConnectionToClient = EntityManager.Instance.GetPlayer(playerId).connectionToClient;
            m_miniGameWindow.ServerActivateMiniGameWindow(playerConnectionToClient, false);
        }

        [Server]
        private void ServerMinigameStarting()
        {
            if (OwnerAndFlagDistance() > m_range) return;
            if (m_startMinigameCoroutine != null) return;
            if (ServerCalculateFlagCapturing() >= 1) return;

            if (m_miniGameWindow.isMinigameStarted == false && m_currentMinigamesCount < m_maxMinigamesPerFlag)
            {
                m_startMinigameCoroutine = StartCoroutine(StartMinigame(m_minMinigameStartTime, m_miniGameWindow));
            }
        }

        [Server]
        private void OnFlagCaptured()
        {
            playerOwner.IncreaseCapturedFlags(1);
        }

        public float OwnerAndFlagDistance()
        {
            Vector3 differencePlayerAndFlag = playerOwner.transform.position - this.transform.position;
            float distancePlayerAndFlagInFloat = (float)Math.Sqrt(Math.Pow(differencePlayerAndFlag.x, 2) +
                                                                  Math.Pow(differencePlayerAndFlag.y, 2) +
                                                                  Math.Pow(differencePlayerAndFlag.z, 2));
            return distancePlayerAndFlagInFloat;
        }

        [Server]
        private float ServerCalculateFlagCapturing()
        {
            if (HoldCapturingTime > 0) return 0;

            if (OwnerAndFlagDistance() <= m_range)
            {
                m_timeFromCaptureBegin += Time.deltaTime;
            }

            return m_timeFromCaptureBegin / m_captureTime;
        }

        [Server]
        private IEnumerator StartMinigame(float time, MinigameController miniGameWindow)
        {
            yield return new WaitForSeconds(time);
            if (OwnerAndFlagDistance() > m_range) yield return null;
            if (ServerCalculateFlagCapturing() >= 1) yield return null;
            ServerStartMinigame(playerOwner.connectionToClient, miniGameWindow);
            m_startMinigameCoroutine = null;
        }

        [Server]
        private void ServerStartMinigame(NetworkConnectionToClient conn, MinigameController miniGameWindow)
        {
            m_currentMinigamesCount++;
            miniGameWindow.ServerActivateMiniGameWindow(conn, true);
        }

        [ClientRpc]
        private void RpcDrawFlagCapturingOnImage(float percent)
        {
            m_fillImage.fillAmount = percent;
        }


        public override void ServerSetColoredMaterial(EntityColorData data)
        {
            base.ServerSetColoredMaterial(data);
            m_mesh.material = DataHolderObject.Instance.GetMaterialById(data.MaterialId);
            m_radiusImage.color = data.Color;
            m_CircleTopImage.color = data.Color;
        }

        public override void SyncObjectState(EntityColorData oldData, EntityColorData newData)
        {
            m_mesh.material = DataHolderObject.Instance.GetMaterialById(newData.MaterialId);
            m_radiusImage.color = newData.Color;
            m_CircleTopImage.color = newData.Color;
        }
    }
}