using UnityEngine;
using System.Collections;

public class endwallscript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c){
		Debug.Log (c);
		GameObject.Destroy (c.gameObject);
	}
}
