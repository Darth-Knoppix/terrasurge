using UnityEngine;
using System.Collections;

public class ScorePickupController : MonoBehaviour {
	public int scoreAmount = 500;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider c){
		if (c.gameObject.CompareTag ("Player")) {
			ScoreController score = FindObjectOfType<ScoreController>() as ScoreController;
			score.incrementScore (scoreAmount);
			Debug.Log ("Score += " + scoreAmount);
			Debug.Log ("Total: " + score.getScore());
			GameObject.Destroy (this.gameObject);
		}
	}
}
