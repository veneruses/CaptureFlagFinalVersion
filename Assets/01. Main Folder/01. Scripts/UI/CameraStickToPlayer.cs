using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraStickToPlayer : MonoBehaviour
{
    [SerializeField] private Camera m_camera;
    
    /// <summary>
    /// Make camera stick to player when player has been connected to server.
    /// </summary>
    /// <param name="target"></param>
    /// <param name="spotTransform"></param>
    public void StickToPlayerCameraSpot(NetworkConnection target,Transform spotTransform)
    {
        m_camera.transform.parent = spotTransform;
        m_camera.transform.position = spotTransform.position;
    }
}
