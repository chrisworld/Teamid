using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//FindObjectOfType<SoundManager>().Play("nameOfSound");

public class SoundManager : MonoBehaviour
{
  public AudioSource defend;
  public AudioSource dash;
  public AudioSource stun;
  public AudioSource background_theme;

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

  public void StartBackgroundTheme()
  {
    
    background_theme.Play();
  }

    public void StopBackgroundTheme()
    {
        background_theme.Stop();
    }
}
