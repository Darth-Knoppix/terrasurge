using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayWallScript : MonoBehaviour {
    private List<GameObject> moving;
    public GameObject ship;
    private Main_alt shipScript;
	// Use this for initialization
	void Start () {
        shipScript = ship.GetComponent<Main_alt>();
	}
	
	// Update is called once per frame
	void Update () {
	}

    void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            //print(shipScript.ticks);
            //collision.gameObject.transform.position += Vector3.up * shipScript.objectSpawnYOffset;
        }
    }
}
