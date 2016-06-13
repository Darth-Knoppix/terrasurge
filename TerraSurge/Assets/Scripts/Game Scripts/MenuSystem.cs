using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// Menu System Class for the game
public class MenuSystem : MonoBehaviour {

    // If this is true then there is a menu active
    // gameover or paused
    private bool menuSystemActive;

    private bool paused;
    private bool levelcomplete;
    private bool gameover;

    // Canvas's for the menus
    private GameObject pausedCanvas;
    private GameObject gameoverCanvas;
    private GameObject levelComplete_Canvas;
    private GameObject gameHUD_Canvas;

    private Text gameOverScoreText;
    private Text levelCompleteScoreText;
    private ScoreController scoreController;

    private float timescale;

    public AudioSource audio1;

    // Set up variables
    void Start()
    {
        menuSystemActive = false;
        levelcomplete = false;
        paused = false;
        gameover = false;

        timescale = Time.timeScale;
        scoreController = GameObject.Find("GameManager").GetComponent<ScoreController>();

        pausedCanvas = GameObject.Find("PausedGame_Canvas");
        pausedCanvas.SetActive(false);

        gameoverCanvas = GameObject.Find("GameOver_Canvas");
        gameOverScoreText = GameObject.Find("GameOverHighScore_Score").GetComponent<Text>();
        gameoverCanvas.SetActive(false);

        levelComplete_Canvas = GameObject.Find("LevelComplete_Canvas");
        levelCompleteScoreText = GameObject.Find("LevelHighScore_Score").GetComponent<Text>();
        levelComplete_Canvas.SetActive(false);

        gameHUD_Canvas = GameObject.Find("GameHUD_Canvas");
    }

    void Update() {
        // If it's not game over or level complete
        if (!gameover && !levelcomplete)
        {
            // Pause on Esc, Space, Return or P
            if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Return)
                || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.P))
            {
                if (!menuSystemActive)
                {
                    pauseGame();
                }
                else
                {
                    resumeGame();
                }
            }
        }
        else if (gameover)
        {
            // Play on Space and Return
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                gameOverRestartLevel();
            }
        }
        else if (levelcomplete)
        {
            // Play on Space and Return
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                nextLevel();
            }
        }
	}

    // Pause Game
    public void pauseGame()
    {
        Debug.Log("Paused the Game");
        menuSystemActive = true;
        paused = true;
        Time.timeScale = 0;
        gameHUD_Canvas.SetActive(false);
        pausedCanvas.SetActive(true);
        audio1.Pause();
    }

    // Resume Game
    public void resumeGame()
    {
        Debug.Log("Resumed the Game");
        menuSystemActive = false;
        paused = false;
        Time.timeScale = timescale;
        gameHUD_Canvas.SetActive(true);
        pausedCanvas.SetActive(false);
        audio1.Play();
    }

    public void mainMenuResume()
    {
        // Resume first
        resumeGame();

        // Change Scene
        Debug.Log("Changing Game Scene: MainMenu");

        //Application.LoadLevel("MainScene.unity");
        SceneManager.LoadScene("MainMenu");
    }

    public void quitGame()
    {
        // Quit game
        Debug.Log("Closing Game");
        Application.Quit();
    }

    public void gameOverRestartLevel()
    {
        // Resume first
        resumeGame();

        gameover = false;

        // Restart Level
        Debug.Log("Restarting Game by reloading scene");

        // Reload the Scene
        string scenename = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scenename);
    }

    public void pausedRestartLevel()
    {
        // Resume first
        resumeGame();

        // Restart Level
        Debug.Log("Restarting Game");

        // Reload the Scene
        string scenename = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(scenename);
    }

    public void gameOver()
    {
        Debug.Log("Game Over!");
        menuSystemActive = true;
        gameover = true;
        audio1.Pause();
        
        gameHUD_Canvas.SetActive(false);
        gameoverCanvas.SetActive(true);

        gameOverScoreText.text = ""+scoreController.getScore();
        gameOverScoreText.color = Color.yellow;

        // kill camera shake
        GameObject.Find("ShipCamera").GetComponent<CameraShake>().shakeDuration = 0f;

        // freeze time
        Time.timeScale = 0;
    }

    public void levelComplete()
    {
        Debug.Log("Level Complete!");
        menuSystemActive = true;
        levelcomplete = true;
        Time.timeScale = 0;
        gameHUD_Canvas.SetActive(false);
        levelComplete_Canvas.SetActive(true);
        audio1.Pause();

        levelCompleteScoreText.text = "" + scoreController.getScore();
        levelCompleteScoreText.color = Color.yellow;
    }

    public void nextLevel()
    {
        // Resume first
        resumeGame();

        levelcomplete = false;
        gameHUD_Canvas.SetActive(true);
        levelComplete_Canvas.SetActive(false);

        // required to fix timescale bug?
        audio1.Play();
        // Timescale bug fix
        Time.timeScale = timescale;

        // Load a scene
        string scenename = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("MainMenu");
    }

    public bool isActive()
    {
        {
            return menuSystemActive;
        }
    }
}
