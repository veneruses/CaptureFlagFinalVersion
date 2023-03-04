using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEditor;


namespace CaptureFlagGame
{
    [CustomEditor(typeof(NetworkStartPosition))]
    public class NetworkStartPositionInspector : Editor
    {
        NetworkStartPosition networkStartPosition;
        private Vector3 lastPosition;

        private void OnEnable()
        {
            networkStartPosition = (NetworkStartPosition)target;
        }

        public override void OnInspectorGUI()
        {
            Vector3 position = networkStartPosition.transform.position;
            networkStartPosition.gameObject.name = $"Player Spawn Position [ x:{position.x}, z:{position.z} ]";
        }
    }
}
