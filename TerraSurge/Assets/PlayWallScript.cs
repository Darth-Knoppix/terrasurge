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
        moving = new List<GameObject>();
	}
	
	// Update is called once per frame
	void Update () {
       foreach(GameObject obj in moving)
        {
            if (obj != null)
            {
                obj.transform.position += new Vector3(0f, 0f, -shipScript.shipSpeed) * Time.deltaTime;
            }
           // else obj.
        }
	}

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Obstacle")
        {
            print(shipScript.ticks);
            collision.gameObject.transform.position += Vector3.up * shipScript.objectSpawnYOffset;
            moving.Add(collision.gameObject);
        }
    }
}
