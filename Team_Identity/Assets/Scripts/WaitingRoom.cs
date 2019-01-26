using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class WaitingRoom : MonoBehaviour
{
  public int joined_player;
  public int ready_player;
  public bool is_ready;
  public Spawner spawner;
  
  public Dictionary<int, Player> players = new Dictionary<int, Player> (); 
  //private List<int> connectedDevices;

  // Start is called before the first frame update
  void Start()
  {
    joined_player = 0;
    ready_player = 0;
    is_ready = false;
  }

  void OnReady(string code){
    //connectedDevices = AirConsole.instance.GetControllerDeviceIds();
    //foreach (int deviceID in connectedDevices) {
    //  AddNewPlayer (deviceID);
    //}
  }

  public void AddReadyPlayer(int player_id){
    if (players.ContainsKey(player_id)){
      return;
    }

    //Instantiate player prefab, store device id + player script in a dictionary
    //GameObject newPlayer = Instantiate (playerPrefab, transform.position, transform.rotation) as GameObject;
    // Spawn Player

    //players.Add(deviceID, newPlayer.GetComponent<Player_Platformer>());
    
    players.Add(player_id, spawner.SpawnPlayer());
    ready_player += 1;
    Debug.Log("Ready player with id: " + player_id + " ready players: " + ready_player);
    
    List<int> connectedDevices = AirConsole.instance.GetControllerDeviceIds();
    Debug.Log("Amount con Devices: " + connectedDevices.Count);
    
    // check device ids
    foreach (int deviceID in connectedDevices) {
      if (!players.ContainsKey(deviceID))
        return;
    }

    // all players are ready condition
    Debug.Log("All Players ready");
    is_ready = true;
  }


}
