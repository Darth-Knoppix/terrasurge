using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UI_HUD : MonoBehaviour
{
    public GameObject player;
    public Main pscript;
	public PlayerControllerPredefined pcp;

    public Text score;
    public Text lives;
    public Text multiplier;

    private string values;

    private GameObject gameHUD_Canvas;

    // Use this for initialization
    void Start()
    {
        //player = GetComponent<CharacterController>();
        //pscript = player.GetComponent<playerscript>();

        score = GameObject.Find("Score_Text").GetComponent<Text>();
        lives = GameObject.Find("Lives_Text").GetComponent<Text>();
        multiplier = GameObject.Find("ScoreMultiplier_Text").GetComponent<Text>();

        pscript = GameObject.Find("Ship").GetComponent<Main>();
		if (pscript == null) {
			pcp = GameObject.Find("Ship").GetComponent<PlayerControllerPredefined>();
		}

        gameHUD_Canvas = GameObject.Find("GameHUD_Canvas");
    }

    // Update is called once per frame
    void Update()
    {
		if (pscript == null) {
			score.text = "Score: \n" + pcp.score;
			lives.text = "Health: \n" + pcp.lives;
			String multi = pcp.scoremx.ToString("n2"); // 2dp Number
			multiplier.text = "Multiplier: \n" + multi;
		} else {
			score.text = "Score: \n" + pscript.score;
			lives.text = "Health: \n" + pscript.lives;
			String multi = pscript.scoremx.ToString ("n2"); // 2dp Number
			multiplier.text = "Multiplier: \n" + multi;
		}
    }
}
