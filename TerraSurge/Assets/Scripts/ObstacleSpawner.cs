using UnityEngine;
using System.Collections;
using System;

public class ObstacleSpawner : MonoBehaviour {

    public GameObject[] obstaclesToSpawn; 	// What are we spawning
    public GameObject[] powerUpsToSpawn;    // What are we spawning
    public GameObject ship; //the ship
    public float gameSpeed;				// How fast are we moving it toward the player
	public float spawnTimer;			// How long until the next one spawns
	public float spawnHeight;			// The Ship start point
	public float xRange;				// Range in which obstacles will spawn

    public int obstacleSpawnRate;
    public int powerupSpawnRate;

	// Use this for initialization
	void Start () {
		// Make positive
		xRange 		= Mathf.Abs (xRange);
		gameSpeed 	= Mathf.Abs (gameSpeed);
		spawnTimer 	= Mathf.Abs (spawnTimer);
    }

    // Update is called once per frame
    void Update () {
        int whichObstacle2spawn = UnityEngine.Random.Range(0, obstaclesToSpawn.Length);
        int doISpawnObstacle = UnityEngine.Random.Range(0, 1000);
        int whichPowerup2spawn = UnityEngine.Random.Range(0, powerUpsToSpawn.Length);
        int doISpawnPowerup = UnityEngine.Random.Range(0, 1000);
        //if (Time.frameCount % spawnTimer == 0 && Time.timeScale > 0) {

        if (doISpawnObstacle < obstacleSpawnRate)
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
                Debug.Log("obstaclesToSpawn.Count: " + obstaclesToSpawn.Length);
            }
        }
        if (doISpawnPowerup < powerupSpawnRate)
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
                Debug.Log("whichPowerup2spawn.Count: " + powerUpsToSpawn.Length);
            }
        }
		}

    private void spawnAndMove(GameObject obj)
    {
        Vector3 spawnPoint = new Vector3(ship.transform.position.x,this.transform.position.y, this.transform.position.z) + Vector3.left * UnityEngine.Random.Range(-5, 5);

        GameObject newObj = Instantiate(obj, spawnPoint, Quaternion.identity) as GameObject;

        newObj.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, -gameSpeed);
    }
}

