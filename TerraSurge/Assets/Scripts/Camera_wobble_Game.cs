using UnityEngine;
using System.Collections;

/* Wobble the camera slowly and pause the game 
 * and show the pause menu when escape is hit */
public class Camera_wobble_Game : MonoBehaviour {

	public Transform target;
	public float Speed = 1;

	private int counter = 0;
	private bool paused = false;
    private GameObject canvas;
    private float animationspeed;
    private Animator anim;

    private GameObject left_thruster;
    private GameObject center_thruster;
    private GameObject right_thruster;

    private float timescale;

    // Set up variables
    void Start()
    {
        anim = target.GetComponent<Animator>();
        timescale = Time.timeScale;

        right_thruster = GameObject.Find("Right Thuster");
        center_thruster = GameObject.Find("Center Thruster");
        left_thruster = GameObject.Find("Left Thruster");
        
        canvas = GameObject.Find("PausedGame_Canvas");
        canvas.SetActive(false);
    }

    void Update() {

        // Pause Game on Esc Key Down, Resume Once pressed again
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!paused)
            {
                Debug.Log("Paused the Game");
                paused = true;
                Time.timeScale = 0;
                canvas.SetActive(true);
                animationspeed = anim.speed;
                anim.speed = 0;
            }
            else
            {
                Debug.Log("Resumed the Game");
                paused = false;
                Time.timeScale = timescale;
                canvas.SetActive(false);
                anim.speed = animationspeed;
            }
        }
		
        // Wobble the Camera
		if (!paused) {
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
		}
	}
}
