using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
using UnityEngine.Serialization;

namespace CaptureFlagGame
{
    public class Player : Entity
    {
        [SyncVar(hook = nameof(NameHook))] public string Name;
        [SerializeField] public Transform CameraSpot;
        [SerializeField] public TMPro.TMP_Text Text;

        [SerializeField] [Tooltip("Amount of flags to consider game as winned")]
        private int flagsForWin = 3;

        [SerializeField] [Tooltip("Count of captured flags at this moment")]
        private int capturedFlags;

        [Server]
        public void IncreaseCapturedFlags(int amountFlags)
        {
            capturedFlags += amountFlags;
            if (capturedFlags >= flagsForWin)
            {
                OnPlayerWin();
            }
        }

        public void OnPlayerWin()
        {
            Notification.Instance.RpcSendMessageToAll($"{Name} победил");
        }

        public override void OnStartLocalPlayer()
        {
            References.Instance.playerCanvas.gameObject.SetActive(true);
            GetComponent<Movement>().m_joystick = References.Instance.joystick;
            References.Instance.CameraStickToPlayer.StickToPlayerCameraSpot(netIdentity.connectionToClient,
                CameraSpot);
        }

        public override void ServerSetColoredMaterial(EntityColorData data)
        {
            base.ServerSetColoredMaterial(data);
            m_mesh.material = DataHolderObject.Instance.GetMaterialById(data.MaterialId);
        }

        public override void SyncObjectState(EntityColorData oldData, EntityColorData newData)
        {
            m_mesh.material = DataHolderObject.Instance.GetMaterialById(newData.MaterialId);
        }

        public void NameHook(string oldName, string newName)
        {
            Text.text = newName;
        }
    }
}