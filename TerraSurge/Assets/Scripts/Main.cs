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
	// origin point for objects
    public GameObject origin;
	// original ship location
    public GameObject shiporigin;
	// the ship
    public Transform ship;
	//array of objects used for loading/spawning obstacles
	public GameObject[] objMap;

	// audio source containing the track
	public AudioSource audio1;
	// map of <Object spawn time:spawned object id> based on music
	private SortedDictionary<int,int> audio1Map;
	// index of the next object to be loaded
	private int nextEntry;
	// index of next terrain chunk
	private int nextTerrain;
	// tracer data
	// current tracer index
	private int currentTracer = 0;
	// previous tracer
	private int processedTracer = 0;
	// next tracer
	private int nextTracer = 0;

	// actualy the ships health/hp
    public int health = 100;
    // ship shields
    public int shields = 100;
	// score
    public int score; //score
	// max left/right movement
	public float XLimit = 5;		
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
    public GameObject initialterrain;
	// maximum number of each object in the pool
	public int numberOfEachObject;
	// the main tracer prefab(ie dust cloud or something)
    public GameObject tracer;
	// pool of tracers
	private GameObject[] tracers;
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

	private float turnLeft 	= 0f;
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

    int previousFrameTimer;

    // Starts the game by initialising all variables
    void Start ()
    {
		shipAnimator = GetComponentsInChildren<Animator> ()[0];
        // initialising vriables
		terrainDuration = 200/shipSpeed;
		prevTerrain = 0;
		nextTerrain = 0;
		nextEntry = 0;
		audio1 = GetComponent<AudioSource> ();
		//loads first sound track data
		loadAudio1 ();

		// sets the ship to the origin
        this.gameObject.transform.position = shiporigin.transform.position;

		//initialising pools
		initPool ();

        // Get MenuSystem
        menuSystem = GameObject.Find("ShipCamera").GetComponent<MenuSystem>();

        //begin music
        audio1.Play();
        initialterrain.transform.position = terrainOrigin.transform.position - Vector3.forward*256;
        initialterrain.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -shipSpeed);
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
        // pre calculated ratio to avoid integer overflow
        float ratio = ppqn * tempo  / 60000;
		// calculate midi ticks based on ratio
        int realticks = (int)(timeMS *ratio);
		// offset for second offset(tracers)
        int ticks = realticks + (int)(secondoffset *1000 * ratio);
        // next object to be picked up
        KeyValuePair<int, int> next = audio1Map.ElementAt(nextEntry);
        // generate tracer
        /*
        if (nextTracer< audio1Map.Count &&  ticks > audio1Map.ElementAt(nextTracer).Key)
        {
            GameObject spawnedTracer = tracers[currentTracer];
            spawnedTracer.SetActive(true);
            spawnedTracer.transform.position = this.transform.position + Vector3.forward * shipSpeed* secondoffset+Vector3.forward*setDistanceInFrontOfPlayer + Vector3.left * minorNextObjXOff;
            if(next.Value == 3|| next.Value == 7)
            {
                spawnedTracer.transform.position = spawnedTracer.transform.position + Vector3.left * nextObjXOff;
            }
			//randomising spawn of good objects
			if (audio1Map.ElementAt (currentTracer).Value == 3 || audio1Map.ElementAt (currentTracer).Value == 7|| audio1Map.ElementAt(currentTracer).Value == 4) {
				Vector3 origPos = spawnedTracer.transform.position;
				spawnedTracer.transform.position = new Vector3 ( shiporigin.transform.position.x+nextObjXOff, origPos.y,origPos.z);
			}
            spawnedTracer.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -shipSpeed);
            currentTracer++;
            if (currentTracer >= 100) currentTracer = 0;
            nextTracer++;
		}
		// generate object
        if (realticks > next.Key && bCanSpawn) {
            GameObject spawned = pool [next.Value,pooltracker[next.Value]];
			pooltracker[next.Value]++;
			if (pooltracker [next.Value] >= numberOfEachObject) {
				pooltracker [next.Value] = 0;
			}
			//replace tracer with object
			spawned.SetActive(true);
            tracers[processedTracer].SetActive(false);
            spawned.transform.position = tracers[processedTracer].transform.position;
            spawned.GetComponent<Rigidbody>().velocity = new Vector3(0,0,-shipSpeed);
			nextEntry++;
            //incrementing processed tracer id
            processedTracer++;
            if (processedTracer >= 100) processedTracer = 0;
        }*/
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

				if (turnRight < 1f){
					turnRight += 0.2f;
				}
				performMove ();
			} else {
				turnRight = 0;
			}

			if ((Input.GetKey (KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || Input.GetButtonDown ("X")) && this.gameObject.transform.position.x >= -XLimit) {
				CalcAcceleration (-1f);
				if (turnLeft < 1f){
					turnLeft += 0.2f;
				}
				performMove ();
			} else {
				turnLeft = 0;
			}

			shipAnimator.SetFloat ("LeftAmount", turnLeft);
			shipAnimator.SetFloat ("RightAmount", turnRight);

			//Friction
			currentVelocity *= 0.9f;

        }

        // Level Complete
        if (!menuSystem.isActive() && audio1.time >= audio1.clip.length)
        {
            menuSystem.LevelComplete();
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
//			print ("Hit terrain");
            return;
        }
		// add shields, do nothing if at max shields
        else if (collision.gameObject.tag == "Shield")
        {
            pickupShields();
            Destroy(collision.gameObject);
        }
        // add health, do nothing if at max health
        else if (collision.gameObject.tag == "Health")
        {
            pickupHealth();
            Destroy(collision.gameObject);
        }
        // hit a score tag object
        else if (collision.gameObject.tag == "Score")
        {
            //print("HIT SCORE!!!!!");
            //pickupScore();
            //Destroy(collision.gameObject);
        }
        // disable tracer visual on hit(should not happeN)
        else if ( collision.gameObject.tag == "Tracer")
        {
            collision.gameObject.SetActive(false);
        }

        // hit a bad object
        else {
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
            Destroy (collision.gameObject);
        }
    }

	//initiliases the pool
	void initPool(){
        //intialise tracers
        tracers = new GameObject[100];
        for(int t = 0; t < 100; t++)
        {
            tracers[t] = Instantiate(tracer, tracer.transform.position, Quaternion.identity) as GameObject;
        }
        //initialise obstacles
		pool = new GameObject[objMap.Length,numberOfEachObject];
		pooltracker = new int[objMap.Length];
		for(int i=0;i<objMap.Length;i++){
			pooltracker [i] = 0;
			for(int j=0;j<numberOfEachObject;j++){
				pool[i,j]= Instantiate(objMap[i], objMap[i].transform.position,Quaternion.identity) as GameObject;
			}
		}

	}

    // Sounds for Shield Collisions
    private void collisionShields()
    {
        // Camera Shake!
        GameObject.Find("ShipCamera").GetComponent<CameraShake>().shakeDuration = 1.0f;
        GameObject.Find("ShipCamera").GetComponent<CameraShake>().shakeAmount = 0.7f;

        // Audio for shields hit
        AudioSource shieldsCollisionAudio = GameObject.Find("ShieldCollision").GetComponent<AudioSource>();
        shieldsCollisionAudio.Play();
    }

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
        health = health + 15;
        if (health > 100) health = 100;

        // Audio for Health pickups
        AudioSource audio = GameObject.Find("Pickup2").GetComponent<AudioSource>();
        audio.Play();
    }

    private void pickupShields()
    {
        shields = shields + 15;
        if (shields > 100) shields = 100;

        // Audio for shield pickups
        AudioSource audio = GameObject.Find("Pickup3").GetComponent<AudioSource>();
        audio.Play();
    }

    public void GameOver()
    {
        // Call menuSystem GameOver
        menuSystem.GameOver();
    }

    public AudioSource getAudio()
    {
        return audio1;
    }

	// load audio data
	void loadAudio1(){
		audio1Map = new SortedDictionary<int,int> ();
        /*audio1Map[0] = 2;
        audio1Map[3840] = 2;
        audio1Map[7680] = 2;
        audio1Map[11520] = 2;
        audio1Map[15360] = 2;
        audio1Map[19200] = 2;
        audio1Map[23040] = 2;
        audio1Map[26880] = 2;
        audio1Map[30720] = 2;
        audio1Map[34560] = 2;
        audio1Map[38400] = 2;
        audio1Map[42240] = 2;
        audio1Map[46080] = 2;
        audio1Map[49920] = 2;
        audio1Map[53760] = 2;
        audio1Map[57600] = 2;
        audio1Map[61440] = 2;
        audio1Map[65280] = 2;
        audio1Map[69120] = 2;
        audio1Map[72960] = 2;
        audio1Map[107520] = 2;
        audio1Map[108101] = 2;
        audio1Map[109440] = 3;
        audio1Map[110080] = 3;
        audio1Map[110880] = 3;
        audio1Map[111360] = 2;
        audio1Map[111360] = 3;
        audio1Map[111840] = 3;
        audio1Map[113280] = 3;
        audio1Map[113920] = 3;
        audio1Map[114720] = 3;
        audio1Map[115200] = 2;
        audio1Map[115200] = 3;
        audio1Map[115781] = 2;
        audio1Map[117120] = 3;
        audio1Map[117760] = 3;
        audio1Map[118560] = 3;
        audio1Map[119040] = 2;
        audio1Map[119040] = 3;
        audio1Map[119520] = 3;
        audio1Map[120960] = 3;
        audio1Map[121600] = 3;
        audio1Map[122400] = 3;
        audio1Map[122880] = 2;
        audio1Map[122880] = 3;
        audio1Map[123461] = 2;
        audio1Map[124800] = 3;
        audio1Map[125440] = 3;
        audio1Map[126240] = 3;
        audio1Map[126720] = 2;
        audio1Map[126720] = 3;
        audio1Map[127200] = 3;
        audio1Map[128640] = 3;
        audio1Map[129280] = 3;
        audio1Map[130080] = 3;
        audio1Map[130560] = 2;
        audio1Map[130560] = 3;
        audio1Map[131141] = 2;
        audio1Map[132480] = 3;
        audio1Map[133120] = 3;
        audio1Map[133920] = 3;
        audio1Map[134400] = 2;
        audio1Map[134400] = 3;
        audio1Map[134880] = 3;
        audio1Map[136320] = 3;
        audio1Map[136960] = 3;
        audio1Map[137760] = 3;
        audio1Map[138240] = 2;
        audio1Map[138240] = 3;
        audio1Map[138821] = 2;
        audio1Map[140160] = 3;
        audio1Map[140800] = 3;
        audio1Map[141600] = 3;
        audio1Map[142080] = 2;
        audio1Map[142080] = 3;
        audio1Map[142560] = 3;
        audio1Map[144000] = 3;
        audio1Map[144640] = 3;
        audio1Map[145440] = 3;
        audio1Map[145920] = 2;
        audio1Map[145920] = 3;
        audio1Map[146501] = 2;
        audio1Map[147840] = 3;
        audio1Map[148480] = 3;
        audio1Map[149280] = 3;
        audio1Map[149760] = 2;
        audio1Map[149760] = 3;
        audio1Map[150240] = 3;
        audio1Map[151680] = 3;
        audio1Map[152320] = 3;
        audio1Map[153120] = 3;
        audio1Map[153600] = 2;
        audio1Map[153600] = 3;
        audio1Map[154181] = 2;
        audio1Map[155520] = 3;
        audio1Map[156160] = 3;
        audio1Map[156960] = 3;
        audio1Map[157440] = 2;
        audio1Map[157440] = 3;
        audio1Map[157920] = 3;
        audio1Map[159360] = 3;
        audio1Map[160000] = 3;
        audio1Map[160800] = 3;
        audio1Map[161280] = 2;
        audio1Map[161280] = 3;
        audio1Map[161861] = 2;
        audio1Map[163200] = 3;
        audio1Map[163840] = 3;
        audio1Map[164640] = 3;
        audio1Map[165120] = 2;
        audio1Map[165120] = 3;
        audio1Map[165600] = 3;
        audio1Map[167040] = 3;
        audio1Map[167680] = 3;
        audio1Map[168480] = 3;
        audio1Map[168960] = 2;
        audio1Map[168960] = 3;
        audio1Map[172800] = 2;
        audio1Map[176640] = 2;
        audio1Map[180480] = 2;
        audio1Map[184320] = 2;
        audio1Map[184901] = 2;
        audio1Map[186240] = 3;
        audio1Map[186880] = 3;
        audio1Map[187680] = 3;
        audio1Map[188160] = 2;
        audio1Map[188160] = 3;
        audio1Map[188640] = 3;
        audio1Map[190080] = 3;
        audio1Map[190720] = 3;
        audio1Map[191520] = 3;
        audio1Map[192000] = 2;
        audio1Map[192000] = 3;
        audio1Map[192581] = 2;
        audio1Map[193920] = 3;
        audio1Map[194560] = 3;
        audio1Map[195360] = 3;
        audio1Map[195840] = 2;
        audio1Map[195840] = 3;
        audio1Map[196320] = 3;
        audio1Map[197760] = 3;
        audio1Map[198400] = 3;
        audio1Map[199200] = 3;
        audio1Map[199680] = 2;
        audio1Map[199680] = 3;
        audio1Map[200261] = 2;
        audio1Map[201600] = 3;
        audio1Map[202240] = 3;
        audio1Map[203040] = 3;
        audio1Map[203520] = 2;
        audio1Map[203520] = 3;
        audio1Map[204000] = 3;
        audio1Map[205440] = 3;
        audio1Map[206080] = 3;
        audio1Map[206880] = 3;
        audio1Map[207360] = 2;
        audio1Map[207360] = 3;
        audio1Map[207941] = 2;
        audio1Map[209280] = 3;
        audio1Map[209920] = 3;
        audio1Map[210720] = 3;
        audio1Map[211200] = 2;
        audio1Map[211200] = 3;
        audio1Map[211680] = 3;
        audio1Map[213120] = 3;
        audio1Map[213760] = 3;
        audio1Map[214560] = 3;
        audio1Map[215040] = 2;
        audio1Map[215040] = 3;
        audio1Map[215621] = 2;
        audio1Map[216960] = 3;
        audio1Map[217600] = 3;
        audio1Map[218400] = 3;
        audio1Map[218880] = 2;
        audio1Map[218880] = 3;
        audio1Map[219360] = 3;
        audio1Map[220800] = 3;
        audio1Map[221440] = 3;
        audio1Map[222240] = 3;
        audio1Map[222720] = 2;
        audio1Map[222720] = 3;
        audio1Map[223301] = 2;
        audio1Map[224640] = 3;
        audio1Map[225280] = 3;
        audio1Map[226080] = 3;
        audio1Map[226560] = 2;
        audio1Map[226560] = 3;
        audio1Map[227040] = 3;
        audio1Map[228480] = 3;
        audio1Map[229120] = 3;
        audio1Map[229920] = 3;
        audio1Map[230400] = 3;
        audio1Map[230880] = 3;
        audio1Map[231520] = 3;*/

        audio1Map[45] = 5;
        audio1Map[147] = 6;
        audio1Map[164] = 0;
        audio1Map[320] = 7;
        audio1Map[981] = 6;
        audio1Map[1649] = 4;
        audio1Map[1761] = 5;
        audio1Map[2864] = 3;
        audio1Map[2961] = 6;
        audio1Map[3120] = 6;
        audio1Map[3217] = 5;
        audio1Map[4026] = 2;
        audio1Map[5220] = 6;
        audio1Map[6050] = 6;
        audio1Map[6554] = 4;
        audio1Map[6662] = 4;
        audio1Map[6930] = 6;
        audio1Map[7723] = 2;
        audio1Map[7783] = 6;
        audio1Map[8446] = 2;
        audio1Map[8461] = 4;
        audio1Map[8833] = 3;
        audio1Map[9628] = 6;
        audio1Map[10547] = 2;
        audio1Map[13484] = 2;
        audio1Map[13877] = 2;
        audio1Map[14919] = 1;
        audio1Map[15265] = 5;
        audio1Map[16061] = 4;
        audio1Map[16201] = 3;
        audio1Map[16569] = 4;
        audio1Map[16721] = 0;
        audio1Map[17330] = 0;
        audio1Map[17752] = 6;
        audio1Map[18501] = 3;
        audio1Map[18732] = 6;
        audio1Map[18776] = 3;
        audio1Map[19144] = 2;
        audio1Map[19348] = 2;
        audio1Map[19824] = 6;
        audio1Map[20725] = 6;
        audio1Map[22921] = 6;
        audio1Map[23400] = 5;
        audio1Map[23974] = 6;
        audio1Map[24204] = 4;
        audio1Map[24758] = 4;
        audio1Map[25280] = 0;
        audio1Map[25305] = 0;
        audio1Map[26738] = 0;
        audio1Map[29515] = 2;
        audio1Map[29579] = 6;
        audio1Map[30365] = 5;
        audio1Map[30975] = 7;
        audio1Map[31025] = 1;
        audio1Map[31410] = 0;
        audio1Map[33182] = 4;
        audio1Map[33810] = 6;
        audio1Map[33816] = 6;
        audio1Map[35468] = 7;
        audio1Map[37182] = 7;
        audio1Map[37234] = 4;
        audio1Map[38906] = 5;
        audio1Map[39619] = 1;
        audio1Map[39730] = 5;
        audio1Map[40106] = 6;
        audio1Map[40499] = 1;
        audio1Map[41148] = 0;
        audio1Map[41805] = 0;
        audio1Map[42563] = 4;
        audio1Map[42756] = 3;
        audio1Map[42816] = 3;
        audio1Map[42891] = 2;
        audio1Map[43355] = 4;
        audio1Map[44408] = 0;
        audio1Map[45067] = 1;
        audio1Map[45597] = 6;
        audio1Map[45936] = 2;
        audio1Map[47879] = 6;
        audio1Map[48823] = 6;
        audio1Map[49354] = 4;
        audio1Map[49880] = 7;
        audio1Map[49994] = 2;
        audio1Map[50479] = 6;
        audio1Map[50496] = 3;
        audio1Map[51138] = 1;
        audio1Map[52091] = 4;
        audio1Map[52807] = 0;
        audio1Map[54003] = 4;
        audio1Map[54791] = 3;
        audio1Map[54922] = 4;
        audio1Map[55142] = 1;
        audio1Map[56122] = 5;
        audio1Map[56823] = 5;
        audio1Map[56896] = 3;
        audio1Map[57262] = 6;
        audio1Map[57470] = 6;
        audio1Map[57640] = 1;
        audio1Map[57676] = 6;
        audio1Map[57911] = 5;
        audio1Map[59334] = 0;
        audio1Map[60496] = 3;
        audio1Map[61095] = 7;
        audio1Map[61116] = 6;
        audio1Map[61156] = 7;
        audio1Map[61201] = 7;
        audio1Map[61447] = 1;
        audio1Map[62360] = 5;
        audio1Map[62497] = 0;
        audio1Map[63658] = 7;
        audio1Map[63866] = 1;
        audio1Map[64224] = 2;
        audio1Map[65235] = 4;
        audio1Map[65586] = 5;
        audio1Map[66649] = 7;
        audio1Map[66822] = 1;
        audio1Map[66903] = 0;
        audio1Map[67468] = 1;
        audio1Map[67562] = 5;
        audio1Map[67787] = 2;
        audio1Map[69619] = 0;
        audio1Map[69960] = 4;
        audio1Map[70272] = 4;
        audio1Map[70557] = 7;
        audio1Map[70564] = 4;
        audio1Map[70892] = 1;
        audio1Map[72080] = 3;
        audio1Map[74378] = 7;
        audio1Map[75780] = 5;
        audio1Map[76806] = 7;
        audio1Map[77016] = 6;
        audio1Map[78197] = 1;
        audio1Map[78378] = 1;
        audio1Map[78592] = 6;
        audio1Map[79595] = 2;
        audio1Map[81325] = 3;
        audio1Map[81777] = 6;
        audio1Map[82282] = 3;
        audio1Map[82328] = 7;
        audio1Map[82752] = 5;
        audio1Map[83326] = 7;
        audio1Map[83328] = 0;
        audio1Map[83900] = 4;
        audio1Map[86124] = 3;
        audio1Map[86596] = 6;
        audio1Map[87549] = 7;
        audio1Map[88840] = 5;
        audio1Map[89242] = 2;
        audio1Map[89267] = 7;
        audio1Map[91784] = 4;
        audio1Map[94459] = 5;
        audio1Map[94798] = 1;
        audio1Map[97229] = 3;
        audio1Map[97668] = 2;
        audio1Map[98926] = 4;
        audio1Map[100172] = 2;
        audio1Map[102321] = 5;
        audio1Map[103109] = 2;
        audio1Map[103569] = 5;
        audio1Map[103756] = 1;
        audio1Map[104461] = 3;
        audio1Map[105160] = 3;
        audio1Map[105379] = 4;
        audio1Map[107513] = 5;
        audio1Map[107800] = 3;
        audio1Map[107816] = 3;
        audio1Map[110202] = 5;
        audio1Map[110223] = 0;
        audio1Map[111656] = 1;
        audio1Map[112143] = 3;
        audio1Map[112627] = 5;
        audio1Map[112652] = 2;
        audio1Map[112654] = 1;
        audio1Map[113740] = 3;
        audio1Map[115779] = 1;
        audio1Map[116209] = 4;
        audio1Map[116348] = 0;
        audio1Map[117954] = 1;
        audio1Map[118670] = 2;
        audio1Map[119610] = 4;
        audio1Map[119857] = 2;
        audio1Map[120045] = 2;
        audio1Map[120224] = 4;
        audio1Map[121043] = 1;
        audio1Map[121291] = 0;
        audio1Map[121413] = 4;
        audio1Map[122324] = 3;
        audio1Map[122393] = 1;
        audio1Map[122738] = 0;
        audio1Map[123541] = 6;
        audio1Map[123770] = 1;
        audio1Map[124417] = 5;

    }
    
}


