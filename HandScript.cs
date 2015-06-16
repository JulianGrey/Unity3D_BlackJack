using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HandScript : MonoBehaviour {

    public class Hand {
        public List<DeckScript.Card> cards = new List<DeckScript.Card>();
        public List<GameObject> hand = new List<GameObject>();
        public Transform handArea;

        public int insuranceWinnings = 0;
        public int potentialWinnings = 0;
        public int value = 0;

        public bool canAcceptBlackjack = false;

        public bool active = false;
        public bool blackJack = false;
        public bool bust = false;
        public bool isPlayer = false;
        public bool isSplit = false;
        public bool push = false;
        public bool stand = false;
        public bool win = false;
    }
}
