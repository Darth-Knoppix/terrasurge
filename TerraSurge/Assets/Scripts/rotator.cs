using UnityEngine;
using System.Collections;

public class rotator : MonoBehaviour {
	public float degreesPerSecond = 50.0f;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update() {
		transform.Rotate(Vector3.up * degreesPerSecond * Time.deltaTime, Space.Self);
	}
}
