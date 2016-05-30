using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBoard_Display : MonoBehaviour {
	private Text currentScore;
	private ScoreController scoreboard;

	// Use this for initialization
	void Start () {
		currentScore = GameObject.Find ("HighScore_Value").GetComponent<Text> ();
		scoreboard = GameObject.Find("GameManager").GetComponent<ScoreController> ();
	}
	
	// Update is called once per frame
	void Update () {
		currentScore.text = scoreboard.getScoref ();
	}
}
