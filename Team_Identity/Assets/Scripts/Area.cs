using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Area : MonoBehaviour
{
    private string team;
    public int points;
    public bool active;
    public GameObject text;

    // Start is called before the first frame update
    void Start()
    {
        points = 10;
        //hardcoded team for now
        team = "Team Blue";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

}
