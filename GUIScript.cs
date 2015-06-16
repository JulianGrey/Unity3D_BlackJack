using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIScript : MonoBehaviour {
    GameplayScript gameplayScript;
    
    void Start() {
        gameplayScript = GameObject.Find("Gameplay").GetComponent<GameplayScript>();
    }

    public void GoToMainMenu() {
        if(!gameplayScript.dealt || gameplayScript.roundComplete) {
            if(gameplayScript.currentBet > 0 && !gameplayScript.dealt) {
                // Make sure that if the player has placed a bet but hasn't started
                // a round, the amount bet is added to the player's chip total
                gameplayScript.totalChips += gameplayScript.currentBet;
            }
            GameObject.Find("PlayerAttributes").GetComponent<PlayerAttributesScript>().totalChips = gameplayScript.totalChips;
            Application.LoadLevel("MainMenu");
        }
    }
    
    public void PlaceBet(int bet) {
        if(!gameplayScript.dealt) {
            if(bet <= gameplayScript.totalChips) {
                gameplayScript.currentBet += bet;
                gameplayScript.totalChips -= bet;
            }
        }
    }

    public void ClearBet() {
        if(!gameplayScript.betComplete) {
            if(gameplayScript.currentBet > 0) {
                gameplayScript.totalChips += gameplayScript.currentBet;
                gameplayScript.currentBet = 0;
            }
        }
    }

    // Hit and initial deal
    public void Advance() {
        // Deal
        if(!gameplayScript.betComplete && gameplayScript.currentBet > 0 && !gameplayScript.roundComplete) {
            gameplayScript.betComplete = true;
            gameplayScript.firstTurn = true;
            gameplayScript.handOne.potentialWinnings = gameplayScript.currentBet;

            gameplayScript.handOne.active = true;
        }
        if(gameplayScript.totalChips == 0 && gameplayScript.roundComplete) {
            // Go to main menu
        }
        // Hit
        else if(gameplayScript.handOne.isSplit && !gameplayScript.roundComplete) {
            if(gameplayScript.handOne.active) {
                gameplayScript.DealCard(gameplayScript.handOne);
                if(gameplayScript.handOne.value >= 21) {
                    gameplayScript.handOne.stand = true;
                    gameplayScript.handOne.active = false;
                    gameplayScript.handTwo.active = true;
                }
            }
            else if(gameplayScript.handTwo.active) {
                gameplayScript.DealCard(gameplayScript.handTwo);
                if(gameplayScript.handTwo.value >= 21) {
                    gameplayScript.handTwo.stand = true;
                    gameplayScript.handTwo.active = false;
                }
            }
        }
        else if(!gameplayScript.handOne.isSplit && !gameplayScript.roundComplete) {
            if(gameplayScript.handOne.value < 21 && gameplayScript.currentBet > 0 && gameplayScript.dealt) {
                gameplayScript.DealCard(gameplayScript.handOne);
                if(gameplayScript.handOne.value > 21) {
                    gameplayScript.handOne.bust = true;
                }

                if(gameplayScript.firstTurn) {
                    gameplayScript.firstTurn = false;
                }
            }
        }
        // Play again
        else if(gameplayScript.roundComplete) {
            gameplayScript.ResetStats();

            if(gameplayScript.deck.Count < gameplayScript.initNumCards / 3) {
                gameplayScript.shuffleDeck = true;
            }
        }
    }

    // Stand
    public void Stand() {
        if(gameplayScript.dealt) {
            if(gameplayScript.handOne.isSplit) {
                if(gameplayScript.handOne.active) {
                    gameplayScript.handOne.stand = true;
                    gameplayScript.handOne.active = false;
                    gameplayScript.handTwo.active = true;
                }
                else if(gameplayScript.handTwo.active) {
                    gameplayScript.handTwo.stand = true;
                    gameplayScript.handTwo.active = false;
                }
            }
            else {
                gameplayScript.stood = true;
            }
        }

        if(gameplayScript.firstTurn) {
            gameplayScript.firstTurn = false;
        }
    }

    // Double
    public void DoubleUp() {
        if(gameplayScript.canDouble) {
            if(gameplayScript.currentBet <= gameplayScript.totalChips) {
                gameplayScript.DealCard(gameplayScript.handOne);
                gameplayScript.totalChips -= gameplayScript.currentBet;
                gameplayScript.currentBet *= 2;
                gameplayScript.handOne.potentialWinnings *= 2;
                gameplayScript.stood = true;

                if(gameplayScript.firstTurn) {
                    gameplayScript.firstTurn = false;
                }
            }
        }
    }

    // Split
    public void Split() {
        if(gameplayScript.firstTurn && gameplayScript.currentBet <= gameplayScript.totalChips) {
            gameplayScript.firstTurn = false;
            gameplayScript.handOne.isSplit = true;
            gameplayScript.handOne.active = true;
            gameplayScript.handTwo.potentialWinnings = gameplayScript.currentBet;
            gameplayScript.totalChips -= gameplayScript.currentBet;
            gameplayScript.currentBet *= 2;

            // Move the split card to the second hand position
            //gameplayScript.handOne.hand[1].transform.position = gameplayScript.handTwo.handArea.position;

            // Split the hands
            gameplayScript.handTwo.cards.Add(gameplayScript.handOne.cards[1]);
            gameplayScript.handOne.cards.Remove(gameplayScript.handOne.cards[1]);
            gameplayScript.handTwo.hand.Add(gameplayScript.handOne.hand[1]);
            gameplayScript.handOne.hand.Remove(gameplayScript.handOne.hand[1]);

            // Revert a low value Ace to high value if splitting Aces
            if(gameplayScript.handOne.cards[0].nValue == 1) {
                gameplayScript.handOne.cards[0].nValue = 11;
            }
            if(gameplayScript.handTwo.cards[0].nValue == 1) {
                gameplayScript.handTwo.cards[0].nValue = 11;
            }

            gameplayScript.TestCaseSplitPushBust();
            //gameplayScript.DealCard(gameplayScript.handOne);
            gameplayScript.DealCard(gameplayScript.handTwo);
        }
    }

    // Surrender
    public void Surrender() {
        gameplayScript.roundComplete = true;
        gameplayScript.stood = true;
        gameplayScript.surrender = true;
        if(gameplayScript.dealerHand.blackJack) {
            // Forefeit bet
            gameplayScript.dealerHand.win = true;
        }
    }

    // Insurance
    public void GetInsurance() {
        int halfOfBet = (int)(gameplayScript.currentBet / 2);
        if(gameplayScript.totalChips >= halfOfBet) {
            gameplayScript.handOne.insuranceWinnings = halfOfBet * 3;
            gameplayScript.totalChips -= halfOfBet;
            gameplayScript.canInsure = false;
            gameplayScript.isInsured = true;

            if(gameplayScript.dealerHand.blackJack) {
                gameplayScript.MoveToComplete();
            }
        }
    }
}
