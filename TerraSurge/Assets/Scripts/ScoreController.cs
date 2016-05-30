using UnityEngine;
using System.Collections;
using System.IO;

public class ScoreController : MonoBehaviour {
	private int score;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void incrementScore(int amount){
		Debug.Log ("Increment");
		this.score += Mathf.Abs(amount);

	}

	public int getScore(){
		Debug.Log ("Score is " + this.score);
		return this.score;
	}
}
