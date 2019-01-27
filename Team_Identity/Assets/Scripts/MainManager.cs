using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainManager : MonoBehaviour
{
    public string[] teamNames = { "Team A", "Team B"};
    public int teamACount = 0;
    public int teamBCount = 0;
    public GameObject teamAarea;
    public GameObject teamBarea;
    public int gametime = 0;
    public int goaltime = 10;
    public GameObject timeText;
    public Sprite[] baseSprites;
    public List<Sprite> spritelist = new List<Sprite>();
    public static string winningTeam;
    public static int winningTeamPoints;
    public static int losingTeamPoints;

    void Awake()
    {
        object[] loadedSprites = Resources.LoadAll("PlayerSprites",typeof(Sprite));
        baseSprites = new Sprite[loadedSprites.Length];
        for (int i = 0; i < loadedSprites.Length; i++)
        {
            baseSprites[i] = (Sprite)loadedSprites[i];
        }

        for (int i = 0; i < baseSprites.Length; i++)
        {
            spritelist.Add(baseSprites[i]);
        }
        Debug.Log(spritelist.Count + "Sprites in IconList loaded");
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Timer");
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        if(gametime >= goaltime)
        {
            Endgame();
        }
        timeText.GetComponent<Text>().text = "Time: "+gametime;
    }

    private void Endgame()
    {
        Debug.Log("End triggered");
        if(teamAarea.GetComponent<Area>().points > teamBarea.GetComponent<Area>().points)
        {
            winningTeam = "Team A";
            winningTeamPoints = teamAarea.GetComponent<Area>().points;
            losingTeamPoints = teamBarea.GetComponent<Area>().points;
        }
        else if (teamAarea.GetComponent<Area>().points < teamBarea.GetComponent<Area>().points)
        {
            winningTeam = "Team B";
            losingTeamPoints = teamAarea.GetComponent<Area>().points;
            winningTeamPoints = teamBarea.GetComponent<Area>().points;
        }
        else if (teamAarea.GetComponent<Area>().points == teamBarea.GetComponent<Area>().points)
        {
            winningTeam = "Both";
            losingTeamPoints = teamAarea.GetComponent<Area>().points;
            winningTeamPoints = teamBarea.GetComponent<Area>().points;
        }

        SceneManager.LoadScene("Endgame");
    }







    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            gametime++;
        }
    }


}
