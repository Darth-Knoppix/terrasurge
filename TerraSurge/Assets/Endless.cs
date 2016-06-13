using UnityEngine;
using System.Collections;

public class Endless : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<GameObject>().GetComponent<Main>().setEndless();
	}
}
