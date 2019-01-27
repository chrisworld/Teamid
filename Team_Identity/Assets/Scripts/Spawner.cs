using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class Spawner : MonoBehaviour
{
  
  List<Transform> spawn_positions;
  public GameObject player_prefab;
  public GameObject npc_prefab;
  public float spawn_range = 20f;

  private int spawn_index;
  private List<Player> player_list = new List<Player>();
  private int[] shuffled_index;

    // Start is called before the first frame update
    void Start()
    {
        spawn_positions = new List<Transform>();
        PopulateSpawnList();
        shuffled_index = ShuffledVector(spawn_positions.Count);
        spawn_index = 0;
    }

    private void PopulateSpawnList()
    {
        GameObject[] spawners = GameObject.FindGameObjectsWithTag("SpawnPosition");
        foreach (var obj in spawners)
        {
            spawn_positions.Add(obj.transform);
        }
    }
  
  public Player SpawnPlayer()
  {
    Transform spawn_pos = spawn_positions[shuffled_index[spawn_index]];
    spawn_index += 1;
    Vector2 spawnPosition = new Vector2 (spawn_pos.position.x, spawn_pos.position.y);
    Quaternion spawnRotation = Quaternion.Euler (0f, 0f, 0f);
    GameObject player = (GameObject)Instantiate (player_prefab, spawnPosition, spawnRotation);
    return player.GetComponent<Player>();
  }

  // Spawn NPC
  public void SpawnNPCs(int n_npc)
  {
    int npc_index = 0;
    for (int index = 0; index < n_npc; index += 1) 
    {
      Transform spawn_pos = spawn_positions[shuffled_index[spawn_index]];
      spawn_index += 1;
      Vector2 spawnPosition = new Vector2 (spawn_pos.position.x, spawn_pos.position.y);
      Quaternion spawnRotation = Quaternion.Euler (0f, 0f, 0f);
      npc_index += 1;
      GameObject npcs = (GameObject)Instantiate (npc_prefab, spawnPosition, spawnRotation);
    }
  }

  // Add Spawnable Player to game
  public void AddPlayerToGame(Player player)
  {
    player_list.Add(player);
    Debug.Log("Add player to next game: " + player);
  }

  // create shuffled vector for spawning
  
  public int[] ShuffledVector(int vec_len) {

    // create vector
    int[] sh_vec = new int[vec_len];
    for (int i = 0; i < vec_len; i++){
      sh_vec[i] = i;
    } 

    // shuffle
    for (int i = 0; i < vec_len; i++){
      int rnd = Random.Range(0, vec_len);
      int temp = sh_vec[rnd];
      sh_vec[rnd] = sh_vec[i];
      sh_vec[i] = temp;
    }

    return sh_vec;
  }
}
