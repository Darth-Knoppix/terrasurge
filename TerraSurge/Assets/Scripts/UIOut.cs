using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UIOut : MonoBehaviour
{
    public GameObject player;
    public Main pscript;
    public Text output;
    string values;
    // Use this for initialization
    void Start()
    {
        //player = GetComponent<CharacterController>();
        //pscript = player.GetComponent<playerscript>();
        pscript = GameObject.Find("Ship").GetComponent<Main>();
    }

    // Update is called once per frame
    void Update()
    {
        values = "Score: " + pscript.score + "\r\nLives: " + pscript.lives +"\r\nSmx"+pscript.scoremx;
        GetComponent<Text>().text = values;
    }
}
