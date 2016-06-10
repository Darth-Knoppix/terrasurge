using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class UI_HUD : MonoBehaviour
{
    public Text score;
    public Text multiplier;

    public Text health;
    public Text healthLabel;

    public Text shields;
    public Text shieldsLabel;

	public int defaultTiemout = 40;

	private int lastScore;
	private int scoreCooldownTimer;

    private string values;
	private Main player;
	private ScoreController scoreboard;

    private GameObject gameHUD_Canvas;

    private GameObject playerShield;

    // Use this for initialization
    void Start()
    {
        //player = GetComponent<CharacterController>();
        //pscript = player.GetComponent<playerscript>();

        score = GameObject.Find("Score_Value").GetComponent<Text>();
        multiplier = GameObject.Find("ScoreMultiplier_Text").GetComponent<Text>();

        health = GameObject.Find("Health_Value").GetComponent<Text>();
        healthLabel = GameObject.Find("Health_Label").GetComponent<Text>();

        shields = GameObject.Find("Shields_Value").GetComponent<Text>();
        shieldsLabel = GameObject.Find("Shields_Label").GetComponent<Text>();

        scoreboard = GameObject.Find("GameManager").GetComponent<ScoreController> ();
		player = GameObject.Find ("Ship").GetComponent<Main> ();

        gameHUD_Canvas = GameObject.Find("GameHUD_Canvas");

        playerShield = GameObject.Find("Shield");
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

        updateHealth();
        updateShields();

		if (scoreCooldownTimer > 0) {
			scoreCooldownTimer--;
		}
    }

    private void updateHealth()
    {
        shieldsLabel.text = "Health";
        if (player.health <= 25)
        {
            health.text = "" + player.health + "%";
            health.color = Color.red;
        }
        else if (player.health <= 50)
        {
            health.text = "" + player.health + "%";
            health.color = new Color(255,128,0);
        }
        else if (player.health <= 75)
        {
            health.text = "" + player.health + "%";
            health.color = Color.yellow;
        }
        else if (player.health <= 100)
        {
            health.text = "" + player.health + "%";
            health.color = Color.green;
        }
    }

    private void updateShields()
    {
        shieldsLabel.text = "Shields";
        shields.text = "" + player.shields + "%";
        shields.color = new Color(0, 128, 255);

        if (player.shields <= 0)
        {
            playerShield.GetComponent<MeshRenderer>().enabled = false;
		}else if(player.shields <= 75)
		{
			//Do something	
		}
        else
        {
            playerShield.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}


