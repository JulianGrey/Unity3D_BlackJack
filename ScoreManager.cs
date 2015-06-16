using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour {
    GameplayScript gameplayScript;
    Text text;

    void Awake() {
        text = GetComponent<Text>();
    }

    void Start() {
        gameplayScript = GameObject.Find("Gameplay").GetComponent<GameplayScript>();
    }

    void FixedUpdate() {
        text.text = "Remaining funds: " + gameplayScript.totalChips;
        text.text += "\nRound Bet: " + gameplayScript.currentBet;
    }
}
