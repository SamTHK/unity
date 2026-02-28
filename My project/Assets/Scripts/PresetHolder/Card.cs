using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : EffectHolder
{
    public Dictionary<string, float> adjustable_variables; 
    public CardPreset preset {  get; protected set; }
    public int cost { get; protected set; }
    public Sprite image { get; protected set; }

    public string[] description { get; protected set; }

    public List<ActionToken> actions { get; protected set; }
    public List<ChooseToken> choices { get; protected set; }

    public Card()
    {
        type = EffectHolderType.Card;
    }
}

public class ClassHolder
{

}


[Serializable]
public class CardPreset : Preset
{ 

}

