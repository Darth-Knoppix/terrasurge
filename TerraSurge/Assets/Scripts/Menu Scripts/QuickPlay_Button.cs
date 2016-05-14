using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class QuickPlay_Button : MonoBehaviour {

	public void onClick(){
	    // Change Scene
	    Debug.Log("Changing Game Scene");
        
        //Application.LoadLevel("MainScene.unity");
        SceneManager.LoadScene("MainScene");
    }
}

