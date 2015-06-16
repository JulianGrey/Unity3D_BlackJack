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
                //else {
                //    text.text = "";
                //    if(gameplayScript.handOne.win) {
                //        if(gameplayScript.handOne.isSplit) {
                //            text.text = "Payout to player's first hand!";
                //        }
                //        else {
                //            text.text = "Payout to player!";
                //        }
                //    }
                //    if(gameplayScript.handTwo.win) {
                //        if(gameplayScript.handOne.win) {
                //            text.text += "\n";
                //        }
                //        text.text += "Payout to player's second hand!";
                //    }
                //    if(gameplayScript.dealerHand.win) {
                //        if(gameplayScript.handOne.win || gameplayScript.handTwo.win) {
                //            text.text += "\n";
                //        }
                //        text.text = "Payout to dealer!";
                //    }
                //    if(gameplayScript.handOne.bust) {
                //        if(gameplayScript.handOne.isSplit) {
                //            text.text += "PLAYER BUST (first hand)!";
                //        }
                //        else {
                //            text.text = "PLAYER BUST!";
                //        }
                //    }
                //    if(gameplayScript.handTwo.bust) {
                //        text.text += "PLAYER BUST (second hand)!";
                //    }
                //    if(gameplayScript.dealerHand.bust) {
                //        text.text = "DEALER BUST!";
                //    }
                //    if(gameplayScript.handOne.value == gameplayScript.dealerHand.value) {
                //        if(gameplayScript.handOne.isSplit) {
                //            text.text += "Push first hand";
                //        }
                //        else {
                //            text.text += "Push";
                //        }
                //    }
                //    if(gameplayScript.handTwo.value == gameplayScript.dealerHand.value) {
                //        text.text += "Push second hand";
                //    }
                //}
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
