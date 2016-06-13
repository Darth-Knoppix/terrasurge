using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.IO;

public class Main : MonoBehaviour {

	//Switches to disable functionality
	public bool bCanSpawn;
    // cheating prick
    public bool invincible;
	// origin point for objects
    public GameObject origin;
	// original ship location
    public GameObject shiporigin;
	// the ship
    public Transform ship;

	// audio source containing the track
	public AudioSource audio1;
	// index of next terrain chunk
	private int nextTerrain;
	//Animator for player shield 
	private Animator shieldAnim;

	// actualy the ships health/hp
    public int health = 100;
    // ship shields
    public int shields = 100;
	// score
    public int score; //score
                      // max left/right movement
    public float XLimit;	
	// relative velocity of the objects to the ship
	public float shipSpeed;
	// time difference from generation to impact
    public int secondoffset;
    // distance in front of player objects pop at
    public int setDistanceInFrontOfPlayer;

    // pools of objects. These are initialised on startup
    // then future objects are not instantiated, but instead loaded
    // from the pool to prevent lag
    // obstacles
    private GameObject[,] pool;
	// used to track index of latest used object from the pool
	private int [] pooltracker;
    public GameObject[] initialterrain;
    // maximum number of each object in the pool
    public int numberOfEachObject;
	// pool of terrain objects
	public GameObject[] terrainMap;
	// time for each terrain object to fly past player(256/shipspeed)
	public float terrainDuration;
	// id of previous
	private int prevTerrain;
	// spawn of the terrain objects
	public GameObject terrainOrigin;

	private Animator shipAnimator;

	// Acceleration for ship side movement
	public float currentVelocity 	= 0.0f;
	public float accelerationRate 	= 1.0f;
	private float direction 		= 0.0f;

	private float tilt 	= 0f;

    // MenuSystem script for Game navigation
    private MenuSystem menuSystem;

    //score multiplier
    public float scoreMultiplier = 1;

    // First Song Length
    // For level complete state testing
    private int songLengthMilliseconds = 5000;

    int previousFrameTimer;

    // Starts the game by initialising all variables
    void Start()
    {
        shipAnimator = GetComponentsInChildren<Animator>()[0];
        // initialising vriables
        terrainDuration = 239 / shipSpeed;
        prevTerrain = 0;
        nextTerrain = 0;

        // sets the ship to the origin
        this.gameObject.transform.position = shiporigin.transform.position;

        //initialising pools
        //		initPool ();

        // Get MenuSystem
        menuSystem = GameObject.Find("ShipCamera").GetComponent<MenuSystem>();
        //		Debug.Log (menuSystem);

        //begin music
        audio1.Play();
        
        // initiate terrain
        for (int i = 0; i < initialterrain.Length; i++)
        {
            initialterrain[i].transform.position = terrainOrigin.transform.position + Vector3.forward * (150 - i * 235);
            initialterrain[i].GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -shipSpeed);
        }

