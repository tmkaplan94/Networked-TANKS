﻿using UnityEngine;
using System.Collections;
using Photon.Pun;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public bool m_GameIsNetworked;
    public int m_NumRoundsToWin = 5;        
    public float m_StartDelay = 3f;         
    public float m_EndDelay = 3f;           
    public CameraControl m_CameraControl;   
    public Text m_MessageText;              
    public GameObject m_TankPrefab;
    public PhotonView m_ManagerView;
    public FollowNetworkTargets m_NetworkTargets;
    public TankManager[] m_Tanks;

    private int m_RoundNumber;              
    private WaitForSeconds m_StartWait;     
    private WaitForSeconds m_EndWait;       
    private TankManager m_RoundWinner;
    private TankManager m_GameWinner;       


    const float k_MaxDepenetrationVelocity = float.PositiveInfinity;

    private void Start()
    {
        // This line fixes a change to the physics engine.
        Physics.defaultMaxDepenetrationVelocity = k_MaxDepenetrationVelocity;
        
        m_StartWait = new WaitForSeconds(m_StartDelay);
        m_EndWait = new WaitForSeconds(m_EndDelay);

        if (m_GameIsNetworked)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("IsMasterClient = " + PhotonNetwork.IsMasterClient);
                SpawnNetworkedTanks();
                SetNetworkedCameraTargets();
            }
            else
            {
                SetNetworkedCameraTargets();
            }
        }
        else
        {
            SpawnAllTanks();
            SetCameraTargets();
        }
        // SpawnNetworkedTanks();
        // SetNetworkedCameraTargets();

        StartCoroutine(GameLoop());
    }
    
    private void SpawnAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].m_Instance = Instantiate(
                m_TankPrefab, m_Tanks[i].m_SpawnPoint.position, m_Tanks[i].m_SpawnPoint.rotation);
            m_Tanks[i].m_PlayerNumber = i + 1;
            m_Tanks[i].Setup();
        }
    }

    private void SpawnNetworkedTanks()
    {
        Debug.Log("SpawnedNetworkedTanks() was called");

        // if (PhotonNetwork.IsMasterClient)
        // {
        //     m_Tanks[0].m_Instance = PhotonNetwork.Instantiate(
        //         m_NetworkedTankPrefab.name, m_Tanks[0].m_SpawnPoint.position, m_Tanks[0].m_SpawnPoint.rotation);
        //     Debug.Log("PhotonNetwork.Instantiate() was called");
        //     m_NetworkTargets.AddToTargets(m_Tanks[0].m_Instance.transform);
        //     m_Tanks[0].Setup();
        // }
        // else
        // {
        //     m_Tanks[1].m_Instance = PhotonNetwork.Instantiate(
        //         m_NetworkedTankPrefab.name, m_Tanks[1].m_SpawnPoint.position, m_Tanks[1].m_SpawnPoint.rotation);
        //     Debug.Log("PhotonNetwork.Instantiate() was called");
        //     m_NetworkTargets.AddToTargets(m_Tanks[1].m_Instance.transform);
        //     m_Tanks[1].Setup();
        // }

        if (!PhotonNetwork.IsMasterClient) return;
        
        // spawn player 1
        m_Tanks[0].m_Instance = PhotonNetwork.Instantiate(
            "Networked Tank Blue", m_Tanks[0].m_SpawnPoint.position, m_Tanks[0].m_SpawnPoint.rotation);
        Debug.Log("PhotonNetwork.Instantiate() was called");
        m_NetworkTargets.AddToTargets(m_Tanks[0].m_Instance.transform);
        m_Tanks[0].Setup();
        
        // spawn player 2
        m_Tanks[1].m_Instance = PhotonNetwork.Instantiate(
            "Networked Tank Red", m_Tanks[1].m_SpawnPoint.position, m_Tanks[1].m_SpawnPoint.rotation);
        Debug.Log("PhotonNetwork.Instantiate() was called");
        m_NetworkTargets.AddToTargets(m_Tanks[1].m_Instance.transform);
        m_Tanks[1].Setup();
    }
    
    private void SetCameraTargets()
    {
        Transform[] targets = new Transform[m_Tanks.Length];

        for (int i = 0; i < targets.Length; i++)
        {
            targets[i] = m_Tanks[i].m_Instance.transform;
        }

        m_CameraControl.m_Targets = targets;
    }

    private void SetNetworkedCameraTargets()
    {
        m_CameraControl.m_Targets = m_NetworkTargets.GetTargetsArray();
    }

    private IEnumerator GameLoop()
    {
        yield return StartCoroutine(RoundStarting());
        yield return StartCoroutine(RoundPlaying());
        yield return StartCoroutine(RoundEnding());

        if (m_GameWinner != null)
        {
            if (m_GameIsNetworked)
            {
                PhotonNetwork.LoadLevel("Unite 2015 Networked");
            }
            else
            {
                SceneManager.LoadScene("Unite 2015");
            }
        }
        else
        {
            StartCoroutine(GameLoop());
        }
    }


    private IEnumerator RoundStarting()
    {
        // As soon as the round starts reset the tanks and make sure they can't move.
        ResetAllTanks ();
        DisableTankControl ();

        // Snap the camera's zoom and position to something appropriate for the reset tanks.
        m_CameraControl.SetStartPositionAndSize ();

        // Increment the round number and display text showing the players what round it is.
        m_RoundNumber++;
        m_MessageText.text = "ROUND " + m_RoundNumber;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_StartWait;
    }


    private IEnumerator RoundPlaying()
    {
        // As soon as the round begins playing let the players control the tanks.
        EnableTankControl ();

        // Clear the text from the screen.
        m_MessageText.text = string.Empty;

        // While there is not one tank left...
        while (!OneTankLeft())
        {
            // ... return on the next frame.
            yield return null;
        }
    }


    private IEnumerator RoundEnding()
    {
        // Stop tanks from moving.
        DisableTankControl ();

        // Clear the winner from the previous round.
        m_RoundWinner = null;

        // See if there is a winner now the round is over.
        m_RoundWinner = GetRoundWinner ();

        // If there is a winner, increment their score.
        if (m_RoundWinner != null)
            m_RoundWinner.m_Wins++;

        // Now the winner's score has been incremented, see if someone has one the game.
        m_GameWinner = GetGameWinner ();

        // Get a message based on the scores and whether or not there is a game winner and display it.
        string message = EndMessage ();
        m_MessageText.text = message;

        // Wait for the specified length of time until yielding control back to the game loop.
        yield return m_EndWait;    }


    private bool OneTankLeft()
    {
        int numTanksLeft = 0;

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                numTanksLeft++;
        }

        return numTanksLeft <= 1;
    }


    private TankManager GetRoundWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Instance.activeSelf)
                return m_Tanks[i];
        }

        return null;
    }


    private TankManager GetGameWinner()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            if (m_Tanks[i].m_Wins == m_NumRoundsToWin)
                return m_Tanks[i];
        }

        return null;
    }


    private string EndMessage()
    {
        string message = "DRAW!";

        if (m_RoundWinner != null)
            message = m_RoundWinner.m_ColoredPlayerText + " WINS THE ROUND!";

        message += "\n\n\n\n";

        for (int i = 0; i < m_Tanks.Length; i++)
        {
            message += m_Tanks[i].m_ColoredPlayerText + ": " + m_Tanks[i].m_Wins + " WINS\n";
        }

        if (m_GameWinner != null)
            message = m_GameWinner.m_ColoredPlayerText + " WINS THE GAME!";

        return message;
    }


    private void ResetAllTanks()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].Reset();
        }
    }


    private void EnableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].EnableControl();
        }
    }


    private void DisableTankControl()
    {
        for (int i = 0; i < m_Tanks.Length; i++)
        {
            m_Tanks[i].DisableControl();
        }
    }
}