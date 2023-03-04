using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using EasyJoystick;
using Mirror;

public class Movement : NetworkBehaviour
{
    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private float m_speed = 5f;
    public EasyJoystick.Joystick m_joystick;
    private float xMovement;
    private float zMovement;


    public void Update()
    {
        if (isLocalPlayer)
        {
            xMovement = m_joystick.Horizontal();
            zMovement = m_joystick.Vertical();
        }
    }

    private void FixedUpdate()
    {
        //Vector3 dir = new Vector3(xMovement, 0, zMovement);
        Vector3 locVel = transform.TransformDirection(new Vector3(xMovement, m_rigidbody.velocity.y, zMovement));
        m_rigidbody.velocity = locVel * m_speed;
    }
}