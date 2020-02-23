using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour {
    public Material blue;
    public Material id;
    private bool isClick = false;
    // Use this for initialization
    void Start () {    
    }
    // Update is called once per frame
    void Update () {   
    }
    void OnMouseDown()
    {
        if (!isClick) {
            this.gameObject.GetComponent<MeshRenderer> ().material = blue;
            isClick = true;
        } else {
            this.gameObject.GetComponent<MeshRenderer> ().material = id;
            isClick = false;
        }
    }
}