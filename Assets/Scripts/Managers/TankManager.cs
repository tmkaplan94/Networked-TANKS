using System;
using Photon.Pun;
using UnityEngine;

[Serializable]
public class TankManager
{
    public Color m_PlayerColor;                              
    public Transform m_SpawnPoint;                           
    [HideInInspector] public int m_PlayerNumber;             
    [HideInInspector] public string m_ColoredPlayerText;     
    [HideInInspector] public GameObject m_Instance;          
    [HideInInspector] public int m_Wins;
    public PhotonView _View;

    private TankMovement m_Movement;       
    private TankShooting m_Shooting;
    private GameObject m_CanvasGameObject;

    public void Setup()
    {
        m_Movement = m_Instance.GetComponent<TankMovement>();
        m_Shooting = m_Instance.GetComponent<TankShooting>();
        m_CanvasGameObject = m_Instance.GetComponentInChildren<Canvas>().gameObject;

        if (PhotonNetwork.IsMasterClient)
        {
            m_Movement.m_PlayerNumber = 1;
            m_Shooting.m_PlayerNumber = 1;
        }
        else
        {
            m_Movement.m_PlayerNumber = 2;
            m_Shooting.m_PlayerNumber = 2;
        }


        m_ColoredPlayerText = "<color=#" + ColorUtility.ToHtmlStringRGB(m_PlayerColor) + ">PLAYER " + m_PlayerNumber + "</color>";

        MeshRenderer[] renderers = m_Instance.GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].material.color = m_PlayerColor;
        }
    }


    public void DisableControl()
    {
        if (!_View.IsMine) return;
        m_Movement.enabled = false;
        m_Shooting.enabled = false;

        m_CanvasGameObject.SetActive(false);
    }


    public void EnableControl()
    {
        if (!_View.IsMine) return;
        m_Movement.enabled = true;
        m_Shooting.enabled = true;

        m_CanvasGameObject.SetActive(true);
    }


    public void Reset()
    {
        if (!_View.IsMine) return;
        m_Instance.transform.position = m_SpawnPoint.position;
        m_Instance.transform.rotation = m_SpawnPoint.rotation;

        m_Instance.SetActive(false);
        m_Instance.SetActive(true);
    }
}
