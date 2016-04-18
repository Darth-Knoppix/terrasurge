using UnityEngine;
using System.Collections;

public class cam : MonoBehaviour {

	public Transform myCamera;
	public Transform target;
	// Use this for initialization
	void Start () {
		myCamera = GetComponent<Transform>();
		target = GetComponent<Transform> ();
	}
	
	// Update is called once per frame
	void Update () {
		print (target.position);
		print (myCamera.position);
		myCamera.position = target.position;// + new Vector3 (0, 10, 0);
	}
}
