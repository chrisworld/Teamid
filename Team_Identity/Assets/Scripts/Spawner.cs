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

  void Awake()
  {
    AirConsole.instance.onMessage += OnMessage;
  }

  void OnMessage(int fromDeviceID, JToken data){
    Debug.Log("message from " + fromDeviceID + " data: " + data);
    if (data["action"] != null && data["action"].ToString().Equals("spawn") ){
      SpawnPlayer();
    }
  }

  // Start is called before the first frame update
  void SpawnPlayer()
  {
    Vector3 spawnPosition = new Vector3 (spawn_pos.position.x + Random.Range(-spawn_range, spawn_range), 0.5f, spawn_pos.position.z + Random.Range(-spawn_range, spawn_range));
    Quaternion spawnRotation = Quaternion.Euler (0f, 0f, 0f);
    GameObject player = (GameObject)Instantiate (player_prefab, spawnPosition, spawnRotation);
  }

  // Update is called once per frame
  void Update()
  {
      
  }
}
