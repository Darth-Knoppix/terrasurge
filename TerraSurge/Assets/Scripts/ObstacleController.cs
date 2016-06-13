using UnityEngine;
using System.Collections;

public class ObstacleController : MonoBehaviour {
	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider collider){
		if (collider.gameObject.name == "PlayWall") {
			this.GetComponentsInChildren<Animator> () [0].SetTrigger ("Animate");
		}

        if (collider.gameObject.name == "Ship" || collider.gameObject.name == "Backdrop")
        {
			this.GetComponentsInChildren<Animator> () [0].ResetTrigger ("Animate");
            this.gameObject.SetActive(false);
        }
    }

	void OnDisable (){
		this.GetComponentsInChildren<Animator> () [0].ResetTrigger ("Animate");
	}
    

}
