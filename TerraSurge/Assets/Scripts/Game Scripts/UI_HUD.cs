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
	public Image healthBar;

    public Text shields;
    public Text shieldsLabel;

	public int defaultTiemout = 40;
    public GameObject ship;

	private int lastScore;
	private int scoreCooldownTimer;
    private string values;

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
		healthBar = GameObject.Find("HealthBar").GetComponent<Image>();

        shields = GameObject.Find("Shields_Value").GetComponent<Text>();
        shieldsLabel = GameObject.Find("Shields_Label").GetComponent<Text>();

        scoreboard = GameObject.Find("GameManager").GetComponent<ScoreController> ();

        gameHUD_Canvas = GameObject.Find("GameHUD_Canvas");

        playerShield = GameObject.Find("Shield");
    }

    private int getHealth()
    {
        if (ship.GetComponent<Main>() != null)
        {
            return ship.GetComponent<Main>().health;
        }
        return ship.GetComponent<Main_AudioSync>().health;
    }

    private int getShields()
    {
        {
            if (ship.GetComponent<Main>() != null)
            {
                return ship.GetComponent<Main>().shields;
            }
            return ship.GetComponent<Main_AudioSync>().shields;
        }
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
        
		//String multi = player.scoreMultiplier.ToString ("n2"); // 2dp Number
		//multiplier.text = "Multiplier: \n" + multi;

        updateHealth();
        updateShields();

		if (scoreCooldownTimer > 0) {
			scoreCooldownTimer--;
		}
    }

    private void updateHealth()
    {
        shieldsLabel.text = "Health";
		healthBar.fillAmount = getHealth() / 100f;
        if (getHealth() <= 25)
        {
            health.text = "" + getHealth() + "%";
            health.color = Color.red;
			healthBar.color = Color.red;
        }
        else if (getHealth() <= 50)
        {
            health.text = "" + getHealth() + "%";
            health.color = new Color(255,128,0);
			healthBar.color = new Color(255,128,0);
        }
        else if (getHealth() <= 75)
        {
            health.text = "" + getHealth() + "%";
            health.color = Color.yellow;
			healthBar.color = Color.yellow;
        }
        else if (getHealth() <= 100)
        {
            health.text = "" + getHealth() + "%";
            health.color = Color.green;
			healthBar.color = Color.green;
        }
    }

    private void updateShields()
    {
        shieldsLabel.text = "Shields";
        shields.text = "" + getShields() + "%";
        shields.color = new Color(0, 128, 255);

        if (getShields() <= 0)
        {
            playerShield.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            playerShield.GetComponent<MeshRenderer>().enabled = true;
        }
    }
}


