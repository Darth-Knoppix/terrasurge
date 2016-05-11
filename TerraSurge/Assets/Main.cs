using UnityEngine;
using System.Collections;

public class Main : MonoBehaviour
{
    public GameObject cubeObst;
    public GameObject cylObst;
    public GameObject capObst;
    public GameObject powerUp;

    public GameObject origin;
    public GameObject shiporigin;

    public Transform ship;


    public int lives; //could easily be a hp bar as well...
    public int score; //score

    private int jumpCD; //cooldown on jump
                        // Use this for initialization
    void Start()
    {
        jumpCD = 0;
        this.gameObject.transform.position = shiporigin.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        float spawnObjRN = Random.Range(0, 200);
        float nextObjXOff = Random.Range(-3F, 3F);
        float nextObjYOff = Random.Range(-3F, 3F);

        if (spawnObjRN < 1)
        {
            GameObject obj = Instantiate(cubeObst, origin.transform.position + Vector3.up * nextObjXOff + Vector3.right * nextObjYOff, Quaternion.identity) as GameObject;
            obj.GetComponent<Rigidbody>().AddRelativeForce(obj.transform.forward * -20, ForceMode.Impulse);
        }
        else if (spawnObjRN < 2)
        {
            GameObject obj = Instantiate(cylObst, origin.transform.position + Vector3.up * nextObjXOff + Vector3.right * nextObjYOff, Quaternion.identity) as GameObject;
            obj.GetComponent<Rigidbody>().AddRelativeForce(obj.transform.forward * -20, ForceMode.Impulse);
        }
        else if (spawnObjRN < 3)
        {
            GameObject obj = Instantiate(capObst, origin.transform.position + Vector3.up * nextObjXOff + Vector3.right * nextObjYOff, Quaternion.identity) as GameObject;
            obj.GetComponent<Rigidbody>().AddRelativeForce(obj.transform.forward * -20, ForceMode.Impulse);
        }
        else if (spawnObjRN < 4)
        {
            GameObject obj = Instantiate(powerUp, origin.transform.position + Vector3.up * nextObjXOff + Vector3.right * nextObjYOff, Quaternion.identity) as GameObject;
            obj.GetComponent<Rigidbody>().AddRelativeForce(obj.transform.forward * -20, ForceMode.Impulse);
        }
        // print("==");
        // print(this.gameObject.transform.position.x);
        // print(this.gameObject.transform.position.y);
        // print(this.gameObject.transform.position.z);
        if (Input.GetKey(KeyCode.D) && (this.gameObject.transform.position.x - shiporigin.transform.position.x < 5))
        {
            ship.Translate(Vector3.right * 0.5F);
        }
        if (Input.GetKey(KeyCode.A) && (shiporigin.transform.position.x - this.gameObject.transform.position.x < 5))
        {
            ship.Translate(Vector3.left * 0.5F);
        }
        if (Input.GetKey(KeyCode.W) && (this.gameObject.transform.position.z - shiporigin.transform.position.z < 5))
        {
            ship.Translate(Vector3.forward * 0.5F);
        }
        if (Input.GetKey(KeyCode.S) && (shiporigin.transform.position.z - this.gameObject.transform.position.z < 5))
        {
            ship.Translate(Vector3.back * 0.5F);
        }
        if (Input.GetKey(KeyCode.Space) && jumpCD == 0)
        {
            ship.GetComponent<Rigidbody>().AddForce(Vector3.up * 50000000);
            jumpCD = 300;
        }
        if (jumpCD > 0)
        {
            jumpCD = jumpCD - 1;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Terrain")
        {
            return;
        }
        if (collision.gameObject.tag == "Good")
        {
            //  print("score"+score);
            Destroy(collision.gameObject);//.SetActive(false);
            score++;
        }
        else {

            //   print("lives:" + lives);
            Destroy(collision.gameObject);//.SetActive(false);
            lives--;
            if (lives == 0)
            {
                print("GAMEOVER"); //TODO insert gameover code here, link to gameover ui
            }
        }
    }
}
