using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyJoystick;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

namespace CaptureFlagGame
{
    public class References : MonoBehaviour
    {
        public static References Instance;

        public Canvas playerCanvas;
        public Joystick joystick;
        public CameraStickToPlayer CameraStickToPlayer;
        public MinigameController MinigameController;
        public Image BackgroundImage;
        public Image GreenLineImage;
        public Image RedLineImage;
        public RedLine RedLine;
        public GameObject MiniGameWindow;


        private void Awake()
        {
            //SINGLETON
            if (Instance == null)
            {
                Instance = this;
            }
            else
                Destroy(this);
        }
    }
}