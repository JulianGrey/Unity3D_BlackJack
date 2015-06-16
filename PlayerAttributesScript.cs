using UnityEngine;
using System.Collections;

public class PlayerAttributesScript : MonoBehaviour {

    // The purpose of this script is to hold player values in a persistent
    // gameObject, such as the player's chip total. The intention of this
    // is to load these values in from an external file. Will need to look
    // into PlayerPrefs to see how Unity handles serialisation.

    public int totalChips;

	// Use this for initialization
	void Start () {
        DontDestroyOnLoad(transform.gameObject);
	}
}
