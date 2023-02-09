using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Realtime;

public class RoomButton : MonoBehaviour
{
    public TMP_Text RoomButtonText;
    private RoomInfo info;

    public void setButtonDetails(RoomInfo roomInfo)
    {
        info = roomInfo;
        RoomButtonText.text = info.Name;
    }

    public void OpenRoom()
    {
        Launcher.instance.JoinRoomMethod(info);
    }
}
