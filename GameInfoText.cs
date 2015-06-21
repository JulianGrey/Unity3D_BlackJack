using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameInfoText : MonoBehaviour {
    GameplayScript gameplayScript;
    Text text;

    void Awake() {
        text = GetComponent<Text>();
    }

    void Start() {
        gameplayScript = GameObject.Find("Gameplay").GetComponent<GameplayScript>();
    }

    void FixedUpdate() {
        if(!gameplayScript.roundComplete) {
            if(!gameplayScript.dealt) {
                text.text = "Place your bets!";
            }
            else if(gameplayScript.dealt && !gameplayScript.roundComplete) {
                text.text = "Round in progress";
            }
            else {
                text.text = "";
            }
        }
        else {
            if(!gameplayScript.payoutInfo) {
                if(gameplayScript.surrender) {
                    if(gameplayScript.dealerHand.blackJack) {
                        text.text = "Surrender denied!";
                    }
                    else {
                        text.text = "Hand surrendered!";
                    }
                }
                if(gameplayScript.payout) {
                    int netWinnings = gameplayScript.totalChips - gameplayScript.initTotalChips;
                    text.text = "Net winnings: ";

                    if(netWinnings > 0) {
                        text.text += "+";
                    }
                    text.text += netWinnings;
                }
                gameplayScript.payoutInfo = true;
            }
        }
    }
}
