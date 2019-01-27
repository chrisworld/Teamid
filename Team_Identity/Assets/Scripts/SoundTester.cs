using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundTester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      if (Input.GetKeyDown(KeyCode.M)) FindObjectOfType<SoundManager>().stun.Play();
      if (Input.GetKeyDown(KeyCode.N)) FindObjectOfType<SoundManager>().dash.Play();
      if (Input.GetKeyDown(KeyCode.K)) FindObjectOfType<SoundManager>().defend.Play();
      if (Input.GetKeyDown(KeyCode.L)) FindObjectOfType<SoundManager>().StartBackgroundTheme();
    }
}
