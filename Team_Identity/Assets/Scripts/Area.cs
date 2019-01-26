using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Area : MonoBehaviour
{
    public string team;
    public int points;
    public bool active;
    public GameObject text;

    // Start is called before the first frame update
    void Start()
    {
        points = 10;
        text.GetComponent<Text>().text = points + " Points in Area " + team;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    

}
