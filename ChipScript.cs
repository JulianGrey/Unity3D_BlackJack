using UnityEngine;
using System.Collections;

public class ChipScript : MonoBehaviour {

	void Start () {
        Debug.Log(GetComponent<Renderer>().material);
	}
}
