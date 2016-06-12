using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Main_AudioSync : MonoBehaviour
{
    public bool debugMode;
    // original ship location
    public GameObject shiporigin;
    // the ship
    public GameObject ship;
    public GameObject[] powerups;
    public GameObject animationTrigger;

    // audio source containing the track
    public AudioSource audio1;
    // map of <Object spawn time:spawned object id> based on music
    private SortedDictionary<int, int> audio1Map;
    // index of the previous obstacle
    private int prevObstacleID = 0;
    // index of the next object to be loaded
    private int nextObstacleID = 0;
    // index of the previous powerup
    private int prevPOID = 0;
    // index of the next powerup
    private int nextPOID = 0;

	//Animator for player shield 
	private Animator shieldAnim;

    // actualy the ships shields/hp
    public int health = 100;
    public int shields = 100;
    // score
    public int score; //score
                      // max left/right movement
    public float XLimit;
    // relative velocity of the objects to the ship
    public float shipSpeed;
    // distance in front of player objects pop at
    public int setTimeInFrontOfPlayer;


    public GameObject initialterrain;
    // pool of terrain objects
    public GameObject[] terrainMap;
    // time for each terrain object to fly past player(256/shipspeed)
    public float terrainDuration;
    // id of previous
    private int prevTerrain = 0;
    // index of next terrain chunk
    private int nextTerrain = 0;
    // spawn of the terrain objects
    public GameObject terrainOrigin;
    public int objectSpawnYOffset;
    private bool hasStarted = false;

    float songtime;

    private Animator shipAnimator;

    // Acceleration for ship side movement
    public float currentVelocity = 0.0f;
    public float accelerationRate = 1.0f;
    private float direction = 0.0f;

    private float turnLeft = 0f;
    private float turnRight = 0f;

    //for music calculations DONT CHANGE
    int ppqn = 480;
    int tempo = 260;

    // MenuSystem script for Game navigation
    private MenuSystem menuSystem;

    //score multiplier
    public float scoreMultiplier = 1;

    // First Song Length
    // For level complete state testing
    private int songLengthMilliseconds = 5000;
    public int ticks;
    int previousFrameTimer;

    // Starts the game by initialising all variables
    void Start()
    {
        shipAnimator = GetComponentsInChildren<Animator>()[0];
        // initialising vriables
        terrainDuration = 200 / shipSpeed;
        //audio1 = GetComponent<AudioSource>();

        // sets the ship to the origin
        this.gameObject.transform.position = shiporigin.transform.position;


        // Get MenuSystem
        menuSystem = GameObject.Find("ShipCamera").GetComponent<MenuSystem>();

        initialterrain.transform.position = terrainOrigin.transform.position - Vector3.forward * 256;
        initialterrain.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -shipSpeed);
        //begin music
        //audio1.Play();
        songtime = 0F;

        shieldAnim = GameObject.Find("Shield").GetComponent<Animator> ();
    }

    // Update is called once per frame
    void Update()
    {
        songtime += Time.deltaTime;
        // current audio playtime
        double playtime = songtime;// audio1.time;
        // convert time in seconds to time in ms
        int timeMS = (int)(playtime * 1000);
       
        // generate terrain
        if (timeMS > terrainDuration * nextTerrain * 1000)
        {
            int maxTerrain = terrainMap.Length;
            //print(maxTerrain
            int nextTerrainIndex = nextTerrain % maxTerrain;
            //			print (nextTerrainIndex);
            GameObject spawned = terrainMap[nextTerrainIndex];// [next.Value];
            spawned.transform.position = terrainOrigin.transform.position;
            spawned.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -shipSpeed);
            nextTerrain++;
        }

        // Movement check gameover
        if (!menuSystem.isActive())
        {
            if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || Input.GetButtonDown("B")) && this.gameObject.transform.position.x <= XLimit)
            {
                CalcAcceleration(1f);

                if (turnRight < 1f)
                {
                    turnRight += 0.2f;
                }
                performMove();
            }
            else {
                turnRight = 0;
            }

            if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetButtonDown("X")) && this.gameObject.transform.position.x >= -XLimit)
            {
                CalcAcceleration(-1f);
                if (turnLeft < 1f)
                {
                    turnLeft += 0.2f;
                }
                performMove();
            }
            else {
                turnLeft = 0;
            }

            shipAnimator.SetFloat("LeftAmount", turnLeft);
            shipAnimator.SetFloat("RightAmount", turnRight);

            //Friction
            currentVelocity *= 0.9f;

        }

        // Level Complete
        if (!menuSystem.isActive() && audio1.time >= audio1.clip.length)
        {
            menuSystem.levelComplete();
        }
    }


    private void spawnAndMove(GameObject obj)
    {
        Vector3 spawnPoint = new Vector3(this.transform.position.x, this.transform.position.y - objectSpawnYOffset,
            animationTrigger.transform.position.z + setTimeInFrontOfPlayer * shipSpeed) + Vector3.left * UnityEngine.Random.Range(-10, 10);
        obj.transform.position = spawnPoint;
        obj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -shipSpeed);
    }

    void performMove()
    {
        if (this.gameObject.transform.position.x > XLimit)
        {
            this.gameObject.transform.position = new Vector3(
                XLimit,
                this.gameObject.transform.position.y,
                this.gameObject.transform.position.z
            );
        }
        else if (this.gameObject.transform.position.x < -XLimit)
        {
            this.gameObject.transform.position = new Vector3(
                -XLimit,
                this.gameObject.transform.position.y,
                this.gameObject.transform.position.z
            );
        }
        else {
            this.gameObject.transform.position = new Vector3(
                this.gameObject.transform.position.x + (currentVelocity),
                this.gameObject.transform.position.y,
                this.gameObject.transform.position.z
            );
        }
    }

    void CalcAcceleration(float dir)
    {
        currentVelocity += ((dir * accelerationRate) * Time.deltaTime);
    }

    //test for collision
    //test for collision
    void OnCollisionEnter(Collision collision)
    {
        // do nothing if hit terrain
        if (collision.gameObject.tag == "Terrain")
        {
            //			print ("Hit terrain");
            return;
        }
        // add shields, do nothing if at max shields
        else if (collision.gameObject.tag == "Shield")
        {
            pickupShields();
            collision.gameObject.SetActive(false);
        }
        // add health, do nothing if at max health
        else if (collision.gameObject.tag == "Health")
        {
            pickupHealth();
            collision.gameObject.SetActive(false);
        }
        // hit a score tag object
        else if (collision.gameObject.tag == "Score")
        {
            //print("HIT SCORE!!!!!");
            //pickupScore();
            //Destroy(collision.gameObject);
        }
        // disable tracer visual on hit(should not happeN)
        else if (collision.gameObject.tag == "Tracer")
        {
            collision.gameObject.SetActive(false);
        }

        // hit a bad object
        else if(!debugMode) {
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
            if (health <= 0)
            {
                GameOver();
            }
            collision.gameObject.SetActive(false);
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
            //pickupHealth score instead!
            pickupScore();

            ScoreController score = GameObject.Find("GameManager").GetComponent<ScoreController>();
            ScorePickupController scoreC = FindObjectOfType<ScorePickupController>() as ScorePickupController;
            score.incrementScore(scoreC.scoreAmount);
        }
        else
        {
            health = health + 25;
            if (health > 100) health = 100;

            // Audio for Health pickups
            AudioSource audio = GameObject.Find("Pickup2").GetComponent<AudioSource>();
            audio.Play();
        }
    }

    private void pickupShields()
    {
        if (shields >= 100)
        {
            //pickupHealth score instead!
            pickupScore();
            print("shields pickup as score!");
            ScoreController score = FindObjectOfType<ScoreController>() as ScoreController;
            ScorePickupController scoreC = FindObjectOfType<ScorePickupController>() as ScorePickupController;
            score.incrementScore(scoreC.scoreAmount);
        }
        else
        {
            shields = shields + 25;
            if (shields > 100) shields = 100;

            // Audio for shield pickups
            AudioSource audio = GameObject.Find("Pickup3").GetComponent<AudioSource>();
            audio.Play();
        }
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


