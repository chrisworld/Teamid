using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class WaitingRoom : MonoBehaviour
{
  public int total_members;
  public Spawner spawner;

  [HideInInspector]
  public int ready_player;
  [HideInInspector]
  public bool is_ready;
  
  public Dictionary<int, Player> players = new Dictionary<int, Player> (); 
  //private List<int> connectedDevices;

  // Start is called before the first frame update
  void Start()
  {
    ready_player = 0;
    is_ready = false;
  }

  // Add Ready Player in Waiting Room
  public void AddReadyPlayer(int player_id){
    if (players.ContainsKey(player_id)){
      return;
    }

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
    int total_npcs = total_members - ready_player;
    Debug.Log("All Players ready: " + ready_player + " NPCs: " + total_npcs);
    is_ready = true;
    spawner.SpawnNPCs(total_npcs);

        // TODO: signal Game manager to start game, hand over the list of players
  }


}
