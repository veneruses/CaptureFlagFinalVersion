using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;


namespace CaptureFlagGame
{
    [CustomEditor(typeof(FlagSpawnPosition))]
    public class FlagSpawnPositionInspector : Editor
    {
        FlagSpawnPosition flagSpawnPosition;
        private Vector3 lastPosition;

        private void OnEnable()
        {
            flagSpawnPosition = (FlagSpawnPosition)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            Vector3 position = flagSpawnPosition.transform.position;
            flagSpawnPosition.gameObject.name = $"Flag Spawn Position [ x:{position.x}, z:{position.z} ]";
        }
    }
}
