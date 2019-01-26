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
    static float dashCdStat = 2f;

    private int points;
    private Vector2 direction;

    private Rigidbody2D rb2d;
    public GameObject textPoints;
    private GameObject locatedArea;

    bool movingLeft;
    bool movingRight;
    bool movingUp;
    bool movingDown;

    //states
    bool isDashing;
    bool stunned = false;
    bool blinkOn = false;
    bool dodging = false;
    public bool control = true;

    private float nextDash;

    float dashTimer;
    float dashEndtime;
    float stunTimer = 3f;
    float stunEndtime;
    float nextBlink;
    float blinkIntervall = 0.5f;
    float massReductionStun = 4f;

    Vector2 dashDirection;


    // Start is called before the first frame update
    void Start()
    {
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
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Fire1"))
            if (locatedArea != null && !locatedArea.GetComponent<Area>().team.Equals(team))
            {
                Steal(locatedArea);
            }
            else if (locatedArea != null && locatedArea.GetComponent<Area>().team.Equals(team))
            {
                Deposit(locatedArea);
            }

    }


    private void Dash()
    {
        if (Time.time > dashTimer)
        {

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
        Debug.Log("Entered " + locatedArea);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (isDashing)
        {
            if (collision.gameObject.tag == "Player")
            {
                collision.gameObject.GetComponent<Player>().GetStunned();
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Left " + locatedArea);
        locatedArea = null;

    }

    public void PlayerInput(JToken data)
    {
        switch (data["action"].ToString())
        {
            case "dpad":
                switch (data["dpad"]["directionchange"]["key"].ToString())
                {
                    case "up":
                        movingUp = System.Convert.ToBoolean(data["dpad"]["directionchange"]["pressed"]);
                        break;

                    case "down":
                        movingDown = System.Convert.ToBoolean(data["dpad"]["directionchange"]["pressed"]);
                        break;

                    case "left":
                        movingLeft = System.Convert.ToBoolean(data["dpad"]["directionchange"]["pressed"]);
                        break;

                    case "right":
                        movingRight = System.Convert.ToBoolean(data["dpad"]["directionchange"]["pressed"]);
                        break;

                    default:
                        Debug.Log(data);
                        break;
                }
                break;

            case "dash":
                this.Dash();
                break;

            case "dodge":
                this.Dodge();
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
        stunned = true;
        stunEndtime = Time.time + stunTimer;
        gameObject.GetComponent<Rigidbody2D>().mass = gameObject.GetComponent<Rigidbody2D>().mass / massReductionStun;
    }

    private void Dodge()
    {
        
    }

    private void Steal(GameObject areaObject)
    {
        Area area = areaObject.GetComponent<Area>();
        area.points--;
        area.text.GetComponent<Text>().text = area.points + " Points in Area " + area.team;
        this.points++;
        textPoints.GetComponent<Text>().text = points + " points in the backpack.";
    }
    private void Deposit(GameObject areaObject)
    {
        Area area = areaObject.GetComponent<Area>();
        area.points++;
        area.text.GetComponent<Text>().text = area.points + " Points in Area " + area.team;
        this.points--;
        textPoints.GetComponent<Text>().text = points + " points in the backpack.";
    }
}
