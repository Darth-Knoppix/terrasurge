using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UI_HUD : MonoBehaviour
{
    public Text score;
    public Text health;
    public Text multiplier;
	public int defaultTiemout = 40;

	private int lastScore;
	private int scoreCooldownTimer;

    private string values;
	private Main player;
	private ScoreController scoreboard;

    private GameObject gameHUD_Canvas;

    // Use this for initialization
    void Start()
    {
        //player = GetComponent<CharacterController>();
        //pscript = player.GetComponent<playerscript>();

        score = GameObject.Find("Score_Value").GetComponent<Text>();
        health = GameObject.Find("Health_Value").GetComponent<Text>();
        multiplier = GameObject.Find("ScoreMultiplier_Text").GetComponent<Text>();

		scoreboard = GameObject.Find("GameManager").GetComponent<ScoreController> ();
		player = GameObject.Find ("Ship").GetComponent<Main> ();

        gameHUD_Canvas = GameObject.Find("GameHUD_Canvas");
    }

	void highlightScore(){
		scoreCooldownTimer = defaultTiemout;
		score.color = Color.green;
	}

    // Update is called once per frame
    void Update()
    {
		if (scoreCooldownTimer <= 0) {
			score.color = Color.white;
		}
		if (lastScore != scoreboard.getScore()) {
			highlightScore ();
		}

		lastScore = scoreboard.getScore();
		score.text = "" + scoreboard.getScore();
        health.text = "" + player.health + "%";
		String multi = player.scoreMultiplier.ToString ("n2"); // 2dp Number
		multiplier.text = "Multiplier: \n" + multi;

		if (scoreCooldownTimer > 0) {
			scoreCooldownTimer--;
		}
    }
}
