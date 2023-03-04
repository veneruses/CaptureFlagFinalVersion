using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Image = UnityEngine.UI.Image;
using Random = UnityEngine.Random;

namespace CaptureFlagGame
{
    public class MinigameController : NetworkBehaviour
    {
        [SerializeField] private string m_greenLineTag;
        [SerializeField] private string m_backgroundImageTag;
        [SerializeField] private Image m_backgroundImage;
        [SerializeField] private Image m_greenLineImage;
        [SerializeField] private Image m_redLineImage;
        [SerializeField] private RedLine m_redLine;
        [SyncVar] [SerializeField] private bool m_isRedlineInGreenLine = false;
        [SyncVar] [SerializeField] private bool m_mousePressed = false;

        [SerializeField]
        private float m_redLineSpeed = 5;

        [SerializeField] private float m_maxMinigameTime = 1.5f;
        [SyncVar] [SerializeField] private float m_currentMinigameTime = 1.5f;
        public float stopCapturingOnFailFlagTime = 2f;

        [SyncVar] public bool isMinigameStarted = false;

        [SerializeField]
        private GameObject m_miniGameWindow;

        private Vector3 m_redLineDirection;
        public Action OnMinigameLoose;


        #region Monobehaviour methods
        private void Awake()
        {
            m_backgroundImage = References.Instance.BackgroundImage;
            m_greenLineImage = References.Instance.GreenLineImage;
            m_redLineImage = References.Instance.RedLineImage;
            m_redLine = References.Instance.RedLine;
            m_miniGameWindow = References.Instance.MiniGameWindow;
        }
        
        private void Update()
        {
            if (isLocalPlayer)
            {
                if (isMinigameStarted)
                {
                    m_currentMinigameTime -= Time.deltaTime;
                    if (m_currentMinigameTime <= 0)
                    {
                        OnLoose();
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        m_mousePressed = true;
                    }

                    MoveRedLine();
                    CheckIfWin();
                }
            }
        }

        #endregion

        #region NetworkBehaviour methods

        public override void OnStartServer()
        {
            if (isServer)
            {
                ServerAddMinigameController();
                OnMinigameLoose +=
                    () =>
                    {
                        //Update hold capturing time on all players flags to stop capturing all flags for this specific player
                        EntityManager.Instance.GetPlayerFlags(GetComponent<NetworkIdentity>().connectionToClient.connectionId).ForEach(
                            (flag) => {flag.HoldCapturingTime = stopCapturingOnFailFlagTime; });
                    };
            }
        }

        public override void OnStartClient()
        {
            if (isClient)
            {
                m_redLine.OnTriggerEnter2DAction += (collider) => CheckIfRedLineInGreenLine(collider.tag);
                m_redLine.OnTriggerExit2DAction += (collider) =>
                {
                    if (collider.tag == m_backgroundImageTag)
                    {
                        SetRedLineDirection(m_greenLineImage.transform.position.x, m_redLineImage.transform.position.x);
                    }

                    if (collider.tag == m_greenLineTag)
                    {
                        m_isRedlineInGreenLine = false;
                    }
                };
            }
        }
        #endregion

        

       

        [Server]
        private void ServerAddMinigameController()
        {
            NetworkIdentity identity = GetComponent<NetworkIdentity>();
            EntityManager.Instance.AddMinigameController(identity.connectionToClient.connectionId, this);
        }

        


        [Command]
        private void CmdResetStates()
        {
            ServerResetStates();
        }

        [Server]
        private void ServerResetStates()
        {
            m_mousePressed = false;
            m_isRedlineInGreenLine = false;
            isMinigameStarted = false;
            m_currentMinigameTime = m_maxMinigameTime;
            RpcResetStates();
        }

        [ClientRpc]
        public void RpcResetStates()
        {
            m_mousePressed = false;
            m_isRedlineInGreenLine = false;
            isMinigameStarted = false;
            m_currentMinigameTime = m_maxMinigameTime;
        }

