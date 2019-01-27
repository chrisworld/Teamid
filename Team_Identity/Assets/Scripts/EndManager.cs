using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndManager : MonoBehaviour
{

    public GameObject Text;
    // Start is called before the first frame update
    void Start()
    {
        Text.GetComponent<Text>().text = MainManager.winningTeam + " won the game with " + MainManager.winningTeamPoints + " : " + MainManager.losingTeamPoints;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
