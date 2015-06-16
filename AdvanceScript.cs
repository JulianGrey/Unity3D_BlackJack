using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AdvanceScript : MonoBehaviour {
    public GameplayScript gameplayScript;
    Text text;

    void Awake() {
        text = GetComponent<Text>();
    }

    void Start() {
        gameplayScript = GameObject.Find("Gameplay").GetComponent<GameplayScript>();
    }

    void FixedUpdate() {
        if(gameplayScript.totalChips == 0 && gameplayScript.roundComplete) {
            text.text = "Game over";
        }
        else if(gameplayScript.roundComplete) {
            text.text = "Play again";
        }
        else if(gameplayScript.dealt == true && gameplayScript.currentBet > 0) {
            text.text = "Hit";
        }
        else {
            text.text = "Deal";
        }
    }
}
