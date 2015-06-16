using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class HandTwoValueManager : MonoBehaviour {

    GameplayScript gameplayScript;
    Text text;

    void Awake() {
        text = GetComponent<Text>();
    }

    void Start() {
        gameplayScript = GameObject.Find("Gameplay").GetComponent<GameplayScript>();
    }

    void FixedUpdate() {
        if(gameplayScript.dealt && gameplayScript.handOne.isSplit && !gameplayScript.roundComplete) {
            text.text = gameplayScript.handTwo.value.ToString();
        }
        else if(gameplayScript.roundComplete && gameplayScript.handOne.isSplit) {
            if(gameplayScript.handTwo.win) {
                text.text = "Win";
            }
            else if(gameplayScript.handTwo.bust) {
                text.text = "Bust";
            }
            else if(gameplayScript.handTwo.push) {
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
