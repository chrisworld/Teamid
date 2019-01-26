using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public string team;
    public Sprite icon;
    static public Sprite[] baseSprites;
    static private List<Sprite> spritelist = new List<Sprite>();
    private int points;
    public float speed;
    private Rigidbody2D rb2d;
    public GameObject textPoints;
    private GameObject locatedArea;

    // Start is called before the first frame update
    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();


    }

    private void FixedUpdate()
    {
        float moveHorizontal = Input.GetAxisRaw("Horizontal");

        //Store the current vertical input in the float moveVertical.
        float moveVertical = Input.GetAxisRaw("Vertical");

        //Use the two store floats to create a new Vector2 variable movement.
        Vector2 movement = new Vector2(moveHorizontal, moveVertical);

        //Call the AddForce function of our Rigidbody2D rb2d supplying movement multiplied by speed to move our player.
        rb2d.AddForce(movement * speed);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonUp("Fire1"))
            if(locatedArea != null && !locatedArea.GetComponent<Area>().team.Equals(team))
        {
            Steal(locatedArea);
        }
        else if(locatedArea != null && locatedArea.GetComponent<Area>().team.Equals(team))
            {
                Deposit(locatedArea);
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

    static void IconBackToList(Sprite sprite)
    {
        spritelist.Add(sprite);
    }


    private void GetIcon(Sprite icon)
    {
        int rnd = Random.Range(0, spritelist.Count);
        gameObject.GetComponent<SpriteRenderer>().sprite = spritelist[rnd];
        spritelist.RemoveAt(rnd);
    }

    private void Dash()
    {

    }

    private void Move()
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
