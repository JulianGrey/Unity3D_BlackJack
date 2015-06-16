using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuScript : MonoBehaviour {
    public Button continueButton;
    public GameObject playerAttributesPrefab;
    public GameObject playerAttributesObject;
    public PlayerAttributesScript playerAttributesScript;

    void Start() {
        if(GameObject.Find("PlayerAttributes") == null) {
            GameObject playerAttributes = Instantiate(playerAttributesPrefab, Vector3.zero, Quaternion.identity) as GameObject;
            playerAttributes.name = "PlayerAttributes";
        }
        playerAttributesObject = GameObject.Find("PlayerAttributes");
        playerAttributesScript = playerAttributesObject.GetComponent<PlayerAttributesScript>();
    }

    public void NewGame() {
        playerAttributesScript.totalChips = 100;
        Application.LoadLevel("Game");
    }

    public void ContinueGame() {
        Application.LoadLevel("Game");
    }

    public void CloseGame() {
        Application.Quit();
    }

    void FixedUpdate() {
        if(playerAttributesScript.totalChips > 0) {
            continueButton.interactable = true;
        }
        else {
            continueButton.interactable = false;
        }
    }
}
