using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLine : MonoBehaviour
{
    public Action<Collider2D> OnTriggerEnter2DAction;
    public Action<Collider2D> OnTriggerExit2DAction;
    private void OnTriggerEnter2D(Collider2D col)
    {
        OnTriggerEnter2DAction?.Invoke(col);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        OnTriggerExit2DAction?.Invoke(other);
    }
}