        public void ResetLinesPositions(Transform redLine, Transform greenLine, Collider2D background)
        {
            Vector3[] backgroundCorners = new Vector3[4];
            background.GetComponent<RectTransform>().GetWorldCorners(backgroundCorners);

            float backgroundMinX = backgroundCorners[0].x; //left x position of backGround
            float backgroundMaxX = backgroundCorners[2].x; //right x position of backGround
            float backgroundCenterX =
                backgroundMinX + (backgroundMaxX - backgroundMinX) / 2; //center x position of backGround

            Vector3 greenLineNewPos = new Vector3(Random.Range(backgroundCenterX, backgroundMaxX), greenLine.position.y,
                greenLine.position.z);
            Vector3 redLineNewPos = new Vector3(Random.Range(backgroundMinX, backgroundCenterX), redLine.position.y,
                redLine.position.z);

            greenLine.position = greenLineNewPos;
            redLine.position = redLineNewPos;
        }

        [Server]
        public void ServerActivateMiniGameWindow(NetworkConnectionToClient conn, bool state)
        {
            isMinigameStarted = state;
            TargetActivateMinigameWindowOnClient(conn, state);
        }
        
        

        [TargetRpc]
        private void TargetActivateMinigameWindowOnClient(NetworkConnectionToClient conn, bool state)
        {
            m_miniGameWindow.SetActive(state);
            if (state)
                OnGameStart();
            else
            {
                OnMinigameFinish();
            }
        }

        public void OnGameStart()
        {
            ResetLinesPositions(m_redLineImage.transform, m_greenLineImage.transform,
                m_backgroundImage.GetComponent<Collider2D>());
            SetRedLineDirection(m_greenLineImage.transform.position.x, m_redLineImage.transform.position.x);
        }

        public void OnMinigameFinish()
        {
            m_miniGameWindow.SetActive(false);
            isMinigameStarted = false;
            CmdResetStates();
        }


        public void OnWin()
        {
            CmdSendServerWin(DataHolderObject.Instance.GetPlayerId());
            OnMinigameFinish();
        }

        [Command]
        public void CmdSendServerWin(int networkConnectionIdToClient)
        {
            //Debug.Log(EntityManager.Instance.GetPlayer(networkConnectionIdToClient).name + " Выиграл");
        }

        [Command]
        public void CmdSendServerLoose(int connectionToClient)
        {
            ServerOnLoose(connectionToClient);
        }

        [Server]
        public void ServerOnLoose(int connectionToClient)
        {
            if (isServer)
            {
                OnMinigameLoose?.Invoke();
                string message = "Игрок "+EntityManager.Instance.GetPlayer(connectionToClient).name + " из другой команды проиграл мини-игру";
                NetworkConnectionToClient playerToNotShowMessage = EntityManager.Instance.GetPlayer(connectionToClient).connectionToClient;
                Notification.Instance.SendAllExceptSomebody(message,playerToNotShowMessage);
            }
                
        }

        public void OnLoose()
        {
            CmdSendServerLoose(DataHolderObject.Instance.GetPlayerId());
            OnMinigameFinish();
        }


        public void MoveRedLine()
        {
            m_redLineImage.transform.Translate(m_redLineDirection * m_redLineSpeed * Time.deltaTime);
        }

        private void SetRedLineDirection(float greenLineXPos, float redLineXPos)
        {
            if (greenLineXPos > redLineXPos)
            {
                m_redLineDirection = Vector3.right;
            }
            else
            {
                m_redLineDirection = Vector3.left;
            }
        }


        private void CheckIfRedLineInGreenLine(string tag)
        {
            if (tag == m_greenLineTag)
            {
                m_isRedlineInGreenLine = true;
            }
        }

        private void CheckIfWin()
        {
            if (m_mousePressed)
            {
                if (m_isRedlineInGreenLine)
                {
                    OnWin();
                }
                else
                {
                    OnLoose();
                }
            }
        }
        
    }
}