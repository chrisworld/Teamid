using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class AirManagerTest : MonoBehaviour
{
    public AirConsole airconsole;
    public Spawner spawner;
    public GameObject playerPrefab;

    public static Dictionary<int, Player> players = new Dictionary<int, Player>();

    void Awake()
    {
        airconsole = GameObject.FindGameObjectWithTag("Airconsole").GetComponent<AirConsole>();
        AirConsole.instance.onMessage += OnMessage;
        //AirConsole.instance.onReady += OnReady;
        //AirConsole.instance.onConnect += OnConnect;
    }

    void OnReady(string code)
    {
        //Since people might be coming to the game from the AirConsole store once the game is live, 
        //I have to check for already connected devices here and cannot rely only on the OnConnect event 
        List<int> connectedDevices = AirConsole.instance.GetControllerDeviceIds();
        foreach (int deviceID in connectedDevices)
        {
            AddNewPlayer(deviceID);
        }
    }

    void OnConnect(int device)
    {
        AddNewPlayer(device);
    }

    public List<int> GetConnectedDevices()
    {
        List<int> connectedDevices = airconsole.GetControllerDeviceIds();

        return connectedDevices;
    }

    public void SpawnPlayers(List<int> deviceIDs)
    {
        if (deviceIDs != null)
        {
            foreach (var id in deviceIDs)
            {
                
                AddNewPlayer(id);
            }
            foreach(var id in spawner.player_list)
            {

            }
        }
    }

    private void AddNewPlayer(int deviceID)
    {

        if (players.ContainsKey(deviceID))
        {
            Debug.Log("i return");
            return;
        }

        //Instantiate player prefab, store device id + player script in a dictionary
        //GameObject newPlayer = Instantiate(playerPrefab, transform.position, transform.rotation) as GameObject;
        Player newPlayer = spawner.SpawnPlayer();
        players.Add(deviceID, newPlayer);
        SendTeamMessageToController(deviceID,"huh");
        Debug.Log("Send message: " + newPlayer.team + " to DeviceID: " + deviceID);
    }
    public void SendTeamMessageToController(int ControllerID,string team)
    {
        AirConsole.instance.Message(ControllerID, team);
    }


    void OnMessage(int from, JToken data)
    {
       Debug.Log("message: " + data);

        //When I get a message, I check if it's from any of the devices stored in my device Id dictionary
        if (players.ContainsKey(from) && data["element"] != null)
        {
            //I forward the command to the relevant player script, assigned by device ID
            players[from].PlayerInput(data);
        }
    }

    void OnDestroy()
    {
        if (AirConsole.instance != null)
        {
            AirConsole.instance.onMessage -= OnMessage;
            AirConsole.instance.onReady -= OnReady;
            AirConsole.instance.onConnect -= OnConnect;
        }
    }
}
