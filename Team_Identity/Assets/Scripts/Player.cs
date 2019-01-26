using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    private string team;
    public Sprite icon;
    static public Sprite[] baseSprites;
    static private List<Sprite> spritelist = new List<Sprite>();
    private int points;
    private Vector2 direction;
    public float speed;
    private Rigidbody2D rb2d;
    public GameObject textPoints;
    private GameObject locatedArea;

    bool movingLeft;
    bool movingRight;
    bool movingUp;
    bool movingDown;
    bool isDashing;

    float dashCooldown;
    float dashTimer;
    public float dashDuration = 0.2f;
    public int dashMultiplier = 2;
    Vector2 dashDirection;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        direction = new Vector2(0, 0);
    }

    private void FixedUpdate()
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

        if (Input.GetKeyDown("space"))
            Dash();

        if (isDashing)
        {
            rb2d.AddForce(dashDirection * speed * dashMultiplier);

            dashTimer -= Time.fixedDeltaTime;

            if (dashTimer <= 0)
            {
                isDashing = false;
                dashCooldown = 10f;
            }
        }
        else
        {
            rb2d.AddForce(direction * speed);
        }

        dashCooldown -= Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Fire1"))
            if(locatedArea != null && !locatedArea.GetComponent<Area>().team.Equals(team))
        {
            Steal(locatedArea);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        locatedArea = collision.gameObject;
        Debug.Log("Entered " + locatedArea);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Debug.Log("Left " + locatedArea);
        locatedArea = null;
        
    }

    public void PlayerInput(JToken data)
    {
        switch(data["action"].ToString())
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

    static void IconBackToList(Sprite sprite)
    {
        spritelist.Add(sprite);
    }


    private void GetIcon(Sprite icon)
    {
        int rnd = UnityEngine.Random.Range(0, spritelist.Count);
        gameObject.GetComponent<SpriteRenderer>().sprite = spritelist[rnd];
        spritelist.RemoveAt(rnd);
    }

    private void Dash()
    {
        if (dashCooldown <= 0)
        {
            isDashing = true;
            dashTimer = dashDuration;
            dashDirection = direction;
        }
        

    }

    private void Move(Vector2 direction)
    {

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
