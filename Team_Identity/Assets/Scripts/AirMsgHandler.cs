using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirMsgHandler : MonoBehaviour
{
  public WaitingRoom waiting_room;
  public Spawner spawner;

  void Awake()
  {
    AirConsole.instance.onMessage += OnMessage;
  }

  void OnMessage(int fromDeviceID, JToken data){
    Debug.Log("message from " + fromDeviceID + ": " + data);
    //if (data["action"] != null && data["action"].ToString().Equals("ready") ){
    //  waiting_room.AddReadyPlayer(fromDeviceID);
    //}

    if (data["action"] != null && data["action"].ToString().Equals("spawn") ){
      spawner.SpawnPlayer();
    }
  }
}
