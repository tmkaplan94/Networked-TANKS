using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class PlayerItem : MonoBehaviour
{
    [SerializeField] private Text playerName;

    public void SetPlayerName(Player player)
    {
        playerName.text = player.NickName;
    }
}