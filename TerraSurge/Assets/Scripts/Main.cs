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

	//pools of shit
	private GameObject[,] pool;
	private int [] pooltracker;
	public int numberOfEachObject;

    //for music calculations
    int ppqn = 480;
    int tempo = 260;

    private int jumpCD; //cooldown on jump

    // GameOver canvas
    private GameObject canvas;

    private float timescale;

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
		initPool ();

        //Setup GameOver Canvas
        canvas = GameObject.Find("GameOver_Canvas");
        canvas.SetActive(false);
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
        //avoid integer overflow
        float ratio = ppqn * tempo / 60000;
        int ticks = (int)(timeMS * ratio);
		//print(audio1Map[playtime]);
		//print(timeMS);
		KeyValuePair<int,int> next = audio1Map.ElementAt(nextEntry);
        //print(nextEntry);
        print("timems" + timeMS);
        print("ppqn" + ppqn);
        print("tempo" + tempo);
        print("ticks" + ticks);
        print("nexttick" + next.Key);
        if (ticks > next.Key) {
            //print(next);
            //print("nextentry"+nextEntry);
            GameObject spawned = pool [next.Value,pooltracker[next.Value]];// [next.Value];
			pooltracker[next.Value]++;
			if (pooltracker [next.Value] >= numberOfEachObject) {
				pooltracker [next.Value] = 0;
			}
			spawned.SetActive(true);
			spawned.transform.position = origin.transform.position + Vector3.up*nextObjXOff+Vector3.right*nextObjYOff;
			spawned.GetComponent<Rigidbody>().velocity = new Vector3(0,0,-10);
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
            print("lives:" + lives); // insert check here
			if (lives <= 0) {
                GameOver();
			}
			Destroy (collision.gameObject);
        }
    }

	void initPool(){
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
        timescale = Time.timeScale;
        Time.timeScale = 0;
        canvas.SetActive(true);
    }

	void loadAudio1(){
		audio1Map = new SortedDictionary<int,int> ();
        audio1Map[0] = 2;
        audio1Map[581] = 2;
        audio1Map[3840] = 2;
        audio1Map[4075] = 2;
        audio1Map[7680] = 2;
        audio1Map[8261] = 2;
        audio1Map[11520] = 2;
        audio1Map[11755] = 2;
        audio1Map[15360] = 2;
        audio1Map[15941] = 2;
        audio1Map[19200] = 2;
        audio1Map[19435] = 2;
        audio1Map[23040] = 2;
        audio1Map[23621] = 2;
        audio1Map[26880] = 2;
        audio1Map[27115] = 2;
        audio1Map[30720] = 2;
        audio1Map[31301] = 2;
        audio1Map[34560] = 2;
        audio1Map[34795] = 2;
        audio1Map[38400] = 2;
        audio1Map[38981] = 2;
        audio1Map[42240] = 2;
        audio1Map[42475] = 2;
        audio1Map[46080] = 2;
        audio1Map[46661] = 2;
        audio1Map[49920] = 2;
        audio1Map[50155] = 2;
        audio1Map[53760] = 2;
        audio1Map[54341] = 2;
        audio1Map[57600] = 2;
        audio1Map[57835] = 2;
        audio1Map[61440] = 2;
        audio1Map[62021] = 2;
        audio1Map[65280] = 2;
        audio1Map[65515] = 2;
        audio1Map[69120] = 2;
        audio1Map[69701] = 2;
        audio1Map[72960] = 2;
        audio1Map[73195] = 2;
        audio1Map[107520] = 2;
        audio1Map[108000] = 3;
        audio1Map[108101] = 2;
        audio1Map[108641] = 3;
        audio1Map[109440] = 3;
        audio1Map[109680] = 3;
        audio1Map[110080] = 3;
        audio1Map[110320] = 3;
        audio1Map[110880] = 3;
        audio1Map[111120] = 3;
        audio1Map[111360] = 2;
        audio1Map[111360] = 3;
        audio1Map[111360] = 3;
        audio1Map[111595] = 2;
        audio1Map[111840] = 3;
        audio1Map[112480] = 3;
        audio1Map[113280] = 3;
        audio1Map[113520] = 3;
        audio1Map[113920] = 3;
        audio1Map[114160] = 3;
        audio1Map[114720] = 3;
        audio1Map[114960] = 3;
        audio1Map[115200] = 2;
        audio1Map[115200] = 3;
        audio1Map[115200] = 3;
        audio1Map[115680] = 3;
        audio1Map[115781] = 2;
        audio1Map[116321] = 3;
        audio1Map[117120] = 3;
        audio1Map[117360] = 3;
        audio1Map[117760] = 3;
        audio1Map[118000] = 3;
        audio1Map[118560] = 3;
        audio1Map[118800] = 3;
        audio1Map[119040] = 2;
        audio1Map[119040] = 3;
        audio1Map[119040] = 3;
        audio1Map[119275] = 2;
        audio1Map[119520] = 3;
        audio1Map[120160] = 3;
        audio1Map[120960] = 3;
        audio1Map[121200] = 3;
        audio1Map[121600] = 3;
        audio1Map[121840] = 3;
        audio1Map[122400] = 3;
        audio1Map[122640] = 3;
        audio1Map[122880] = 2;
        audio1Map[122880] = 3;
        audio1Map[122880] = 3;
        audio1Map[123360] = 3;
        audio1Map[123461] = 2;
        audio1Map[124001] = 3;
        audio1Map[124800] = 3;
        audio1Map[125040] = 3;
        audio1Map[125440] = 3;
        audio1Map[125680] = 3;
        audio1Map[126240] = 3;
        audio1Map[126480] = 3;
        audio1Map[126720] = 2;
        audio1Map[126720] = 3;
        audio1Map[126720] = 3;
        audio1Map[126955] = 2;
        audio1Map[127200] = 3;
        audio1Map[127840] = 3;
        audio1Map[128640] = 3;
        audio1Map[128880] = 3;
        audio1Map[129280] = 3;
        audio1Map[129520] = 3;
        audio1Map[130080] = 3;
        audio1Map[130320] = 3;
        audio1Map[130560] = 2;
        audio1Map[130560] = 3;
        audio1Map[130560] = 3;
        audio1Map[131040] = 3;
        audio1Map[131141] = 2;
        audio1Map[131681] = 3;
        audio1Map[132480] = 3;
        audio1Map[132720] = 3;
        audio1Map[133120] = 3;
        audio1Map[133360] = 3;
        audio1Map[133920] = 3;
        audio1Map[134160] = 3;
        audio1Map[134400] = 2;
        audio1Map[134400] = 3;
        audio1Map[134400] = 3;
        audio1Map[134635] = 2;
        audio1Map[134880] = 3;
        audio1Map[135520] = 3;
        audio1Map[136320] = 3;
        audio1Map[136560] = 3;
        audio1Map[136960] = 3;
        audio1Map[137200] = 3;
        audio1Map[137760] = 3;
        audio1Map[138000] = 3;
        audio1Map[138240] = 2;
        audio1Map[138240] = 3;
        audio1Map[138240] = 3;
        audio1Map[138720] = 3;
        audio1Map[138821] = 2;
        audio1Map[139361] = 3;
        audio1Map[140160] = 3;
        audio1Map[140400] = 3;
        audio1Map[140800] = 3;
        audio1Map[141040] = 3;
        audio1Map[141600] = 3;
        audio1Map[141840] = 3;
        audio1Map[142080] = 2;
        audio1Map[142080] = 3;
        audio1Map[142080] = 3;
        audio1Map[142315] = 2;
        audio1Map[142560] = 3;
        audio1Map[143200] = 3;
        audio1Map[144000] = 3;
        audio1Map[144240] = 3;
        audio1Map[144640] = 3;
        audio1Map[144880] = 3;
        audio1Map[145440] = 3;
        audio1Map[145680] = 3;
        audio1Map[145920] = 2;
        audio1Map[145920] = 3;
        audio1Map[145920] = 3;
        audio1Map[146400] = 3;
        audio1Map[146501] = 2;
        audio1Map[147041] = 3;
        audio1Map[147840] = 3;
        audio1Map[148080] = 3;
        audio1Map[148480] = 3;
        audio1Map[148720] = 3;
        audio1Map[149280] = 3;
        audio1Map[149520] = 3;
        audio1Map[149760] = 2;
        audio1Map[149760] = 3;
        audio1Map[149760] = 3;
        audio1Map[149995] = 2;
        audio1Map[150240] = 3;
        audio1Map[150880] = 3;
        audio1Map[151680] = 3;
        audio1Map[151920] = 3;
        audio1Map[152320] = 3;
        audio1Map[152560] = 3;
        audio1Map[153120] = 3;
        audio1Map[153360] = 3;
        audio1Map[153600] = 2;
        audio1Map[153600] = 3;
        audio1Map[153600] = 3;
        audio1Map[154080] = 3;
        audio1Map[154181] = 2;
        audio1Map[154721] = 3;
        audio1Map[155520] = 3;
        audio1Map[155760] = 3;
        audio1Map[156160] = 3;
        audio1Map[156400] = 3;
        audio1Map[156960] = 3;
        audio1Map[157200] = 3;
        audio1Map[157440] = 2;
        audio1Map[157440] = 3;
        audio1Map[157440] = 3;
        audio1Map[157675] = 2;
        audio1Map[157920] = 3;
        audio1Map[158560] = 3;
        audio1Map[159360] = 3;
        audio1Map[159600] = 3;
        audio1Map[160000] = 3;
        audio1Map[160240] = 3;
        audio1Map[160800] = 3;
        audio1Map[161040] = 3;
        audio1Map[161280] = 2;
        audio1Map[161280] = 3;
        audio1Map[161280] = 3;
        audio1Map[161760] = 3;
        audio1Map[161861] = 2;
        audio1Map[162401] = 3;
        audio1Map[163200] = 3;
        audio1Map[163440] = 3;
        audio1Map[163840] = 3;
        audio1Map[164080] = 3;
        audio1Map[164640] = 3;
        audio1Map[164880] = 3;
        audio1Map[165120] = 2;
        audio1Map[165120] = 3;
        audio1Map[165120] = 3;
        audio1Map[165355] = 2;
        audio1Map[165600] = 3;
        audio1Map[166240] = 3;
        audio1Map[167040] = 3;
        audio1Map[167280] = 3;
        audio1Map[167680] = 3;
        audio1Map[167920] = 3;
        audio1Map[168480] = 3;
        audio1Map[168720] = 3;
        audio1Map[168960] = 2;
        audio1Map[168960] = 3;
        audio1Map[168960] = 3;
        audio1Map[169541] = 2;
        audio1Map[172800] = 2;
        audio1Map[173035] = 2;
        audio1Map[176640] = 2;
        audio1Map[177221] = 2;
        audio1Map[180480] = 2;
        audio1Map[180715] = 2;
        audio1Map[184320] = 2;
        audio1Map[184800] = 3;
        audio1Map[184901] = 2;
        audio1Map[185441] = 3;
        audio1Map[186240] = 3;
        audio1Map[186480] = 3;
        audio1Map[186880] = 3;
        audio1Map[187120] = 3;
        audio1Map[187680] = 3;
        audio1Map[187920] = 3;
        audio1Map[188160] = 2;
        audio1Map[188160] = 3;
        audio1Map[188160] = 3;
        audio1Map[188395] = 2;
        audio1Map[188640] = 3;
        audio1Map[189280] = 3;
        audio1Map[190080] = 3;
        audio1Map[190320] = 3;
        audio1Map[190720] = 3;
        audio1Map[190960] = 3;
        audio1Map[191520] = 3;
        audio1Map[191760] = 3;
        audio1Map[192000] = 2;
        audio1Map[192000] = 3;
        audio1Map[192000] = 3;
        audio1Map[192480] = 3;
        audio1Map[192581] = 2;
        audio1Map[193121] = 3;
        audio1Map[193920] = 3;
        audio1Map[194160] = 3;
        audio1Map[194560] = 3;
        audio1Map[194800] = 3;
        audio1Map[195360] = 3;
        audio1Map[195600] = 3;
        audio1Map[195840] = 2;
        audio1Map[195840] = 3;
        audio1Map[195840] = 3;
        audio1Map[196075] = 2;
        audio1Map[196320] = 3;
        audio1Map[196960] = 3;
        audio1Map[197760] = 3;
        audio1Map[198000] = 3;
        audio1Map[198400] = 3;
        audio1Map[198640] = 3;
        audio1Map[199200] = 3;
        audio1Map[199440] = 3;
        audio1Map[199680] = 2;
        audio1Map[199680] = 3;
        audio1Map[199680] = 3;
        audio1Map[200160] = 3;
        audio1Map[200261] = 2;
        audio1Map[200801] = 3;
        audio1Map[201600] = 3;
        audio1Map[201840] = 3;
        audio1Map[202240] = 3;
        audio1Map[202480] = 3;
        audio1Map[203040] = 3;
        audio1Map[203280] = 3;
        audio1Map[203520] = 2;
        audio1Map[203520] = 3;
        audio1Map[203520] = 3;
        audio1Map[203755] = 2;
        audio1Map[204000] = 3;
        audio1Map[204640] = 3;
        audio1Map[205440] = 3;
        audio1Map[205680] = 3;
        audio1Map[206080] = 3;
        audio1Map[206320] = 3;
        audio1Map[206880] = 3;
        audio1Map[207120] = 3;
        audio1Map[207360] = 2;
        audio1Map[207360] = 3;
        audio1Map[207360] = 3;
        audio1Map[207840] = 3;
        audio1Map[207941] = 2;
        audio1Map[208481] = 3;
        audio1Map[209280] = 3;
        audio1Map[209520] = 3;
        audio1Map[209920] = 3;
        audio1Map[210160] = 3;
        audio1Map[210720] = 3;
        audio1Map[210960] = 3;
        audio1Map[211200] = 2;
        audio1Map[211200] = 3;
        audio1Map[211200] = 3;
        audio1Map[211435] = 2;
        audio1Map[211680] = 3;
        audio1Map[212320] = 3;
        audio1Map[213120] = 3;
        audio1Map[213360] = 3;
        audio1Map[213760] = 3;
        audio1Map[214000] = 3;
        audio1Map[214560] = 3;
        audio1Map[214800] = 3;
        audio1Map[215040] = 2;
        audio1Map[215040] = 3;
        audio1Map[215040] = 3;
        audio1Map[215520] = 3;
        audio1Map[215621] = 2;
        audio1Map[216161] = 3;
        audio1Map[216960] = 3;
        audio1Map[217200] = 3;
        audio1Map[217600] = 3;
        audio1Map[217840] = 3;
        audio1Map[218400] = 3;
        audio1Map[218640] = 3;
        audio1Map[218880] = 2;
        audio1Map[218880] = 3;
        audio1Map[218880] = 3;
        audio1Map[219115] = 2;
        audio1Map[219360] = 3;
        audio1Map[220000] = 3;
        audio1Map[220800] = 3;
        audio1Map[221040] = 3;
        audio1Map[221440] = 3;
        audio1Map[221680] = 3;
        audio1Map[222240] = 3;
        audio1Map[222480] = 3;
        audio1Map[222720] = 2;
        audio1Map[222720] = 3;
        audio1Map[222720] = 3;
        audio1Map[223200] = 3;
        audio1Map[223301] = 2;
        audio1Map[223841] = 3;
        audio1Map[224640] = 3;
        audio1Map[224880] = 3;
        audio1Map[225280] = 3;
        audio1Map[225520] = 3;
        audio1Map[226080] = 3;
        audio1Map[226320] = 3;
        audio1Map[226560] = 2;
        audio1Map[226560] = 3;
        audio1Map[226560] = 3;
        audio1Map[226795] = 2;
        audio1Map[227040] = 3;
        audio1Map[227680] = 3;
        audio1Map[228480] = 3;
        audio1Map[228720] = 3;
        audio1Map[229120] = 3;
        audio1Map[229360] = 3;
        audio1Map[229920] = 3;
        audio1Map[230160] = 3;
        audio1Map[230400] = 3;
        audio1Map[230400] = 3;
        audio1Map[230880] = 3;
        audio1Map[231520] = 3;

    }
}
