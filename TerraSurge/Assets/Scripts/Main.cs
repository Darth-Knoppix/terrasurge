using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;
using System.IO;

public class Main : MonoBehaviour {
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
	// tracer data
	// current tracer index
	private int currentTracer = 0;
	// previous tracer
	private int processedTracer = 0;
	// next tracer
	private int nextTracer = 0;

	// actualy the ships shields/hp
    public int lives = 10000; 
	// score
    public int score; //score
	// max left/right movement
	public float XLimit = 5;		
	// relative velocity of the objects to the ship
	public float shipSpeed;		
	// first offset(time between music beats and object spawn)
    public float firstoffset = 0F;
	// time difference from generation to impact
    public int secondoffset = 3;

	// pools of objects
	// obstacles
	private GameObject[,] pool;
	// used to track index of latest used object from the pool
	private int [] pooltracker;
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

    //for music calculations DONT CHANGE
    int ppqn = 480;
    int tempo = 260;

    // GameOver canvas
    private GameObject canvas;
    // Timescale
    private float timescale;

    //score multiplier
    public float scoremx = 1;

    int previousFrameTimer;

    // Use this for initialization
    void Start ()
    {
		terrainDuration = 256/shipSpeed;
		prevTerrain = 0;
		nextEntry = 0;
		audio1 = GetComponent<AudioSource> ();
		//loads first sound track data
		loadAudio1 ();

		// sets the ship to the origin
        this.gameObject.transform.position = shiporigin.transform.position;

		//inits the pools
		initPool ();

        //Setup GameOver Canvas
        canvas = GameObject.Find("GameOver_Canvas");
        canvas.SetActive(false);

		//begin music
        audio1.Play();
    }

	// Update is called once per frame
	void Update () {
		//shipSpeed = shipSpeed + scoremx * 10;
        if (GameObject.Find("GameOver_Canvas") == null && GameObject.Find("PausedGame_Canvas") == null)
        {
            score = (int)(score + scoremx);
			// score multiplier
            scoremx = scoremx + 0.05F;
        }
		// random numbers for object spawn
        float spawnObjRN = UnityEngine.Random.Range(0, 200);
		float nextObjXOff = UnityEngine.Random.Range(-5F, 5F);
		float nextObjYOff = UnityEngine.Random.Range(-XLimit, XLimit);

		// audio playtime
		double playtime = audio1.time;
		// increases 'speed' of object spawn
        //playtime = playtime * 1.05;

        // convert time in seconds to time in ms
        int timeMS = (int)(playtime * 1000);
		// offset by first offset
        timeMS = (int)(timeMS - firstoffset * 1000);
        // pre calculated ratio to avoid integer overflow
        float ratio = ppqn * tempo  / 60000;
		// calculate midi ticks based on ratio
        int realticks = (int)(timeMS *ratio);
		// offset for second offset(tracers)
        int ticks = realticks + (int)(secondoffset *1000 * ratio) ;
       
		// generate tracer
        if (ticks > audio1Map.ElementAt(nextTracer).Key)
        {
            GameObject spawnedTracer = tracers[currentTracer];
            spawnedTracer.SetActive(true);
            spawnedTracer.transform.position = this.transform.position + Vector3.forward * shipSpeed* secondoffset;
			//randomising spawn of good objects
			if (audio1Map.ElementAt (currentTracer).Value == 3 || audio1Map.ElementAt (currentTracer).Value == 7) {
				Vector3 origPos = spawnedTracer.transform.position;
				spawnedTracer.transform.position = new Vector3 ( shiporigin.transform.position.x+nextObjXOff, origPos.y,origPos.z);
			}
            spawnedTracer.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -shipSpeed);
            currentTracer++;
            if (currentTracer >= 100) currentTracer = 0;
            nextTracer++;
		}
		// generate object
		KeyValuePair<int,int> next = audio1Map.ElementAt(nextEntry);
        if (realticks > next.Key) {
            GameObject spawned = pool [next.Value,pooltracker[next.Value]];
			pooltracker[next.Value]++;
			if (pooltracker [next.Value] >= numberOfEachObject) {
				pooltracker [next.Value] = 0;
			}
			//replace tracer with object
			spawned.SetActive(true);
            tracers[processedTracer].SetActive(false);
            spawned.transform.position = tracers[processedTracer].transform.position;
            // randomly rotate the rocks
			if (next.Value == 2)
            {
                Vector3 rotate = new Vector3(UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360), UnityEngine.Random.Range(0, 360));
                spawned.transform.Rotate(rotate);
            }
            spawned.GetComponent<Rigidbody>().velocity = new Vector3(0,0,-shipSpeed);
			nextEntry++;
            //incrementing processed tracer id
            processedTracer++;
            if (processedTracer >= 100) processedTracer = 0;
        }
		// generate terrain
		if (timeMS > terrainDuration * prevTerrain*1000) {
			GameObject spawned = terrainMap [0];// [next.Value];
			GameObject obj = Instantiate (spawned, new Vector3(terrainOrigin.transform.position.x,terrainOrigin.transform.position.y,terrainOrigin.transform.position.z+ 128f), Quaternion.identity) as GameObject;

			//Create another terrain piece to ease pop in - Garbage implementation, replace ASAP and improve
			if(prevTerrain == 0){
				GameObject obj2 = Instantiate (spawned, new Vector3(terrainOrigin.transform.position.x,terrainOrigin.transform.position.y,terrainOrigin.transform.position.z), Quaternion.identity) as GameObject;
				obj2.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, -shipSpeed);
			}
			obj.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, -shipSpeed);
			prevTerrain++;
		}

        // Movement check gameover
        if (GameObject.Find("GameOver_Canvas") == null && GameObject.Find("PausedGame_Canvas") == null)
        {
            if ((Input.GetKey(KeyCode.D) || Input.GetButtonDown("B")) && (this.gameObject.transform.position.x - shiporigin.transform.position.x < XLimit))
            {
                ship.Translate(Vector3.right * 0.5F);
            }
            if ((Input.GetKey(KeyCode.A) || Input.GetButtonDown("X")) && (shiporigin.transform.position.x - this.gameObject.transform.position.x < XLimit))
            {
                ship.Translate(Vector3.left * 0.5F);
            }
        }
    }

	//test for collision
    void OnCollisionEnter(Collision collision)
    {
		// do nothing if hit terrain
        if(collision.gameObject.tag == "Terrain")
        {
			print ("Hit terrain");
            return;
        }
		// add score on pickup
        if (collision.gameObject.tag == "Score")
        {
            score+=500;
            print("score"+score);
			Destroy(collision.gameObject);
        }
		// add shields, do nothing if at max shields
        if (collision.gameObject.tag == "Shield")
        {
            lives = lives + 500;
            if (lives > 10000) lives = 10000;
        }
		// disable tracer visual on hit(should not happeN)
        if( collision.gameObject.tag == "Tracer")
        {
            collision.gameObject.SetActive(false);
        }
		// hit a bad object
        else {
			//reset score multiplier
            scoremx = 1;
			//lose lives
            lives = lives - (int)collision.rigidbody.mass * 500;
			if (lives <= 0) {
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

    public void GameOver()
    {
        Debug.Log("Game Over!");
        audio1.Pause();
        timescale = Time.timeScale;
        Time.timeScale = 0;
        canvas.SetActive(true);
    }

    public AudioSource getAudio()
    {
        return audio1;
    }


	// load audio data
	void loadAudio1(){
		audio1Map = new SortedDictionary<int,int> ();
        audio1Map[0] = 2;
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
        audio1Map[231520] = 3;

    }
    
}


