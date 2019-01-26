using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class Spawner : MonoBehaviour
{
  public Transform spawn_pos;
  public GameObject player_prefab;
  public float spawn_range = 20f; 

  private List<Player> player_list = new List<Player>();

  void Start()
  {

  }

  /*
  void OnMessage(int fromDeviceID, JToken data){
    Debug.Log("message from " + fromDeviceID + " data: " + data);
    if (data["action"] != null && data["action"].ToString().Equals("spawn") ){
      //Player player = new Player();
      player_list.Add(new Player());
      SpawnPlayer();
    }
  }
  */

  // Start is called before the first frame update
  public Player SpawnPlayer()
  {
    Vector3 spawnPosition = new Vector3 (spawn_pos.position.x + Random.Range(-spawn_range, spawn_range), 0.5f, spawn_pos.position.z + Random.Range(-spawn_range, spawn_range));
    Quaternion spawnRotation = Quaternion.Euler (0f, 0f, 0f);
    GameObject player = (GameObject)Instantiate (player_prefab, spawnPosition, spawnRotation);
    return player.GetComponent<Player>();
  }

  // Add Spawnable Player to game
  public void AddPlayerToGame(Player player)
  {
    player_list.Add(player);
    Debug.Log("Add player to next game: " + player);
  }
}
