using UnityEngine;
using System.Collections;

public class GroundObstacleController : MonoBehaviour {
	Animator animator;

	// Use this for initialization
	void Start () {
		animator = this.GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Q)){
			animator.SetBool("trig", !animator.GetBool("trig"));
		}
	}
}
