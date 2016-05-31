using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UI_HUD : MonoBehaviour
{
    public Text score;
    public Text health;
    public Text healthLabel;

    // not used for now
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
        healthLabel = GameObject.Find("Health_Label").GetComponent<Text>();
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
        
		String multi = player.scoreMultiplier.ToString ("n2"); // 2dp Number
		multiplier.text = "Multiplier: \n" + multi;

        updateHealthShields();

		if (scoreCooldownTimer > 0) {
			scoreCooldownTimer--;
		}
    }

    private void updateHealthShields()
    {
        if (player.health <= 25)
        {
            healthLabel.text = "Health";
            health.text = "" + player.health + "%";
            health.color = Color.red;
        }
        else if (player.health <= 50)
        {
            healthLabel.text = "Health";
            health.text = "" + player.health + "%";
            health.color = new Color(255,128,0);
        }
        else if (player.health <= 75)
        {
            healthLabel.text = "Health";
            health.text = "" + player.health + "%";
            health.color = Color.yellow;
        }
        else if (player.health <= 100)
        {
            healthLabel.text = "Health";
            health.text = "" + player.health + "%";
            health.color = Color.green;
        }
        if (player.shields > 0)
        {
            healthLabel.text = "Shields";
            health.text = "" + player.shields + "%";
            health.color = new Color(0, 128, 255);
        }
    }
}


