using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchAndSound : MonoBehaviour {
    public AudioClip loadSound;
    public AudioSource source;
    public float lowPitchRange = .75F;
    public float highPitchRange = 1.5F;
    [Range(0,1)]
    public float volume = 0.8f;
    public string trigger;
    // Use this for initialization
    void Start () {    
    }
    // Update is called once per frame
    void Update () {   
    }
    void OnMouseDown()
    {
        source.pitch = Random.Range (lowPitchRange,highPitchRange);
        source.PlayOneShot(loadSound, volume);
        if (trigger.Length != 0){
            this.gameObject.GetComponent<Animator>().SetTrigger(trigger);
        }
    }
    
}