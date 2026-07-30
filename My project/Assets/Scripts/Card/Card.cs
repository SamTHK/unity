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

    public string description { get; protected set; }
    public Creature player;
    public CardCondition condition;
    public PlaceCard place;
    public int index = -1;
    public bool discard_on_reserve = true;
    public bool play = true;

    public enum PlaceCard
    {
        deck,
        hand,
        discard,
        exhaust,
        nowhere
    }

    public List<Page> pages = new();
    public int turn_reserved = -1;

    // remember to count up the reserve
    public void AddPage(Page page, int index, bool default_ = true)
    {
        int a = index;
        if (a < 0) { a = pages.Count + index; }


    }

    public void AddToken(OnTurnToken token, int index, int page_index)
    {
        token.holder = this;
        token.player = player;

        int a = page_index;
        if (a < 0) { a = pages.Count + page_index; }

        if (pages[a] is OnTurnPage p)
        {
            int b = index;
            if (b < 0) { b = p.actions.Count + index; }

            token.page = p;
            p.actions.Insert(b, token);
            token.SpecificFullProc("token_add", null, true, token);


        }
    }

    public void AddToken(DefensiveToken token, int index, int page_index)
    {
        token.holder = this;
        token.player = player;

        int a = page_index;
        if (a < 0) { a = pages.Count + page_index; }

        if (pages[a] is DefensivePage p)
        {
            int b = index;
            if (b < 0) { b = p.actions.Count + index; }

            token.page = p;
            p.actions.Insert(b, token);
            token.SpecificFullProc("token_add", null, true, token);
        }
    }

    public async Task PingEnd()
    {
        player.cardHolder.AfterPlay(this);
        await SpecificFullProc("card_end", null, true, this);
    }

    public override async Task SpecificFullProc(EffectsUtils.Proc procname, List<EffectHolder> chain, bool global = true, params object[] arg)
    {
        string proc = EffectsUtils.procname[procname];
        if (global)
        { await GlobalProc(proc, chain, arg); }
        await Proc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
    }



    protected override async Task SpecificFullProc(string proc, bool global, List<EffectHolder> chain, object[] arg)
    {
        if (global)
        { await GlobalProc(proc, chain, arg); }
        await Proc(proc, chain, arg);
        await player.Proc(proc, chain, arg);
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

