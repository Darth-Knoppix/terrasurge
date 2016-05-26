using UnityEngine;
using System.Collections;

public class ObstacleSpawner : MonoBehaviour {
	public GameObject obstacleToSpawn; 	// What are we spawning
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
	}
	
	// Update is called once per frame
	void Update () {
		if (Time.frameCount % spawnTimer == 0) {
			GameObject obj = obstacleToSpawn;
			Vector3 spawnPoint = new Vector3 (UnityEngine.Random.Range (-xRange, xRange), spawnHeight, spawnTimer * Time.deltaTime * gameSpeed);

			GameObject newObj = Instantiate (obstacleToSpawn, spawnPoint, Quaternion.identity) as GameObject;

			newObj.GetComponent<Rigidbody> ().velocity = new Vector3 (0f, 0f, -gameSpeed);
		}
	}
}
