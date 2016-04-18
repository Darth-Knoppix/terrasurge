using UnityEngine;
using System.Collections;

public class move : MonoBehaviour {
	public Transform ship;
	private Vector3 forwardVec;
	// Use this for initialization
	void Start () {
		ship = GetComponent<Transform>();
		forwardVec = Vector3.forward;
	}
	
	// Update is called once per frame
	void Update () {
		ship.Translate(Vector3.forward);
		if(Input.GetKey(KeyCode.Q)){
			ship.Translate(Vector3.left*3);
		}
		if(Input.GetKey(KeyCode.E)){
			ship.Translate(Vector3.right*3);
		}
		if(Input.GetKey(KeyCode.A)){
			ship.Rotate(Vector3.down);
		}
		if(Input.GetKey(KeyCode.D)){
			ship.Rotate(Vector3.up);
		}
		if(Input.GetKey(KeyCode.W)){
			ship.Translate(Vector3.up);
		}
		if(Input.GetKey(KeyCode.S)){
			ship.Translate(Vector3.down);
		}
//		if(Input.GetKey(KeyCode.LeftShift)){
//			forwardVec = forwardVec * 1.1;
//		}
//		if(Input.GetKey(KeyCode.LeftControl)){
//			forwardVec = forwardVec * 0.9;
//		}
	}
}
