using UnityEngine;
using System.Collections;

/* Wobble the camera slowly */
public class Camera_wobble_Menu : MonoBehaviour {
	public Transform target;
	public float Speed = 1;
	private int counter = 0;

	void Update() { 
		
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

