using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string team;
    public Sprite icon;
    static public Sprite[] baseSprites;
    static private List<Sprite> spritelist = new List<Sprite>();
    private int points;
    public float speed;
    private Rigidbody2D rb2d;

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

    private void Steal()
    {

    }
    private void Deposit()
    {

    }
}
