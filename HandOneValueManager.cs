using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HandOneValueManager : MonoBehaviour {

    GameplayScript gameplayScript;
    Text text;

    void Awake() {
        text = GetComponent<Text>();
    }

    void Start() {
        gameplayScript = GameObject.Find("Gameplay").GetComponent<GameplayScript>();
    }

    void FixedUpdate() {
        if(gameplayScript.dealt && !gameplayScript.roundComplete) {
            text.text = gameplayScript.handOne.value.ToString();
        }
        else if(gameplayScript.roundComplete) {
            if(gameplayScript.handOne.win) {
                text.text = "Win";
            }
            else if(gameplayScript.handOne.bust) {
                text.text = "Bust";
            }
            else if(gameplayScript.handOne.push) {
                text.text = "Push";
            }
            else {
                text.text = "Lose";
            }
        }
        else {
            text.text = "---";
        }
    }
}
