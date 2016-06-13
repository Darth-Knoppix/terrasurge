using UnityEngine;
using System.Collections;
using SynchronizerData;
using System.Collections.Generic;

public class Generator : MonoBehaviour
{
    // the ship
    public GameObject ship;
    private BeatObserver beatObserver;
    // % chance per tick to spawn a powerup
    public int powerupSpawnRate;
    // pools of objects. These are initialised on startup
    // then future objects are not instantiated, but instead loaded
    // from the pool to prevent lag
    // obstacles
    private GameObject[,] pool;
    // powerups
    private GameObject[,] poweruppool;
    // used to track index of latest used object from the pool
    private int[] pooltracker;
    private int[] poweruppooltracker;
    // maximum number of each object in the pool
    public int numberOfEachObject;
    //array of objects used for loading/spawning obstacles
    public GameObject[] objMap;
    // powerups
    public GameObject[] powerups;
    // wall that triggers anims
    public GameObject animationTrigger;

    // distance in front of player objects pop at
    public double setTimeInFrontOfPlayer;

    public int shipSpeed;
    public int objectSpawnYOffset;

    private double obstacleSpawnRate;
    void Start()
    {
        beatObserver = GetComponent<BeatObserver>();
        obstacleSpawnRate = 1000;
        initPool();
    }

    void Update()
    {
        obstacleSpawnRate += Time.deltaTime * 40;
        if (obstacleSpawnRate > 5999) obstacleSpawnRate = 5999;
        // OBSTACLE SPAWNING
        if ((beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat)
        {
            int objType = UnityEngine.Random.Range(0, objMap.Length); // figure out what to spawn
            chainSpawn(obstacleSpawnRate, objType); // chain spawn it
        }
        if ((beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat)
        {
            // do nothing
        }
        if ((beatObserver.beatMask & BeatType.OffBeat) == BeatType.OffBeat)
        {
            // do nothing
        }
        if ((beatObserver.beatMask & BeatType.OnBeat) == BeatType.OnBeat)
        {
            //spawnObject(1);
        }
        // powerups spawning
        int spawnOrNot = UnityEngine.Random.Range(0, 100);
		if (spawnOrNot < powerupSpawnRate && Time.timeScale != 0)
        {
            spawnPowerup();
        }
    }

    void spawnPowerup()
    {
        int wat2spawn =UnityEngine.Random.Range(0, powerups.Length);
        GameObject spawned = poweruppool[wat2spawn, poweruppooltracker[wat2spawn]];
        poweruppooltracker[wat2spawn]++;
        if (poweruppooltracker[wat2spawn] >= numberOfEachObject)
        {
            poweruppooltracker[wat2spawn] = 0;
        }
        // if it isnt null, we can move it
        if (spawned != null)
        {
            spawned.SetActive(true);
            movePowerup(spawned);
        }
    }

    private void movePowerup(GameObject obj)
    {
        obj.transform.position = new Vector3(this.transform.position.x+UnityEngine.Random.Range(-25,25), 
            ship.transform.position.y + objectSpawnYOffset, 
            animationTrigger.transform.position.z + 3);
        obj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -shipSpeed);
    }

    // recursive function to chain spawn objects
    void chainSpawn(double seed, int objType)
    {
        // terminal case
        double currentSeed = seed;
        int objCount = 0;

        float xOff = UnityEngine.Random.Range(-5f, 5f) + ship.transform.position.x;
        while (currentSeed > 0)
        {
            int shouldISPawn = UnityEngine.Random.Range(0, 1000);
            // recursive case
            if (currentSeed > shouldISPawn)    // spawn a new object if rng says yes
            {
                int xOffRatio;
                if (objCount == 0) xOffRatio = 0;
                else if (objCount % 2 == 0) xOffRatio = objCount/2;
                else xOffRatio = -1-objCount/2;
                spawnObject(objType,xOff,xOffRatio);  // generates a new objects
            }
            currentSeed = currentSeed - 1000;
            objCount++;
        }
    }

    void spawnObject(int type, float xOff, int xOffRatio)
    {
        int wat2spawn = type;// = UnityEngine.Random.Range(0, objMap.Length);
        GameObject spawned = pool[wat2spawn, pooltracker[wat2spawn]];
        pooltracker[wat2spawn]++;
        if (pooltracker[wat2spawn] >= numberOfEachObject)
        {
            pooltracker[wat2spawn] = 0;
        }
        // if it isnt null, we can move it
        if (spawned != null)
        {
            spawned.SetActive(true);
            moveObstacle(spawned,xOff,xOffRatio);
        }
    }

    private void moveObstacle(GameObject obj,float xOff,int xOffRatio)
    {
        float objWidth = obj.GetComponent<Collider>().bounds.size.x;
        obj.transform.position = new Vector3(xOff + xOffRatio *(objWidth), 
            ship.transform.position.y + objectSpawnYOffset, 
            animationTrigger.transform.position.z + 3);
        obj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -shipSpeed);
    }

    bool checkFloat(float f, List<float> used)
    {
        for(int i = 1; i < used.Count; i++) // check every element of the list
        {
            if (Mathf.Abs(used[i] - f) < used[0])
            {
                return false; // if any of them are closer to f than the width, false
            }
        }
        return true;
    }

    //initiliases the pool
    void initPool()
    {
        //initialise obstacles
        pool = new GameObject[objMap.Length, numberOfEachObject];
        pooltracker = new int[objMap.Length];
        for (int i = 0; i < objMap.Length; i++)
        {
            objMap[i].SetActive(false);
            pooltracker[i] = 0;
            for (int j = 0; j < numberOfEachObject; j++)
            {
                pool[i, j] = Instantiate(objMap[i], objMap[i].transform.position, Quaternion.identity) as GameObject;
            }
        }

        //initialise powerups
        poweruppool = new GameObject[powerups.Length, numberOfEachObject];
        poweruppooltracker = new int[powerups.Length];
        for (int i = 0; i < powerups.Length; i++)
        {
            powerups[i].SetActive(false);

            try
            {
                poweruppooltracker[i] = 0;
            }
            // Error occurred
            catch (System.Exception e)
            {
                Debug.Log("E: " + e);
                Debug.Log("powerups.Length = " + powerups.Length);
            }

            for (int j = 0; j < numberOfEachObject; j++)
            {
                poweruppool[i, j] = Instantiate(powerups[i], powerups[i].transform.position, Quaternion.identity) as GameObject;
            }
        }

    }
}