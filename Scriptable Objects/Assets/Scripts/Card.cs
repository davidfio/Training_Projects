using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Cards")]
public class Card : ScriptableObject
{
    public string cardName;
    public string description;

    public Sprite artwork;

    public byte manaCost;
    public byte attack;
    public byte hp;

    public void Print()
    {
        Debug.Log(name + ": " + description + "The card costs: " + manaCost);
    }
}
