using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

namespace CaptureFlagGame
{
    public class SetName : MonoBehaviour
    {
        [SerializeField] private TMP_InputField m_inputField;

        private void Awake()
        {
            m_inputField.onValueChanged.AddListener((string name) =>  OfflineDataHolder.Instance.SetName(name));
        }
    }
}


