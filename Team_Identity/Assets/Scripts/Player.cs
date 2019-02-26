using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NDream.AirConsole;

public class Player : MonoBehaviour
{
    //sources
    public string team = "Team Start";

    public Sprite icon;

    public GameObject gameManager;
    public GameObject player_prefab;
    Camera camera;
    Vector3 righttopForCamera;

    // attribute
    public float speed;
    static readonly float dashDuration = 0.5f;
    public float dashMultiplier;
    static readonly float dodgeCdStat = 3f;
    static readonly float dodgeDurationStat = 1f;
    static readonly float dashCdStat = 2f;
    static readonly float stealCdStat = 2f;
    static readonly float depositCdStat = 2f;

    private int points;
    private Vector2 direction;
    Vector2 dashDirection;
    private float normalRadiusDash;

    private Rigidbody2D rb2d;
    public GameObject textPoints;
    private GameObject locatedArea;
    private GameObject shield;

    bool movingLeft;
    bool movingRight;
    bool movingUp;
    bool movingDown;

    //states
    bool isDashing;
    bool stunned = false;
    bool blinkOn = false;
    bool dodging = false;
    bool stealing = false;
    bool depositing = false;
    // if played by player or npc
    public bool control = true;

    private float nextDash;

    float dashTimer = 0;
    float dashEndtime;
    float stunTimer = 3f;
    float stunEndtime;
    float dodgeTimer = 0;
    float dodgeEndtime;
    float nextBlink;
    float blinkIntervall = 0.3f;
    float stealTimer;
    float stealEndtime;
    float depositTimer;
    float depositEndtime;
    float massReductionStun = 6f;
    float shieldRotatingSpeed = 30f;
    int maxPointsHolding = 3;
    int deviceID;

    //Npc values
    Vector2 destination;
    float destinationTimer;
    float destinationEndtime;
    public static float maxDestinationCdStat = 5f;
    public static float minDestinationCdStat = 2f;


    
    // Start is called before the first frame update
    void Start()
    {
        gameManager = GameObject.FindGameObjectWithTag("Manager");
        if (control)
        {
            GetRandomIcon();
            GetRandomTeam();
            SendTeamMessageToController(deviceID,team);
        }

        normalRadiusDash = transform.localScale.y;
        
        shield = gameObject.transform.GetChild(0).gameObject;
        shield.GetComponent<Renderer>().enabled = false;
        nextBlink = Time.time;

        rb2d = GetComponent<Rigidbody2D>();
        direction = new Vector2(0, 0);
        if (!control)
        {
            camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();
            righttopForCamera = camera.ViewportToWorldPoint(new Vector3(0.8f, 0.8f, 0));
            GetRandomLocation();
        }
    }

