using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardDisplay : MonoBehaviour
{
    public Card card;

    public Text nameText;
    public Text descriptionText;

    public Image artworkImage;

    public Text manaText;
    public Text attackText;
    public Text hpText;

	private void Start ()
    {
        // I can call a method created in the scriptable object
        //card.Print();

        SetUpCard();
        
    }

    private void SetUpCard()
    {
        nameText.text = card.cardName;
        descriptionText.text = card.description;

        artworkImage.sprite = card.artwork;
        manaText.text = card.manaCost.ToString();
        attackText.text = card.attack.ToString();
        hpText.text = card.hp.ToString();
    }	
}
