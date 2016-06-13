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

    private int obstacleSpawnRate;
    void Start()
    {
        beatObserver = GetComponent<BeatObserver>();
        obstacleSpawnRate = 1000;
        initPool();
    }

    void Update()
    {
        // OBSTACLE SPAWNING
        if ((beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat)
        {
            spawnObject(0);
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
            spawnObject(1);
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
        print(obj.transform.position);
    }

    void spawnObject(int type)
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
            moveObstacle(spawned);
        }
    }

    private void moveObstacle(GameObject obj)
    {
        obj.transform.position = new Vector3(ship.transform.position.x+UnityEngine.Random.Range(-5,5), 
            ship.transform.position.y + objectSpawnYOffset, 
            animationTrigger.transform.position.z + 3);
        obj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -shipSpeed);
        print(obj.transform.position);
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