using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Exit_Button : MonoBehaviour {

	public void onClick(){
	  // Close game
	  Debug.Log("Closing Game");
	  Application.Quit();
	}
}