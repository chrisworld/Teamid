using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class EndManager : MonoBehaviour
{
    public static int restartCd = 10;
    float restartTimer;
    public GameObject TextTimer;
    public GameObject Text;
    readonly int countDownTimeAfterRestart = 5;
    public GameObject PlayerPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Timer");
        Time.timeScale = 1;
        restartTimer = restartCd;
        Text.GetComponent<Text>().text = MainManager.winningTeam + " won the game with " + MainManager.winningTeamPoints + " : " + MainManager.losingTeamPoints;
        
    }

    // Update is called once per frame
    void Update()
    {
        TextTimer.GetComponent<Text>().text = "Restart in " + restartTimer;
        if(restartTimer<=0)
        {
            RestartGame();
        }
    }

    public void RestartGame()
    {
        MainManager.started = false;
        MainManager.countdownTime = countDownTimeAfterRestart;

        SceneManager.LoadScene(0);
    }
    IEnumerator Timer()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            restartTimer--;
        }
    }
}
