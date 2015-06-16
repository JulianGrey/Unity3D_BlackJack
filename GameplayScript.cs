using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GameplayScript : MonoBehaviour {
    public DeckScript deckScript;
    public List<DeckScript.Card> deck = new List<DeckScript.Card>();

    public HandScript.Hand dealerHand = new HandScript.Hand();
    public HandScript.Hand handOne = new HandScript.Hand();
    public HandScript.Hand handTwo = new HandScript.Hand();

    public Button[] gameButtons;
    public Texture[] textures;

    public GameObject cardPrefab;
    public GameObject activeMarkerPrefab;
    private GameObject activeArrow;
    public Transform handOneArea;
    public Transform handTwoArea;
    public Transform dealerArea;
    public Transform cardStart;
    public Vector3 activeArrowStart;

    public int initNumCards = 0;
    private int numCards = 0;
    private int numDecks = 6;

    public int dealerRevealScore = 0;

    public int currentBet = 0;
    public int totalChips;
    public int initTotalChips;
    public int payoutAmount = 0;

    private float activeArrowOffset = -0.9f;
    private float speed = 20.0f;

    private bool aceChanged;
    public bool betComplete;
    public bool dealt;
    public bool firstTurn;
    public bool stood;
    public bool shuffleDeck;

    public bool canDouble;
    public bool canInsure;
    public bool availableHands = true;
    public bool surrender;

    public bool isInsured;

    public bool payout;
    public bool payoutInfo;

    public bool resetStats;
    public bool roundComplete;

    void Awake() {
        deckScript = GetComponent<DeckScript>();

        deck = deckScript.BuildDeck(numDecks);
        initNumCards = numCards = deck.Count;

        handOne.handArea = handOneArea;
        handTwo.handArea = handTwoArea;
        dealerHand.handArea = dealerArea;
    }

    void Start() {
        aceChanged = false;
        betComplete = false;
        canDouble = false;
        canInsure = false;
        dealerHand.bust = false;
        dealerHand.win = false;
        dealt = false;
        firstTurn = false;
        initTotalChips = 0;
        isInsured = false;
        availableHands = true;
        payout = false;
        payoutInfo = false;
        resetStats = false;
        roundComplete = false;
        shuffleDeck = false;
        stood = false;
        surrender = false;
        totalChips = GameObject.Find("PlayerAttributes").GetComponent<PlayerAttributesScript>().totalChips;

        handOne.canAcceptBlackjack = true;
        handOne.isPlayer = true;
        handTwo.isPlayer = true;
        dealerHand.canAcceptBlackjack = true;

        activeArrowStart = new Vector3(handOneArea.position.x + activeArrowOffset, handOneArea.position.y, handOneArea.position.z - 2.0f);
        activeArrow = Instantiate(activeMarkerPrefab, activeArrowStart, Quaternion.identity) as GameObject;
        activeArrow.transform.eulerAngles = new Vector3(0, 180, 0);
        activeArrow.name = "Active Arrow";
    }

    // Calculate the value of a specified hand
    int CalculateValue(int cardValue, HandScript.Hand hand) {
        int handValue = 0;

        for(int i = 0; i < hand.cards.Count; i++) {
            handValue += hand.cards[i].nValue;
        }

        if(handValue > 21) {
            aceChanged = false;
            CheckAces(hand);
            if(aceChanged) {
                handValue = CalculateValue(hand);
            }
        }

        return handValue;
    }

    // Second version of calculating the value of a specified hand.
    // This version is for when an Ace has been found in a hand. The
    // result is that it will recursively loop through the hand looking
    // for Aces until either there are no Aces left, or the hand's value
    // is less than 21. This is the only way I could get this to work
    // without using a while loop, which crashed Unity despite the logic
    // being able to break out of the while loop.
    int CalculateValue(HandScript.Hand hand) {
        int handValue = 0;

        for(int i = 0; i < hand.cards.Count; i++) {
            handValue += hand.cards[i].nValue;
        }

        if(handValue > 21) {
            aceChanged = false;
            CheckAces(hand);
            if(aceChanged) {
                handValue = CalculateValue(hand);
            }
        }

        return handValue;
    }

    void ChangeButtonFunctionality() {
        if(!dealt) {
            // Before the round
            foreach(Button button in gameButtons) {
                if(button.tag == "Bet1" && totalChips >= 1 || button.tag == "Bet5" && totalChips >= 5 || button.tag == "Bet10" && totalChips >= 10 || button.tag == "Bet50" && totalChips >= 50 ||
                        button.tag == "Bet100" && totalChips >= 100 || button.tag == "Hit" && currentBet > 0 || button.tag == "ClearBet" && currentBet > 0 || button.tag == "MainMenu") {
                    button.interactable = true;
                }
                else {
                    button.interactable = false;
                }
            }
        }
        else if(roundComplete) {
            // After the round
            foreach(Button button in gameButtons) {
                if(totalChips > 0) {
                    if(button.tag == "Hit" || button.tag == "MainMenu") {
                        button.interactable = true;
                    }
                    else {
                        button.interactable = false;
                    }
                }
                else {
                    if(button.tag == "MainMenu") {
                        button.interactable = true;
                    }
                    else {
                        button.interactable = false;
                    }
                }
            }
        }
        else {
            foreach(Button button in gameButtons) {
                // Split / Double / Insurance / Surrender
                if(button.tag == "Hit"/* || gameButtons[i].tag == "Stand"*/) {
                    button.interactable = true;
                }
                else if(button.tag == "Stand" && !roundComplete) {
                    // Temporary until Stand button is changed to "Quit"
                    button.interactable = true;
                }
                else if(firstTurn) {
                    if(button.tag == "Split" && (handOne.cards[0].sValue == handOne.cards[1].sValue || handOne.cards[0].nValue == handOne.cards[1].nValue) && totalChips >= currentBet * 2) {
                        button.interactable = true;
                    }
                    else if(button.tag == "Double" && (handOne.value >= 9 && handOne.value <= 11) && totalChips >= currentBet * 2) {
                        button.interactable = true;
                    }
                    else if(button.tag == "Insurance" && dealerRevealScore == 11 && totalChips >= currentBet / 2) {
                        button.interactable = true;
                    }
                    else if(button.tag == "Surrender" && !isInsured) {
                        button.interactable = true;
                    }
                    else {
                        button.interactable = false;
                    }
                }
                else {
                    button.interactable = false;
                }
            }
        }
    }

    // Look for a high value Ace, and make it a low value if one exists
    void CheckAces(HandScript.Hand hand) {
        for(int i = 0; i < hand.cards.Count; i++) {
            if(hand.cards[i].nValue == 11) {
                hand.cards[i].nValue = 1;
                aceChanged = true;
                break;
            }
        }
    }

    void CheckBusts(HandScript.Hand hand) {
        if(hand.value > 21) {
            hand.bust = true;
        }
        if(handOne.isSplit && handOne.bust && handTwo.bust || !handOne.isSplit && handOne.bust) {
            availableHands = false;
        }
    }

    void CheckWinCondition(HandScript.Hand hand) {
        payoutAmount = 0;
        if(surrender) {
            if(dealerHand.blackJack) {
                payoutAmount = 0;
            }
            else {
                payoutAmount += currentBet / 2;
            }
        }
        else {
            if(dealerHand.bust) {
                if(hand.value <= 21) {
                    hand.win = true;
                }
            }
            else {
                if(isInsured) {
                    if(dealerHand.blackJack) {
                        payoutAmount += handOne.insuranceWinnings;
                    }
                }
                if(hand.value <= 21 && hand.value > dealerHand.value) {
                    hand.win = true;
                }
                else if(hand.value < dealerHand.value) {
                    dealerHand.win = true; // Might not need this, could check for if player hands haven't won
                }
                else if(hand.value == dealerHand.value) {
                    hand.push = true;
                }
            }

            if(hand.win) {
                if(hand.blackJack && hand.canAcceptBlackjack) {
                    payoutAmount += hand.potentialWinnings * 3;
                }
                else {
                    payoutAmount += hand.potentialWinnings * 2;
                }
            }
            else if(hand.push && !hand.bust) {
                payoutAmount += hand.potentialWinnings;
            }
        }
        totalChips += payoutAmount;
    }

    public void DealCard(HandScript.Hand hand) {
        GameObject card;
        int randomCard = Random.Range(0, numCards);
        hand.cards.Add(deck[randomCard]);
        hand.value = CalculateValue(deck[randomCard].nValue, hand);
        deck.Remove(deck[randomCard]);
        numCards--;

        card = Instantiate(cardPrefab, cardStart.transform.position, Quaternion.identity) as GameObject;
        card.name = hand.cards[hand.cards.Count - 1].name;
        // Grab the texture from the textures list
        card.GetComponent<Renderer>().material.mainTexture = textures[hand.cards[hand.cards.Count - 1].texture];
        if(!hand.isPlayer && hand.hand.Count == 0) {
            card.GetComponent<CardScript>().isRevealed = false;
        }
        else {
            card.GetComponent<CardScript>().isRevealed = true;
        }
        // Add the gameObject to the hand
        hand.hand.Add(card);
    }

    void DealInitialCards() {
        for(int i = 0; i < 4; i++) {
            if(i % 2 == 0) {
                DealCard(dealerHand);
            }
            else {
                DealCard(handOne);
            }
            if(i == 3) {
                dealt = true;
            }
        }
    }

    void MoveActiveArrow() {
        float step = speed * Time.deltaTime;

        if(handOne.active) {
            activeArrow.transform.position = Vector3.MoveTowards(activeArrow.transform.position, new Vector3(handOneArea.position.x + activeArrowOffset, handOneArea.position.y, handOneArea.position.z), step);
        }
        else if(handTwo.active) {
            activeArrow.transform.position = Vector3.MoveTowards(activeArrow.transform.position, new Vector3(handTwoArea.position.x + activeArrowOffset, handTwoArea.position.y, handTwoArea.position.z), step);
        }
        else {
            activeArrow.transform.position = Vector3.MoveTowards(activeArrow.transform.position, activeArrowStart, step);
        }
    }

    void MoveCard(HandScript.Hand hand) {
        float step = speed * Time.deltaTime;
        for(int i = 0; i < hand.hand.Count; i++) {
            hand.hand[i].gameObject.transform.position = Vector3.MoveTowards(hand.hand[i].transform.position,
                                                                             new Vector3(hand.handArea.position.x + (i * 0.5f),
                                                                                         hand.handArea.position.y + (i * 0.01f),
                                                                                         hand.handArea.position.z),
                                                                             step);
        }
    }

    public void MoveToComplete() {
        firstTurn = false;
        roundComplete = true;
        handOne.stand = true;
        dealerHand.stand = true;
        stood = true;
        roundComplete = true;
    }

    public void ResetHand(HandScript.Hand hand) {
        hand.cards.Clear();
        hand.hand.Clear();
        hand.insuranceWinnings = 0;
        hand.potentialWinnings = 0;
        hand.value = 0;

        hand.active = false;
        hand.blackJack = false;
        hand.bust = false;
        hand.isSplit = false;
        hand.push = false;
        hand.stand = false;
        hand.win = false;
    }

    public void ResetStats() {
        currentBet = 0;
        dealerRevealScore = 0;
        initTotalChips = 0;
        payoutAmount = 0;

        aceChanged = false;
        betComplete = false;
        canDouble = false;
        canInsure = false;
        dealt = false;
        isInsured = false;
        payout = false;
        payoutInfo = false;
        roundComplete = false;
        availableHands = true;
        stood = false;
        surrender = false;
        ResetHand(dealerHand);
        ResetHand(handOne);
        ResetHand(handTwo);
    }

    public void TestCaseInsurance(bool blackjack, int firstCardValue = 0) {
        // The firstCardValue parameter allows selection of a card value for the first
        // card if a BlackJack hasn't been forced to be dealt. If this is left as
        // default, the second card will be a 10-value card.
        int cardArray = 0;

        if(blackjack) {
            // Give the dealer a 10 value card.
            // "12" is the array position for a 10 value card (King in this situation).
            cardArray = 12;
        }
        else {
            if(firstCardValue == 0) {
                cardArray = 8;
            }
            else {
                cardArray = firstCardValue - 1;
            }
        }
        for(int i = 0; i < 2; i++) {
            if(i == 1) {
                // Give the dealer an Ace. Due to the first card having an array value
                // less than this Ace's initial array value (13), its value on the
                // second deal will always be 12.
                cardArray = 12;
            }
            dealerHand.cards.Add(deck[cardArray]);
            dealerHand.value = CalculateValue(deck[cardArray].nValue, dealerHand);
            deck.Remove(deck[cardArray]);
            numCards--;
            
            GameObject card;
            card = Instantiate(cardPrefab, cardStart.transform.position, Quaternion.identity) as GameObject;
            card.name = dealerHand.cards[dealerHand.cards.Count - 1].name;
            // Grab the texture from the textures list
            card.GetComponent<Renderer>().material.mainTexture = textures[dealerHand.cards[dealerHand.cards.Count - 1].texture];

            if(i == 0) {
                card.GetComponent<CardScript>().isRevealed = false;
            }
            else {
                card.GetComponent<CardScript>().isRevealed = true;
            }
            dealerHand.hand.Add(card);
        }
    }

    public void TestCaseSplitPushBust() {
        if(!dealt && betComplete) {
            HandScript.Hand hand;

            DeckScript.Card kingDiamonds = deck[51];
            DeckScript.Card queenDiamonds = deck[50];
            DeckScript.Card jackDiamonds = deck[49];
            DeckScript.Card tenDiamonds = deck[48];
            DeckScript.Card[] cards = {kingDiamonds, queenDiamonds, jackDiamonds, tenDiamonds};

            for(int i = 0; i < 4; i++) {
                if(i % 2 == 0) {
                    hand = dealerHand;
                }
                else {
                    hand = handOne;
                }

                hand.cards.Add(cards[i]);
                hand.value = CalculateValue(cards[i].nValue, hand);
                deck.Remove(cards[i]);
                numCards--;

                GameObject card;
                card = Instantiate(cardPrefab, cardStart.transform.position, Quaternion.identity) as GameObject;
                card.name = hand.cards[hand.cards.Count - 1].name;
                // Grab the texture from the textures list
                card.GetComponent<Renderer>().material.mainTexture = textures[hand.cards[hand.cards.Count - 1].texture];

                if(i == 0) {
                    card.GetComponent<CardScript>().isRevealed = false;
                }
                else {
                    card.GetComponent<CardScript>().isRevealed = true;
                }
                hand.hand.Add(card);
                if(i == 3) {
                    dealt = true;
                }
            }
        }
        if(handOne.isSplit) {
            DeckScript.Card kingClubs = deck[38];
            
            handOne.cards.Add(kingClubs);
            handOne.value = CalculateValue(kingClubs.nValue, handOne);
            deck.Remove(kingClubs);
            numCards--;

            GameObject card;
            card = Instantiate(cardPrefab, cardStart.transform.position, Quaternion.identity) as GameObject;
            card.name = handOne.cards[handOne.cards.Count - 1].name;
            // Grab the texture from the textures list
            card.GetComponent<Renderer>().material.mainTexture = textures[handOne.cards[handOne.cards.Count - 1].texture];
            card.GetComponent<CardScript>().isRevealed = true;

            handOne.hand.Add(card);
        }
    }

    void FixedUpdate() {
        ChangeButtonFunctionality();
        CheckBusts(handOne);
        MoveActiveArrow();
        MoveCard(handOne);
        MoveCard(dealerHand);
        if(handOne.isSplit) {
            MoveCard(handTwo);
            CheckBusts(handTwo);
        }

        if(!dealt) {
            if(betComplete) {
                //TestCaseInsurance(false, 8); // If the parameter is "true", deal a BlackJack to the dealer
                //TestCaseSplitPushBust();
                DealInitialCards();
                // Store value of dealer's revealed card
                dealerRevealScore = dealerHand.cards[1].nValue;
                initTotalChips = totalChips + currentBet;
            }
            if(handOne.value == 21 && handOne.cards.Count == 2) {
                handOne.blackJack = true;
            }
            if(dealerHand.value == 21 && dealerHand.cards.Count == 2) {
                dealerHand.blackJack = true;
            }
        }
        else {
            if(firstTurn) {
                if(handOne.blackJack || dealerRevealScore == 10 && dealerHand.blackJack) {
                    MoveToComplete();
                }
                else {
                    if(handOne.value >= 9 && handOne.value <= 11) {
                        canDouble = true;
                    }
                    // Enable insurance if dealer's up card is an Ace
                    if(dealerRevealScore == 11 || dealerRevealScore == 1) {
                        canInsure = true;
                    }
                }
            }
            if(!stood) {
                if(handOne.isSplit) {
                    if(handOne.active) {
                        if(handOne.value >= 21) {
                            handOne.active = false;
                            handTwo.active = true;
                            handOne.stand = true;
                        }
                    }
                    else if(handTwo.active) {
                        if(handTwo.value >= 21) {
                            handTwo.active = false;
                            handTwo.stand = true;
                        }
                    }
                    else if(handOne.stand && handTwo.stand) {
                        stood = true;
                    }
                    if(handOne.bust && handTwo.bust) {
                        roundComplete = true;
                    }
                }
                else if(!handOne.isSplit) {
                    if(handOne.value >= 21) {
                        stood = true;

                        if(handOne.value > 21) {
                            roundComplete = true;
                        }
                    }
                }
            }
            if(stood && !roundComplete) {
                if(!handOne.isSplit && handOne.bust || handOne.isSplit && handOne.bust && handTwo.bust) {
                    roundComplete = true;
                }
                // Dealer dealt a soft 17 or dealer's hand value is less than 17
                else if(dealerHand.value == 17 && dealerHand.cards.Count == 2 && (dealerHand.cards[0].nValue == 11 || dealerHand.cards[1].nValue == 11) || dealerHand.value < 17 && !roundComplete) {
                    for(int i = 0; i < dealerHand.hand.Count; i++) {
                        dealerHand.hand[i].GetComponent<CardScript>().isRevealed = true;
                    }
                    DealCard(dealerHand);
                }
                else if(dealerHand.value >= 17) {
                    if(dealerHand.value > 21) {
                        dealerHand.bust = true;
                    }
                    roundComplete = true;
                }
            }
        }
        if(roundComplete && !payout) {
            handOne.active = false;
            handTwo.active = false;
            if(!surrender && availableHands) {
                for(int i = 0; i < dealerHand.hand.Count; i++) {
                    dealerHand.hand[i].GetComponent<CardScript>().isRevealed = true;
                }
            }
            CheckWinCondition(handOne);
            if(handOne.isSplit) {
                CheckWinCondition(handTwo);
            }
            payout = true;
        }
        if(roundComplete && payout && deck.Count < initNumCards / 3) {
            deck = deckScript.BuildDeck(numDecks);
            numCards = deck.Count;
        }
    }
}
