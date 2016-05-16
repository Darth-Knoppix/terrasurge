using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/* Wobble the camera slowly and pause the game 
 * and show the pause menu when escape is hit */
public class PausedGame_Menu : MonoBehaviour {

	public Transform target;
	public float Speed = 1;

	private int counter = 0;
	private bool paused = false;
    private GameObject pausedCanvas;
    private GameObject gameoverCanvas;

    private float animationspeed;
    private Animator anim;

    private GameObject left_thruster;
    private GameObject center_thruster;
    private GameObject right_thruster;

    public AudioSource audio1;

    private float timescale;

    // Set up variables
    void Start()
    {
        //anim = target.GetComponent<Animator>();
        timescale = Time.timeScale;

        audio1 = target.GetComponent<Main>().getAudio();

        //right_thruster = GameObject.Find("Right Thuster");
        //center_thruster = GameObject.Find("Center Thruster");
        //left_thruster = GameObject.Find("Left Thruster");

        pausedCanvas = GameObject.Find("PausedGame_Canvas");
        pausedCanvas.SetActive(false);
    }

    void Update() {

        // Pause Game on Esc Key Down, Resume Once pressed again
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }
		
        // Wobble the Camera
		/*if (!paused) {
			if (counter < 20) {
			transform.position -= transform.right / Speed * Time.deltaTime;
			counter++;
			}
			else if (counter >= 20 && counter < 40) {
				transform.position += transform.right / Speed * Time.deltaTime;
				counter++;
			}
			else {
				counter = 0; 
			}
		}*/
	}

    // 
    public void PauseGame()
    {
        Debug.Log("Paused the Game");
        paused = true;
        Time.timeScale = 0;
        pausedCanvas.SetActive(true);
        //animationspeed = anim.speed;
        //anim.speed = 0;
        audio1.Pause();
        
    }

    //
    public void ResumeGame()
    {
        Debug.Log("Resumed the Game");
        paused = false;
        Time.timeScale = timescale;
        pausedCanvas.SetActive(false);
        //anim.speed = animationspeed;
        audio1.Play();
    }

    public void MainMenuResume()
    {
        Debug.Log("Resumed the Game");
        paused = false;
        Time.timeScale = timescale;
        pausedCanvas.SetActive(false);
        //anim.speed = animationspeed;

        // Change Scene
        Debug.Log("Changing Game Scene");

        //Application.LoadLevel("MainScene.unity");
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        // Quit game
        Debug.Log("Closing Game");
        Application.Quit();
    }

    public void GameOverRestartLevel()
    {
        // Restart Level
        Debug.Log("Restarting Game");
        Time.timeScale = timescale;

        gameoverCanvas = GameObject.Find("GameOver_Canvas");
        gameoverCanvas.SetActive(false);

        string scenename = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scenename);
    }

    public void PausedRestartLevel()
    {
        // Restart Level
        Debug.Log("Restarting Game");
        Time.timeScale = timescale;

        pausedCanvas.SetActive(false);

        string scenename = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scenename);
    }

}
