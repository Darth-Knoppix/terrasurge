using UnityEngine;
using System.Collections;
using System;

public class ObstacleSpawner : MonoBehaviour {

    public System.Collections.Generic.List<GameObject> obstaclesToSpawn = 
        new System.Collections.Generic.List<GameObject>(); 	// What are we spawning
    public System.Collections.Generic.List<GameObject> powerUpsToSpawn = 
        new System.Collections.Generic.List<GameObject>();    // What are we spawning

    public float gameSpeed;				// How fast are we moving it toward the player
	public float spawnTimer;			// How long until the next one spawns
	public float spawnHeight;			// The Ship start point
	public float xRange;				// Range in which obstacles will spawn

	// Use this for initialization
	void Start () {
		// Make positive
		xRange 		= Mathf.Abs (xRange);
		gameSpeed 	= Mathf.Abs (gameSpeed);
		spawnTimer 	= Mathf.Abs (spawnTimer);

        // Add to obstaclesToSpawn
        obstaclesToSpawn.Add(GameObject.Find("Shiv-tron-block"));

        // Add to powerUpsToSpawn here!
        powerUpsToSpawn.Add(GameObject.Find("PointBundle"));
    }

    // Update is called once per frame
    void Update () {
        int whichObstacle2spawn = UnityEngine.Random.Range(0, obstaclesToSpawn.Count);
        int doISpawnObstacle = UnityEngine.Random.Range(0, 1000);
        int whichPowerup2spawn = UnityEngine.Random.Range(0, powerUpsToSpawn.Count);
        int doISpawnPowerup = UnityEngine.Random.Range(0, 1000);
        //if (Time.frameCount % spawnTimer == 0 && Time.timeScale > 0) {

        if (doISpawnObstacle < 5)
        {
            // Try and spawn an obstacle
            try
            {
                spawnAndMove(obstaclesToSpawn[whichObstacle2spawn]);
            }
            // Error occurred
            catch (Exception e)
            {
                Debug.Log("E: " + e);
                Debug.Log("obstaclesToSpawn.Count: " + obstaclesToSpawn.Count);
            }
        }
        if (doISpawnPowerup < 5)
        {
            // Try and spawn a powerup
            try
            {
                spawnAndMove(powerUpsToSpawn[whichPowerup2spawn]);
            }
            // Error occurred
            catch (Exception e)
            {
                Debug.Log("E: " + e);
                Debug.Log("whichPowerup2spawn.Count: " + powerUpsToSpawn.Count);
            }
        }
		}

    private void spawnAndMove(GameObject obj)
    {
        Vector3 spawnPoint = new Vector3(UnityEngine.Random.Range(-xRange, xRange), spawnHeight, spawnTimer * Time.deltaTime * gameSpeed);

        GameObject newObj = Instantiate(obj, spawnPoint, Quaternion.identity) as GameObject;

        newObj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -gameSpeed);
    }
}

