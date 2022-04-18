using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

[RequireComponent(typeof(InputField))]
public class PlayerNameInputField : MonoBehaviour
{
    #region Constants

    // store PlayerPref key to avoid typos
    private const string PlayerNamePrefKey = "PlayerName";

    #endregion


    #region MonoBehavior Callbacks
    
    private void Start ()
    {
        SetPlayerPrefPlayerName();
    }
    
    #endregion


    #region Public Methods
    
    // updates the PlayerPrefs player name for future sessions
    public void UpdatePlayerPrefPlayerName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Player Name is null or empty");
            return;
        }
        PhotonNetwork.NickName = value;
        PlayerPrefs.SetString(PlayerNamePrefKey, value);
    }

    // sets the input text to PlayerPref player name
    public void SetPlayerPrefPlayerName()
    {
        string defaultName = "";
        InputField inputField = GetComponent<InputField>();
        if (inputField != null)
        {
            if (PlayerPrefs.HasKey(PlayerNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(PlayerNamePrefKey);
                inputField.text = defaultName;
            }
        }
        
        PhotonNetwork.NickName =  defaultName;
    }
    
    // gets the input text player name
    public string GetPlayerPrefPlayerName()
    {
        InputField inputField = GetComponent<InputField>();
        return inputField.text;
    }

    #endregion
}