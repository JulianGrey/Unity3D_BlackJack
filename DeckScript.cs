using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DeckScript : MonoBehaviour {
    public class Card {
        public int nValue, texture;
        public string suit, sValue, colour;
        public string name;
        public bool changedAce = false;

        public Card(int nVal, string sColour, string sSuit, string sVal, int texSuit, int texValue) {
            nValue = nVal;
            colour = sColour;
            suit = sSuit;
            sValue = sVal;

            // The preliminary checks for the texture to be used for the card will be done here. This calculation
            // provides an integer which will be used as the position in the textures array that holds the
            // respective texture for the card in question.
            texture = (13 * texSuit) + (texValue - 1);

            name = sVal + " of " + suit;
        }
    }

    string Wordify(int value) {
	    string[] strValue = {"Ace", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King"};
	    return strValue[value - 1];
    }

    public List<DeckScript.Card> BuildDeck(int decks = 1) {
        List<DeckScript.Card> deck = new List<DeckScript.Card>();
        string sColour = "";
        string sSuit = "";
        string sValue = "";
        int decksCreated = 0;
        // Variables to hold values for determining the texture the card should have
        int tSuit = 0;
        int tValue = 0;

        while(decksCreated < decks) {
            for(int suit = 0; suit < 4; suit++) {
                tSuit = suit;
                switch(suit) {
                    case 0:
                        sSuit = "Spades";
                        sColour = "Black";
                        break;
                    case 1:
                        sSuit = "Hearts";
                        sColour = "Red";
                        break;
                    case 2:
                        sSuit = "Clubs";
                        sColour = "Black";
                        break;
                    case 3:
                        sSuit = "Diamonds";
                        sColour = "Red";
                        break;
                }

                for(int faceValue = 1; faceValue < 14; faceValue++) {
                    int nValue = 0;
                    tValue = faceValue;
                    sValue = Wordify(faceValue);

                    if(sValue == "Jack" || sValue == "Queen" || sValue == "King") {
                        nValue = 10;
                    }
                    else if(sValue == "Ace") {
                        nValue = 11;
                    }
                    else {
                        nValue = faceValue;
                    }

                    Card card = new Card(nValue, sColour, sSuit, sValue, tSuit, tValue);
                    deck.Add(card);
                }
            }
            decksCreated++;
        }
        return deck;
    }
}
