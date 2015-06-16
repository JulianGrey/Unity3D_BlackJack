using UnityEngine;
using System.Collections;

public class CardScript : MonoBehaviour {
    private Transform graveyard;
    private float speed = 10.0f;
    private bool dispose = false;
    public bool isRevealed = false;

    public Quaternion rotation = Quaternion.identity;

    void Start() {
        graveyard = GameObject.Find("Graveyard").transform;
    }

    void OnTriggerEnter(Collider other) {
        if(other.tag == "Graveyard") {
            Destroy(gameObject);
        }
    }

    void FixedUpdate() {
        if(!GameObject.Find("Gameplay").GetComponent<GameplayScript>().betComplete) {
            dispose = true;
        }
        if(dispose) {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, graveyard.position, step);
        }
        if(isRevealed) {
            transform.eulerAngles = new Vector3(0, 0, 180);
        }
        else {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
}