    private void FixedUpdate()
    {
        //Stunstate
        if (stunned)
        {

            if (Time.time > nextBlink)
            {
                nextBlink = Time.time + blinkIntervall;
                if (blinkOn)
                {
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0.2f);
                }
                else
                {
                    gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
                }
                blinkOn = !blinkOn;
            }
            //End of stun
            if (Time.time > stunEndtime)
            {
                stunned = false;
                gameObject.GetComponent<Rigidbody2D>().mass = gameObject.GetComponent<Rigidbody2D>().mass * massReductionStun;
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            }

        }
        //Control
        else
        {
            if (control)
            {
                direction = Vector2.zero;

                if (movingUp)
                    direction += Vector2.up;
                if (movingDown)
                    direction += Vector2.down;
                if (movingLeft)
                    direction += Vector2.left;
                if (movingRight)
                    direction += Vector2.right;

                if (Input.GetKey("up"))
                    direction += Vector2.up;
                if (Input.GetKey("down"))
                    direction += Vector2.down;
                if (Input.GetKey("left"))
                    direction += Vector2.left;
                if (Input.GetKey("right"))
                    direction += Vector2.right;

                if (direction != Vector2.zero)
                {
                    float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                    transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
                }

                //for testing dash and dodge
                if (Input.GetKeyDown("a"))
                    Dash();
                if (Input.GetKeyDown("s"))
                {
                    Defend();
                }


                //animationlogic
                if (isDashing)
                {

                    if (Time.time >= dashEndtime)
                    {
                        transform.localScale = new Vector3(transform.localScale.x, normalRadiusDash, transform.localScale.z);
                        isDashing = false;
                    }

                }
                else
                {
                    //move
                    rb2d.velocity = direction * speed;
                }
                if (dodging)
                {
                    shield.transform.Rotate(Vector3.forward, shieldRotatingSpeed * Time.deltaTime);
                    if (Time.time >= dodgeEndtime)
                    {
                        dodging = false;
                        shield.GetComponent<Renderer>().enabled = false;
                    }
                }
            }
            else if (MainManager.started)
            {
                //Npc control
                transform.position = Vector2.MoveTowards(transform.position, destination, speed * Time.deltaTime);

                float angle = Mathf.Atan2(destination.y - transform.position.y, destination.x - transform.position.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));

                if (Time.time > destinationEndtime)
                {
                    destinationEndtime = Time.time + Random.Range(minDestinationCdStat, maxDestinationCdStat);
                    GetRandomLocation();

                }

            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (locatedArea != null && stealing && !stunned && control)
        {
            if (Time.time > stealEndtime)
            {
                stealEndtime = Time.time + stealCdStat;
                Steal(locatedArea);

            }

        }
        if (locatedArea != null && depositing && !stunned && control)
        {
            if (Time.time > depositEndtime)
            {
                Debug.Log("depositing");
                depositEndtime = Time.time + depositCdStat;
                Deposit(locatedArea);

            }
        }

    }

    
    private void Dash()
    {
        if (Time.time > dashTimer && !dodging && !stunned)
        {
            FindObjectOfType<SoundManager>().dash.Play();
            transform.localScale = new Vector3(transform.localScale.x, normalRadiusDash / 2f, transform.localScale.z);
            StartCoroutine("DashAnimation");

            dashTimer = Time.time + dashCdStat;
            dashEndtime = Time.time + dashDuration;
            isDashing = true;
            dashDirection = direction;
            rb2d.velocity = dashDirection * speed * dashMultiplier;
        }


    }
    IEnumerator DashAnimation()
    {
        while (true)
        {
            while (transform.localScale.y < normalRadiusDash)
            {

                transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 1.05f, transform.localScale.z);
                yield return new WaitForSeconds(0.01f);
            }

            if (transform.localScale.y >= normalRadiusDash)
            {

                yield break;
            }
        }
    }
    private void Defend()
    {
        
        if (!isDashing && !stunned && Time.time >= dodgeTimer)
        {
            FindObjectOfType<SoundManager>().defend.Play();
            shield.GetComponent<Renderer>().enabled = true;
            dodging = true;
            dodgeTimer = Time.time + dodgeCdStat;
            dodgeEndtime = Time.time + dodgeDurationStat;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        locatedArea = collision.gameObject;

        if (!locatedArea.GetComponent<Area>().team.Equals(team))
        {
            stealing = true;
            stealEndtime = Time.time + stealCdStat;
        }
        else if (locatedArea.GetComponent<Area>().team.Equals(team))
        {
            //Debug.Log("depositing");
            depositing = true;
            depositEndtime = Time.time + depositCdStat;
        }

        Debug.Log("Entered " + locatedArea);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("exited " + locatedArea);
        locatedArea = null;
        stealing = false;
        depositing = false;

    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (collision.gameObject.GetComponent<Player>().dodging != true)
                {
                    //stun the other
                    collision.gameObject.GetComponent<Player>().GetStunned();
                }
                else
                {
                    //become stunned yourself
                    GetStunned();
                }
            }
        }
    }
    public void PlayerInput(JToken data)
    {

        switch (data["element"].ToString())

        {
            case "dpad":
                if (data["data"]["key"] != null)
                {
                    switch (data["data"]["key"].ToString())
                    {
                        case "up":
                            //Debug.Log("Pressed Up on D-pad");
                            movingUp = System.Convert.ToBoolean(data["data"]["pressed"].ToString());
                            break;

                        case "down":
                            //Debug.Log("Pressed Down on D-pad");
                            movingDown = System.Convert.ToBoolean(data["data"]["pressed"].ToString());
                            break;

                        case "left":
                            // Debug.Log("Pressed Left on D-pad");
                            movingLeft = System.Convert.ToBoolean(data["data"]["pressed"].ToString());
                            break;

                        case "right":
                            //Debug.Log("Pressed Right on D-pad");
                            movingRight = System.Convert.ToBoolean(data["data"]["pressed"].ToString());
                            break;

                        default:
                            Debug.Log(data);
                            break;
                    }
                }

                break;

            case "dash":
                if (System.Convert.ToBoolean(data["data"]["pressed"].ToString()))
                {
                    this.Dash();
                }
                break;

            case "defend":
                if (System.Convert.ToBoolean(data["data"]["pressed"].ToString()))
                {
                    Debug.Log("defend");
                    this.Defend();
                }
                break;

            default:
                Debug.Log(data);
                break;
        }
    }
    void IconBackToList(Sprite sprite)
    {
        MainManager.spritelist.Add(sprite);
    }
    private void GetRandomIcon()
    {
        int rnd = Random.Range(0, MainManager.spritelist.Count);
        gameObject.GetComponent<SpriteRenderer>().sprite = MainManager.spritelist[rnd];
        MainManager.spritelist.RemoveAt(rnd);
    }
    public void GetStunned()
    {
        //Beginn of stun
        FindObjectOfType<SoundManager>().stun.Play();
        stunned = true;
        stunEndtime = Time.time + stunTimer;
        gameObject.GetComponent<Rigidbody2D>().mass = gameObject.GetComponent<Rigidbody2D>().mass / massReductionStun;
    }
    private void Steal(GameObject areaObject)
    {
        if (points < maxPointsHolding)
        {
            Area area = areaObject.GetComponent<Area>();
            area.points--;
            area.text.GetComponent<Text>().text = area.points + " Points in Area " + area.team;
            this.points++;
            SendPointMessageToController(this.deviceID, points);
        }
    }
    private void Deposit(GameObject areaObject)
    {
        if (points > 0)
        {
            Area area = areaObject.GetComponent<Area>();
            area.points++;
            area.text.GetComponent<Text>().text = area.points + " Points in Area " + area.team;
            this.points--;
            SendPointMessageToController(this.deviceID, points);
        }
    }
    public void SetDeviceID(int deviceID)
    {
        this.deviceID = deviceID;
    }

    public void GetRandomTeam()
    {
        if (MainManager.teamACount == MainManager.teamBCount)
        {
            int rnd = UnityEngine.Random.Range(0, 2);
            team = MainManager.teamNames[rnd];
            if (rnd == 0)
            {
                MainManager.teamACount++;
            }
            else if (rnd == 1)
            {
                MainManager.teamBCount++;
            }
        }
        else
        if (MainManager.teamACount > MainManager.teamBCount)
        {
            team = MainManager.teamNames[1];
            MainManager.teamBCount++;
        }
        else
        if (MainManager.teamACount < MainManager.teamBCount)
        {
            team = MainManager.teamNames[0];
            MainManager.teamACount++;
        }

    }

    void GetRandomLocation()
    {
        destination = new Vector2(
            Random.Range(-righttopForCamera.x, righttopForCamera.x),
            Random.Range(-righttopForCamera.y, righttopForCamera.y)
            );

    }

    public void SendTeamMessageToController(int ControllerID, string team)
    {
        AirConsole.instance.Message(ControllerID, team);
    }

    public void SendPointMessageToController(int ControllerID,int points)
    {
        Debug.Log("Send message: " + points + " to DeviceID: " + ControllerID);
        AirConsole.instance.Message(ControllerID, points);
    }


}
