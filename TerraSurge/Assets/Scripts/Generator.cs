using UnityEngine;
using System.Collections;
using SynchronizerData;
using System.Collections.Generic;

public class Generator : MonoBehaviour
{
    private BeatObserver beatObserver;
    // pools of objects. These are initialised on startup
    // then future objects are not instantiated, but instead loaded
    // from the pool to prevent lag
    // obstacles
    private GameObject[,] pool;
    // used to track index of latest used object from the pool
    private int[] pooltracker;
    // maximum number of each object in the pool
    public int numberOfEachObject;
    //array of objects used for loading/spawning obstacles
    public GameObject[] objMap;
    // wall that triggers anims
    public GameObject animationTrigger;

    // distance in front of player objects pop at
    public double setTimeInFrontOfPlayer;

    public int shipSpeed;
    public int objectSpawnYOffset;
    void Start()
    {
        beatObserver = GetComponent<BeatObserver>();
        initPool();
    }

    void Update()
    {
        if ((beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat)
        {
            spawnObject(0);
        }
        if ((beatObserver.beatMask & BeatType.UpBeat) == BeatType.UpBeat)
        {
            //spawnObject(1); THIS IS FUCKED UP
        }
        if ((beatObserver.beatMask & BeatType.OffBeat) == BeatType.OffBeat)
        {
            spawnObject(2);
        }
        if ((beatObserver.beatMask & BeatType.OnBeat) == BeatType.OnBeat)
        {
            spawnObject(3);
        }


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
            //replace tracer with object
            if (spawned != null)
            {
                spawned.SetActive(true);
                spawnAndMove(spawned);
            }
    }

    private void spawnAndMove(GameObject obj)
    {
        Vector3 spawnPoint = new Vector3(this.transform.position.x, this.transform.position.y - objectSpawnYOffset,
            animationTrigger.transform.position.z + (int)(setTimeInFrontOfPlayer * shipSpeed)) + Vector3.left * UnityEngine.Random.Range(-10, 10);
        obj.transform.position = spawnPoint;
        obj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -shipSpeed);
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

    }
}