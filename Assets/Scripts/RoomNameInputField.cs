using UnityEngine;
using UnityEngine.UI;

public class RoomNameInputField : MonoBehaviour
{
    #region Constants

    // store PlayerPref key to avoid typos
    private const string RoomNamePrefKey = "RoomName";

    #endregion


    #region MonoBehavior Callbacks

    private void Start()
    {
        SetPlayerPrefRoomName();
    }

    #endregion


    #region Public Methods

    // updates the PlayerPrefs room name for future sessions
    public void UpdatePlayerPrefRoomName(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            Debug.LogError("Room Name is null or empty");
            return;
        }
        PlayerPrefs.SetString(RoomNamePrefKey, value);
    }

    // sets the input text to PlayerPref room name
    public void SetPlayerPrefRoomName()
    {
        string defaultName = "";
        InputField inputField = GetComponent<InputField>();
        if (inputField != null)
        {
            if (PlayerPrefs.HasKey(RoomNamePrefKey))
            {
                defaultName = PlayerPrefs.GetString(RoomNamePrefKey);
                inputField.text = defaultName;
            }
        }
    }
    
    // gets the input text room name
    public string GetPlayerPrefRoomName()
    {
        InputField inputField = GetComponent<InputField>();
        return inputField.text;
    }
    
    #endregion
}