using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Card : EffectHolder
{
    public Dictionary<string, float> adjustable_variables;
    public CardPreset preset { get; protected set; }
    public int cost { get; protected set; }
    public Sprite image { get; protected set; }

    public string[] description { get; protected set; }
    public GameEntity player;
    public List<ActionToken> actions { get; protected set; }
    public List<ChooseToken> choices { get; protected set; }

    public override async Task SpecificFullProc(string proc, List<EffectPair> chain, List<object> arg)
    {
        await FullProc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
    }

    public override async Task SpecificFullDelayProc(string proc, List<EffectPair> chain, DelayAction action, int action_position = 0, List<object> arg = null)
    {
        if (arg == null) { arg = new List<object>(); }
        arg.Insert(action_position, action);
        await GlobalProc(proc, chain, arg);
        await Proc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
        await action.Run();
    }
    public Card()
    {
        type = EffectHolderType.Card;
    }

    public static string FindValue()
    {
        return "";
    }
}



public class ClassHolder
{

}


[Serializable]
public class CardPreset : Preset
{

}