        shieldAnim = GameObject.Find("Shield").GetComponent<Animator> ();
    }

	// Update is called once per frame
	void Update () {
        // a random X offset value for 'good' pickups to force the player to move
        // in order to hit them
        float nextObjXOff = UnityEngine.Random.Range(-5F, 5F);
        float minorNextObjXOff = UnityEngine.Random.Range(-3F, 3F);
        //float spawnNextObj = UnityEngine.Random.Range(-100F, 1F);

        // current audio playtime
        double playtime = audio1.time;

        // convert time in seconds to time in ms
        int timeMS = (int)(playtime * 1000);
		// generate terrain
		if (timeMS > terrainDuration * nextTerrain*1000) {
			int maxTerrain = terrainMap.Length;
			//print(maxTerrain
			int nextTerrainIndex = nextTerrain%maxTerrain;
//			print (nextTerrainIndex);
			GameObject spawned = terrainMap [nextTerrainIndex];// [next.Value];
			spawned.transform.position = terrainOrigin.transform.position;
			spawned.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, -shipSpeed);
				nextTerrain++;
		}

        // Movement check gameover
        if (!menuSystem.isActive())
		{
			if ((Input.GetKey (KeyCode.D) || Input.GetKey (KeyCode.RightArrow) || Input.GetButtonDown ("B")) && this.gameObject.transform.position.x <= XLimit) {
				CalcAcceleration (1f);
				if (tilt <= 1f) {
					tilt += 0.1f;
				}
				performMove ();
			} else if ((Input.GetKey (KeyCode.A) || Input.GetKey (KeyCode.LeftArrow) || Input.GetButtonDown ("X")) && this.gameObject.transform.position.x >= -XLimit) {
				CalcAcceleration (-1f);
				if (tilt >= -1f) {
					tilt -= 0.1f;
				}
				performMove ();
			} else {
				tilt *= 0.8f;
			}

			shipAnimator.SetFloat ("Tilt", tilt);

			//Friction
			currentVelocity *= 0.9f;

        }

        // Level Complete
        if (!menuSystem.isActive() && audio1.time >= audio1.clip.length)
        {
            menuSystem.levelComplete();
        }
    }

	void performMove(){
		if (this.gameObject.transform.position.x > XLimit) {
			this.gameObject.transform.position = new Vector3 (
				XLimit,
				this.gameObject.transform.position.y,
				this.gameObject.transform.position.z
			);
		} else if (this.gameObject.transform.position.x < -XLimit){
			this.gameObject.transform.position = new Vector3 (
				-XLimit,
				this.gameObject.transform.position.y,
				this.gameObject.transform.position.z
			);
		}else {
			this.gameObject.transform.position = new Vector3 (
				this.gameObject.transform.position.x + (currentVelocity),
				this.gameObject.transform.position.y,
				this.gameObject.transform.position.z
			);
		}
	}

	void CalcAcceleration(float dir){
		currentVelocity += ((dir * accelerationRate) * Time.deltaTime);
	}

	//test for collision
    void OnCollisionEnter(Collision collision)
    {
		// do nothing if hit terrain
        if(collision.gameObject.tag == "Terrain")
        {
		//	print ("Hit terrain");
            return;
        }
		// add shields, do nothing if at max shields
        else if (collision.gameObject.tag == "Shield")
        {
            pickupShields();
        }
        // add health, do nothing if at max health
        else if (collision.gameObject.tag == "Health")
        {
            pickupHealth();
        }
        // hit a score tag object
        else if (collision.gameObject.tag == "Score")
        {
            //print("HIT SCORE!!!!!");
            //pickupScore();
            //Destroy(collision.gameObject);
        }

        // hit a bad object
        else if(!invincible)
        {
			//reset score multiplier
            scoreMultiplier = 1;
            // hit something with ship shields remaining
            if (shields > 0)
            {
                collisionShields();
                shields = shields - 25;
                if (shields < 0) shields = 0;
            }
            // hit something reduce health
            else if (health > 0)
            {
                collisionHealth();
                health = health - 25;
                if (health < 0) health = 0;
            }
            // no more player health gameover
			if (health <= 0) {
                GameOver();
            }
        }
        
        collision.gameObject.SetActive(false);
    }


    // Sounds for Shield Collisions
    private void collisionShields()
    {
		shieldAnim.SetTrigger ("TakeHit");
        // Camera Shake!
        GameObject.Find("ShipCamera").GetComponent<CameraShake>().shakeDuration = 1.0f;
        GameObject.Find("ShipCamera").GetComponent<CameraShake>().shakeAmount = 0.7f;

        // Audio for shields hit
        AudioSource shieldsCollisionAudio = GameObject.Find("ShieldCollision").GetComponent<AudioSource>();
        shieldsCollisionAudio.Play();
    }

    // Sounds for Health Collisions
    private void collisionHealth()
    {
        // Camera Shake!
        GameObject.Find("ShipCamera").GetComponent<CameraShake>().shakeDuration = 1.0f;
        GameObject.Find("ShipCamera").GetComponent<CameraShake>().shakeAmount = 0.7f;

        // Audio for Health hit
        AudioSource shieldsCollisionAudio = GameObject.Find("HealthCollision").GetComponent<AudioSource>();
        shieldsCollisionAudio.Play();
    }

    private void pickupScore()
    {
        // Audio for score pickups
        AudioSource audio = GameObject.Find("Pickup1").GetComponent<AudioSource>();
        audio.Play();
    }

    private void pickupHealth()
    {
        if (health >= 100)
        {
            // health is full, add to score instead
            ScoreController _score = GameObject.Find("GameManager").GetComponent<ScoreController>();
            ScorePickupController scoreC = FindObjectOfType<ScorePickupController>() as ScorePickupController;
            _score.incrementScore(scoreC.scoreAmount);
        }
        else
        {
            // add to health
            health = health + 25;
            if (health > 100) health = 100;
        }
        // Audio for Health pickups
        AudioSource audio = GameObject.Find("Pickup_Health").GetComponent<AudioSource>();
        audio.Play();
    }

    private void pickupShields()
    {
        if (shields >= 100)
        {
            // shields are full, add to score instead
            ScoreController score = FindObjectOfType<ScoreController>() as ScoreController;
            ScorePickupController scoreC = FindObjectOfType<ScorePickupController>() as ScorePickupController;
            score.incrementScore(scoreC.scoreAmount);
        }
        else
        {
            // add to shields
            shields = shields + 25;
            if (shields > 100) shields = 100;
        }
        // Audio for shield pickups
        AudioSource audio = GameObject.Find("Pickup_Shields").GetComponent<AudioSource>();
        audio.Play();
    }

    public void GameOver()
    {
        // Call menuSystem GameOver
        menuSystem.gameOver();
    }

    public AudioSource getAudio()
    {
        return audio1;
    }
    
}


