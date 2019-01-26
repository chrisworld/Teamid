using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;


public class MainManager : MonoBehaviour
{
    public string[] teamNames = { "Team Red", "Team Blue", "Team Yellow", "Team Green" };
    public int gametime = 0;
    public int goaltime;
    public GameObject timeText;
    public Sprite[] baseSprites;
    public List<Sprite> spritelist = new List<Sprite>();

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
        Debug.Log(spritelist.Count);
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
        //timeText.GetComponent<Text>().text = ""+gametime;
    }

    private void Endgame()
    {

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
