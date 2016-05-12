using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Text.RegularExpressions;

public class Main : MonoBehaviour {
    public GameObject origin;
    public GameObject shiporigin;

    public Transform ship;
	public int timeOffsetMS;
	//array of objects used for loading/spawning obstacles
	public GameObject[] objMap;

	//audio source
	public AudioSource audio1;
	public TextAsset audio1Meta;
	//to be loaded from text file time(ms):obj
	private SortedDictionary<int,int> audio1Map;
	private int nextEntry;

	public GameObject[] terrainMap;
	public int terrainDuration;
	private int prevTerrain;
	public GameObject terrainOrigin;

    public int lives; //could easily be a hp bar as well...
    public int score; //score

    private int jumpCD; //cooldown on jump
	// Use this for initialization
	void Start ()
    {
		prevTerrain = 0;
		nextEntry = 0;
		timeOffsetMS = 3000;
		jumpCD = 0;
		audio1 = GetComponent<AudioSource> ();
		//loads first sound track
		loadAudio1 ();
        this.gameObject.transform.position = shiporigin.transform.position;
		audio1.Play ();
    }
	
	// Update is called once per frame
	void Update () {
		//pure random object spawn
        float spawnObjRN = UnityEngine.Random.Range(0, 200);
		float nextObjXOff = UnityEngine.Random.Range(-3F, 3F);
		float nextObjYOff = UnityEngine.Random.Range(-3F, 3F);
		//nextObjXOff = -3F;
		double playtime = audio1.time;
		//offset time here if needed
		int timeMS = (int)(playtime * 1000);//-3000;
		//print(audio1Map[playtime]);
		print(timeMS);
		KeyValuePair<int,int> next = audio1Map.ElementAt(nextEntry);
		if (timeMS > next.Key) {
			GameObject spawned = objMap [0];// [next.Value];
			GameObject obj = Instantiate(spawned, origin.transform.position + Vector3.up*nextObjXOff+Vector3.right*nextObjYOff,Quaternion.identity) as GameObject;
			obj.GetComponent<Rigidbody>().velocity = new Vector3(0,0,-10);
			nextEntry++;
		}
		print (prevTerrain);
		if (timeMS > terrainDuration * prevTerrain) {
			GameObject spawned = terrainMap [0];// [next.Value];
			GameObject obj = Instantiate (spawned, terrainOrigin.transform.position, Quaternion.identity) as GameObject;
			obj.GetComponent<Rigidbody> ().velocity = new Vector3 (0, 0, -10);
			prevTerrain++;
		}

		if ((Input.GetKey(KeyCode.D) || Input.GetButtonDown("B")) && (this.gameObject.transform.position.x-shiporigin.transform.position.x< 5))
        {
            ship.Translate(Vector3.right*0.5F);
        }
		if ((Input.GetKey(KeyCode.A) || Input.GetButtonDown("X")) && (shiporigin.transform.position.x - this.gameObject.transform.position.x < 5))
        {
            ship.Translate(Vector3.left * 0.5F);
        }
        if (Input.GetKey(KeyCode.W) && (this.gameObject.transform.position.z - shiporigin.transform.position.z < 5))
        {
            ship.Translate(Vector3.forward * 0.5F);
        }
        if (Input.GetKey(KeyCode.S) && (shiporigin.transform.position.z - this.gameObject.transform.position.z < 5))
        {
            ship.Translate(Vector3.back * 0.5F);
        }
        if (Input.GetKey(KeyCode.Space) && jumpCD == 0)
        {
            ship.GetComponent<Rigidbody>().AddForce(Vector3.up * 50000000);
            jumpCD = 300;
        }
        if(jumpCD>0)
        {
            jumpCD = jumpCD - 1;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Terrain")
        {
            return;
        }
        if (collision.gameObject.tag == "Good")
        {
            score++;
            print("score"+score);
			Destroy(collision.gameObject);
        }
        else {
            lives--;
            print("lives:" + lives);
			Destroy (collision.gameObject);
        }
    }

	void loadAudio1(){
		audio1Map = new SortedDictionary<int,int> ();
		audio1Map[0] = 2;
		audio1Map[497] = 2;
		audio1Map[1920] = 2;
		audio1Map[2417] = 2;
		audio1Map[3840] = 4;
		audio1Map[4337] = 4;
		audio1Map[5760] = 4;
		audio1Map[6257] = 4;
		audio1Map[7680] = 5;
		audio1Map[8177] = 5;
		audio1Map[9600] = 6;
		audio1Map[10097] = 6;
		audio1Map[11520] = 6;
		audio1Map[12017] = 6;
		audio1Map[13440] = 7;
		audio1Map[13937] = 7;
		audio1Map[15360] = 7;
		audio1Map[15857] = 7;
		audio1Map[17280] = 0;
		audio1Map[17777] = 0;
		audio1Map[19200] = 0;
		audio1Map[19697] = 0;
		audio1Map[21120] = 1;
		audio1Map[21617] = 1;
		audio1Map[23040] = 2;
		audio1Map[23537] = 2;
		audio1Map[24960] = 2;
		audio1Map[25457] = 2;
		audio1Map[26880] = 4;
		audio1Map[27377] = 4;
		audio1Map[28800] = 4;
		audio1Map[29297] = 4;
		audio1Map[30720] = 5;
		audio1Map[31217] = 5;
		audio1Map[32640] = 6;
		audio1Map[33137] = 6;
		audio1Map[34560] = 6;
		audio1Map[35057] = 6;
		audio1Map[36480] = 7;
		audio1Map[36977] = 7;
		audio1Map[38400] = 7;
		audio1Map[38897] = 7;
		audio1Map[40320] = 0;
		audio1Map[40817] = 0;
		audio1Map[42240] = 0;
		audio1Map[42737] = 0;
		audio1Map[44160] = 1;
		audio1Map[44657] = 1;
		audio1Map[46080] = 2;
		audio1Map[46577] = 2;
		audio1Map[49920] = 2;
		audio1Map[50417] = 2;
		audio1Map[50880] = 4;
		audio1Map[51377] = 4;
		audio1Map[51840] = 4;
		audio1Map[52337] = 4;
		audio1Map[52800] = 5;
		audio1Map[53297] = 5;
		audio1Map[53760] = 6;
		audio1Map[54257] = 6;
		audio1Map[54720] = 6;
		audio1Map[55217] = 6;
		audio1Map[55680] = 7;
		audio1Map[56177] = 7;
		audio1Map[56640] = 7;
		audio1Map[57137] = 7;
		audio1Map[57600] = 0;
		audio1Map[58097] = 0;
		audio1Map[58560] = 0;
		audio1Map[59057] = 0;
		audio1Map[59520] = 1;
		audio1Map[60017] = 1;
		audio1Map[60480] = 2;
		audio1Map[60977] = 2;
		audio1Map[61440] = 2;
		audio1Map[61937] = 2;
		audio1Map[62400] = 4;
		audio1Map[62897] = 4;
		audio1Map[63360] = 4;
		audio1Map[63857] = 4;
		audio1Map[64320] = 5;
		audio1Map[64817] = 5;
		audio1Map[65280] = 6;
		audio1Map[65777] = 6;
		audio1Map[66240] = 6;
		audio1Map[66737] = 6;
		audio1Map[67200] = 7;
		audio1Map[67697] = 7;
		audio1Map[68160] = 7;
		audio1Map[68657] = 7;
		audio1Map[69120] = 0;
		audio1Map[69617] = 0;
		audio1Map[70080] = 0;
		audio1Map[70577] = 0;
		audio1Map[71040] = 1;
		audio1Map[71537] = 1;
		audio1Map[72000] = 2;
		audio1Map[72497] = 2;
		audio1Map[72960] = 2;
		audio1Map[73457] = 2;
		audio1Map[73920] = 4;
		audio1Map[74417] = 4;
		audio1Map[74880] = 4;
		audio1Map[75377] = 4;
		audio1Map[75840] = 5;
		audio1Map[76337] = 5;
		audio1Map[76800] = 6;
		audio1Map[77297] = 6;
		audio1Map[77760] = 6;
		audio1Map[78257] = 6;
		audio1Map[78720] = 7;
		audio1Map[79217] = 7;
		audio1Map[79680] = 7;
		audio1Map[80177] = 7;

	}
}
