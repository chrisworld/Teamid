using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FindObjectOfType<SoundManager>().Play("nameOfSound");

public class SoundManager : MonoBehaviour
{
  public AudioSource defend;
  public AudioSource dash;
  public AudioSource stun;

  // Singelton instance
  public static SoundManager instance;

    // Init Sounds
  void Awake () {
    // keep the same SoundManager through Scenes
    if (instance == null){
      instance = this;
    }
    else{
      Destroy(gameObject);
      return;
    }
    DontDestroyOnLoad(gameObject);
  }

  // Start is called before the first frame update
  void Start()
  {
      
  }

  // Update is called once per frame
  void Update()
  {
    // debug Sound
    if (Input.GetKeyDown(KeyCode.M)) defend.Play();
    if (Input.GetKeyDown(KeyCode.N)) dash.Play();
    if (Input.GetKeyDown(KeyCode.K)) stun.Play();

  }
}
