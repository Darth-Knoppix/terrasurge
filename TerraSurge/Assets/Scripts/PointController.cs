using UnityEngine;
using System.Collections;

public class PointController : MonoBehaviour {
	public int scoreAmount = 500;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider c){
		if (c.gameObject.CompareTag ("Player")) {
			Main player = c.gameObject.GetComponent<Main>() as Main;
			player.incrementScore (scoreAmount);
			GameObject.Destroy (this.gameObject);
		}
	}
}
