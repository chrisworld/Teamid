using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class MainManager : MonoBehaviour
{

    public static readonly string[] teamNames = { "Team Left", "Team Right"};
    [SerializeField]
    public static int teamACount = 0;
    public static int teamBCount = 0;
    public GameObject teamAarea;
    public GameObject teamBarea;

    int gametime = 0;
    public int goaltime;
    int countdownValue = 60;
    public static int countdownTime;
    readonly int startPlayerCount = 2;
    public GameObject timeText;
    GameObject spawner;
    public Sprite[] baseSprites;
    public static List<Sprite> spritelist = new List<Sprite>();
    List<int> devicesList;
    public static string winningTeam;
    public static int winningTeamPoints;
    public static int losingTeamPoints;

    public GameObject countdown;
    public static bool started = false;
    public static bool endgame = false;
    public static bool endtriggered = false;
    public static bool spawnNPC = false;

    void Awake()
    {
        started = false;
        endgame = false;
        endtriggered = false;
        object[] loadedSprites = Resources.LoadAll("PlayerSprites",typeof(Sprite));
        baseSprites = new Sprite[loadedSprites.Length];
        for (int i = 0; i < loadedSprites.Length; i++)
        {
            baseSprites[i] = (Sprite)loadedSprites[i];
        }

        spritelist.Clear();
        for (int i = 0; i < baseSprites.Length; i++)
        {
            spritelist.Add(baseSprites[i]);
        }
        Debug.Log(spritelist.Count + "Sprites in IconList loaded");
    }
    // Start is called before the first frame update
    void Start()
    {
        teamAarea = GameObject.FindGameObjectWithTag("AreaTeam1");
        teamBarea = GameObject.FindGameObjectWithTag("AreaTeam2");
        spawner = GameObject.FindGameObjectWithTag("Spawner");
        countdownTime = countdownValue;
        gametime = goaltime;
        StartCoroutine("Timer");
        Time.timeScale = 1;

    }

    // Update is called once per frame
    void Update()
    {
        if(gametime == 0 && !endtriggered)
        {
            Endgame();
        }
        if(!started)
        {
            countdown.GetComponent<Text>().text = "Starting when at least " +startPlayerCount+ " Players are connected in " + countdownTime;
            
            if (countdownTime <= 0 && !started){
                FindDevices();
                Debug.Log(devicesList.Count);
                //if enough players in list start round
                if (devicesList.Count >= startPlayerCount)
                {

                    StartRound();
  
                }else
                {
                    //reset Countdown
                    countdownTime = countdownValue;
                }
            }
        }
        
        timeText.GetComponent<Text>().text = "Time: "+gametime;

        //start game early
        if (Input.GetKeyUp("space"))
        {
            FindDevices();
            StartRound();
        }
        //endgame early
        if (Input.GetKeyUp("escape"))
        {
            gametime = 0;
        }


    }
    public void FindDevices()
    {
        devicesList = this.GetComponent<AirManagerTest>().GetConnectedDevices();
    }
    public void StartRound()
    {
        started = true;
        FindObjectOfType<SoundManager>().StartBackgroundTheme();

        countdown.SetActive(false);
        Debug.Log(devicesList.Count);
        this.GetComponent<AirManagerTest>().SpawnPlayers(devicesList);

        //npcs when 4 players or less
        if (devicesList.Count < 5 && spawnNPC)
        {
            spawner.GetComponent<Spawner>().SpawnNPCs(2);
        }
    }

    private void Endgame()
    {
        Debug.Log("End triggered");
        endtriggered = true;
        FindObjectOfType<SoundManager>().StopBackgroundTheme();
        if (teamAarea.GetComponent<Area>().points > teamBarea.GetComponent<Area>().points)
        {
            winningTeam = "Team Left";
            winningTeamPoints = teamAarea.GetComponent<Area>().points;
            losingTeamPoints = teamBarea.GetComponent<Area>().points;
        }
        else if (teamAarea.GetComponent<Area>().points < teamBarea.GetComponent<Area>().points)
        {
            winningTeam = "Team Right";
            losingTeamPoints = teamAarea.GetComponent<Area>().points;
            winningTeamPoints = teamBarea.GetComponent<Area>().points;
        }
        else if (teamAarea.GetComponent<Area>().points == teamBarea.GetComponent<Area>().points)
        {
            winningTeam = "Both";
            losingTeamPoints = teamAarea.GetComponent<Area>().points;
            winningTeamPoints = teamBarea.GetComponent<Area>().points;
        }
        KillLoser();
        teamACount = 0;
        teamBCount = 0;
        StartCoroutine("SwitchToEndgame");
    }

    private void KillLoser()
    {
        foreach(GameObject ele in spawner.GetComponent<Spawner>().npc_list)
        {
            StartCoroutine("KillAnimation",ele);
        }
        foreach(GameObject ele in spawner.GetComponent<Spawner>().player_list)
        {
            if (!ele.GetComponent<Player>().team.Equals(winningTeam)){
                StartCoroutine("KillAnimation", ele);
            }
        }
        
    }

    IEnumerator KillAnimation(GameObject victim)
    {
        float animationTime = 2f;
        float timer = 0;
        float freq = 0.01f;
        timer += animationTime;
        victim.GetComponent<CircleCollider2D>().enabled = false;

        while (timer >= 0)
        {
            timer -= freq;
            yield return new WaitForSeconds(freq);
            Transform v_transform = victim.transform;
            SpriteRenderer v_renderer = victim.GetComponent<SpriteRenderer>();
            v_transform.localScale = new Vector3(v_transform.localScale.x * 1.05f, v_transform.localScale.y * 1.05f, v_transform.localScale.z);
            v_renderer.color = new Color(1f, 1f, 1f, v_renderer.color.a - 0.05f);
        }
        victim.gameObject.SetActive(false);

    }
    IEnumerator SwitchToEndgame()
    {
        yield return new WaitForSeconds(5);
        SceneManager.LoadScene("Endgame");
    }


    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            if (started && gametime >0)
            {
                gametime--;
            }
            if(countdownTime > 0)
            {
                countdownTime--;
            }
            
        }
    }


}
