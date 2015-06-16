using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DealerHandValueManager : MonoBehaviour {

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
            if(gameplayScript.dealerHand.hand[0].GetComponent<CardScript>().isRevealed) {
                text.text = gameplayScript.dealerHand.value.ToString();
            }
            else {
                text.text = gameplayScript.dealerRevealScore.ToString();
            }
        }
        else if(gameplayScript.roundComplete) {
            if(gameplayScript.dealerHand.value > 21) {
                text.text = "Bust";
            }
            else {
                if(gameplayScript.availableHands) {
                    text.text = gameplayScript.dealerHand.value.ToString();
                }
                else {
                    text.text = gameplayScript.dealerRevealScore.ToString();
                }
            }
        }
        else {
            text.text = "---";
        }
    }
}
