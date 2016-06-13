using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu_Button_Survival : MonoBehaviour {

    public void onClick()
    {
        DontDestroyOnLoad(this.gameObject);
        // Change Scene
        Debug.Log("Changing Game Scene");
        SceneManager.LoadScene("GameSurvival");
    }
}
