using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class MenuButton_Endless : MonoBehaviour {
    public void onClick()
    {
        DontDestroyOnLoad(this.gameObject);
        // Change Scene
        Debug.Log("Changing Game Scene");
        SceneManager.LoadScene("GameEndless");
    }
}
