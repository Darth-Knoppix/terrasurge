using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScoreBoard_Display : MonoBehaviour {

    public GameObject score1;
    public GameObject score2;
    public GameObject score3;
    public GameObject score4;
    public GameObject score5;

    private bool scoresShown;

    // Use this for initialization
    void Start () {
        score1.SetActive(false);
        score2.SetActive(false);
        score3.SetActive(false);
        score4.SetActive(false);
        score5.SetActive(false);

        scoresShown = false;
    }
	
	// Update is called once per frame
	void Update () {

	}

    public void toggleHighScores ()
    {
        if (scoresShown)
        {
            score1.SetActive(false);
            score2.SetActive(false);
            score3.SetActive(false);
            score4.SetActive(false);
            score5.SetActive(false);

            scoresShown = false;
        }
        else
        {
            score1.SetActive(true);
            score2.SetActive(true);
            score3.SetActive(true);
            score4.SetActive(true);
            score5.SetActive(true);

            scoresShown = true;
        }
    }
}
