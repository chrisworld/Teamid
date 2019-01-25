using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private string team;
    public Sprite icon;
    public Sprite[] baseSprites;
    private List<Sprite> spritelist = new List<Sprite>();
    private int points;
    public float speed;


    // Start is called before the first frame update
    void Start()
    {

        gameObject.GetComponent<SpriteRenderer>().sprite = spritelist.
    }

    // Update is called once per frame
    void Update()
    {
        
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
