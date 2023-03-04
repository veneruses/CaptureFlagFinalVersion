using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Rotation : NetworkBehaviour
{
    [SerializeField] private float m_sensitivity;

    void Update()
    {
        if (isLocalPlayer)
        {
            float rotationHorizontal = Input.GetAxis("Mouse X");
            transform.Rotate(new Vector3(0,rotationHorizontal * m_sensitivity , 0));
        }
    }
}