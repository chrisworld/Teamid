using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    //sources
    public string team;
    public Sprite icon;
    public GameObject gameManager;

    // attribute
    public float speed;
    public float dashDuration;
    public float dashMultiplier;
    static readonly float dashCdStat = 2f;
    static readonly float stealCdStat = 2f;
    static readonly float depositCdStat = 2f;

    private int points;
    private Vector2 direction;

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
    public bool control = true;

    private float nextDash;

    float dashTimer;
    float dashEndtime;
    float stunTimer = 3f;
    float stunEndtime;
    float dodgeTimer = 1f;
    float dodgeEndtime;
    float nextBlink;
    float blinkIntervall = 0.5f;
    float stealTimer;
    float stealEndtime;
    float depositTimer;
    float depositEndtime;
    float massReductionStun = 4f;
    float shieldRotatingSpeed = 30f;
    int maxPointsHolding = 3;

    Vector2 dashDirection;


    // Start is called before the first frame update
    void Start()
    {
        shield = gameObject.transform.GetChild(0).gameObject;
        shield.GetComponent<SpriteRenderer>().enabled = false;

        nextBlink = Time.time;
        gameManager = GameObject.FindGameObjectWithTag("Manager");
        GetRandomIcon();
        rb2d = GetComponent<Rigidbody2D>();
        direction = new Vector2(0, 0);
    }

    private void FixedUpdate()
    {
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

                //dash attack
                if (Input.GetKeyDown("space"))
                    Dash();
                if (Input.GetButtonDown("Fire2"))
                {
                    Dodge();
                }

                if (isDashing)
                {
                    if (Time.time > dashEndtime)
                    {
                        isDashing = false;
                        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 4f, transform.localScale.z);
                    }

                }
                else
                {
                    //move
                    rb2d.AddForce(direction * speed);
                }
                if (dodging)
                {
                    shield.transform.Rotate(Vector3.forward, shieldRotatingSpeed * Time.deltaTime);
                    if (Time.time > dodgeEndtime)
                    {
                        dodging = false;
                        shield.GetComponent<SpriteRenderer>().enabled = false;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (locatedArea != null && stealing && !stunned)
        {
            if (Time.time > stealEndtime)
            {
                stealEndtime = Time.time + stealCdStat;
                Steal(locatedArea);
                
            }

        }
        if (locatedArea != null && depositing && !stunned)
        {
            if (Time.time > depositEndtime)
            {
                depositEndtime = Time.time + depositCdStat;
                Deposit(locatedArea);
                
            }
        }

    }


    private void Dash()
    {
        if (Time.time > dashTimer)
        {
            FindObjectOfType<SoundManager>().dash.Play();
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y / 4f, transform.localScale.z);
            dashTimer = Time.time + dashCdStat;
            dashEndtime = Time.time + dashDuration;
            isDashing = true;
            dashDirection = direction;
            rb2d.AddForce(dashDirection * speed * dashMultiplier);
        }


    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        locatedArea = collision.gameObject;

        if (!locatedArea.GetComponent<Area>().team.Equals(team))
        {
            stealing = true;
            stealEndtime = Time.time + stealCdStat;
        }else if (locatedArea.GetComponent<Area>().team.Equals(team))
        {
            depositing = true;
            depositEndtime = Time.time + depositCdStat;
        }
        
        Debug.Log("Entered " + locatedArea);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("exited area" + locatedArea);
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

                collision.gameObject.GetComponent<Player>().GetStunned();
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
                            Debug.Log("Pressed Up on D-pad");
                            movingUp = System.Convert.ToBoolean(data["data"]["pressed"].ToString());
                            break;

                        case "down":
                            Debug.Log("Pressed Down on D-pad");
                            movingDown = System.Convert.ToBoolean(data["data"]["pressed"].ToString());
                            break;

                        case "left":
                            Debug.Log("Pressed Left on D-pad");
                            movingLeft = System.Convert.ToBoolean(data["data"]["pressed"].ToString());
                            break;

                        case "right":
                            Debug.Log("Pressed Right on D-pad");
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

            case "dodge":
                if (System.Convert.ToBoolean(data["data"]["pressed"].ToString()))
                {
                    this.Dodge();
                }
                break;

            default:
                Debug.Log(data);
                break;
        }
    }

    void IconBackToList(Sprite sprite)
    {
        gameManager.GetComponent<MainManager>().spritelist.Add(sprite);
    }


    private void GetRandomIcon()
    {
        int rnd = UnityEngine.Random.Range(0, gameManager.GetComponent<MainManager>().spritelist.Count);
        gameObject.GetComponent<SpriteRenderer>().sprite = gameManager.GetComponent<MainManager>().spritelist[rnd];
        gameManager.GetComponent<MainManager>().spritelist.RemoveAt(rnd);
    }

    public void GetStunned()
    {
        //Beginn of stun
        FindObjectOfType<SoundManager>().stun.Play();
        stunned = true;
        stunEndtime = Time.time + stunTimer;
        gameObject.GetComponent<Rigidbody2D>().mass = gameObject.GetComponent<Rigidbody2D>().mass / massReductionStun;
    }

    private void Dodge()
    {
        if (!isDashing)
        {
            FindObjectOfType<SoundManager>().defend.Play();
            shield.GetComponent<SpriteRenderer>().enabled = true;
            dodging = true;
            dodgeEndtime = Time.time + dodgeTimer;
        }
    }

    private void Steal(GameObject areaObject)
    {
        if (points < maxPointsHolding)
        {
            Area area = areaObject.GetComponent<Area>();
            area.points--;
            area.text.GetComponent<Text>().text = area.points + " Points in Area " + area.team;
            this.points++;
            textPoints.GetComponent<Text>().text = points + " points in the backpack.";
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
            textPoints.GetComponent<Text>().text = points + " points in the backpack.";
        }
    }
    private void GetRandomTeam()
    {
        if (gameManager.GetComponent<MainManager>().teamACount == gameManager.GetComponent<MainManager>().teamBCount)
        {
            int rnd = UnityEngine.Random.Range(0, 2);
            team = gameManager.GetComponent<MainManager>().teamNames[rnd];
            if (rnd == 0)
            {
                gameManager.GetComponent<MainManager>().teamACount++;
            }
            else if (rnd == 1)
            {
                gameManager.GetComponent<MainManager>().teamBCount++;
            }
        }
        else
        if (gameManager.GetComponent<MainManager>().teamACount > gameManager.GetComponent<MainManager>().teamBCount)
        {
            team = gameManager.GetComponent<MainManager>().teamNames[1];
            gameManager.GetComponent<MainManager>().teamBCount++;
        }
        else
        if (gameManager.GetComponent<MainManager>().teamACount < gameManager.GetComponent<MainManager>().teamBCount)
        {
            team = gameManager.GetComponent<MainManager>().teamNames[0];
            gameManager.GetComponent<MainManager>().teamACount++;
        }

    }
}
