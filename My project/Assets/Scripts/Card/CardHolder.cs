using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine.AdaptivePerformance;

public class CardHolder
{

    public int energy;

    public Creature player;
    public List<Card> deck = new();
    public List<Card> hand = new();
    public List<Card> exhausted = new();
    public List<Card> discard = new();

    public List<Card> reserve = new();
    public List<Card> rigid = new();
    public List<Card> priority = new();
    public List<Card> priority_rigid = new();

    public bool CheckRigid(Card card)
    {
        if (!rigid.Contains(card) && !priority_rigid.Contains(card))
            return true;

        if (card == rigid[0] || card == priority_rigid[0])
            return true;

        return false;
    }

    public bool CheckPriorirty(Card card)
    {
        ///true is playable, false is not


        if (priority.Count == 0)
            return true;

        if (priority.Contains(card))
            return true;

        return false;
    }

    public void CardRecheck(Card card)
    {
        if (card.place != Card.PlaceCard.hand)
        {
            reserve.Remove(card);
            rigid.Remove(card);
            priority.Remove(card);
            priority_rigid.Remove(card);

            card.turn_reserved = 0;
            foreach (Page page in card.pages)
            {
                page.played = false;
            }
        }
        else 
        {
            Page p = FirstUnplayedPage(card);
            if (p != null)
            {
                
                if (p.turnleft <= 0 || p.turnleft <= card.turn_reserved)
                {
                    if (p.priority && p.rigid)
                    {
                        priority_rigid.Add(card);
                    }
                    else if (p.priority)
                    {
                        priority.Add(card);
                    }
                    else if (p.rigid)
                    {
                        rigid.Add(card);
                        rigid.OrderBy(x => x.index);
                    }
                }

            }
        }

    }

    public Page FirstUnplayedPage(Card card)
    {
        for (int i = 0; i < card.pages.Count; i ++)
        {
            Page p = card.pages[i];
            if (p!= null && p.played == false)
            {
                return p;
            }    
        }
        return null;
    }

    public void ChangePlace(Card card, Card.PlaceCard place, int position, bool random, List<EffectHolder> chain)
    {

        CardChangePlaceAction action = new(this, card, place, position, random);

        card?.SpecificFullDelayProc(chain, action);
        //nho lam effect o day

    }

    public void RunReserve()
    {
        foreach (Card card in reserve)
        {
            card.turn_reserved++;

            
            if (CheckReserve(card))
            {
                ChangePlace(card, Card.PlaceCard.discard, -1, false, null);
            }    
        }    
    }

    public bool CheckReserve(Card card)
    {
        ///true mean discard
        Page p = FirstUnplayedPage(card);

        if (p.turnleft < card.turn_reserved && card.discard_on_reserve)
        {
            return true;
        }    

        return false;
    }


    public void ChangePlaceInternal(Card card, Card.PlaceCard place, int position, bool random)
    {
        List<Card> og_place = Where(card.place), newplace = Where(place);
        og_place.Remove(card);

        int a;

        if (position >= 0)
        {
            a = position;
        }    
        else
        {
            a = newplace.Count + position;
        }    

        if (random)
        {
            int i = Manager.number_seed.Next(0, a);
            newplace.Insert(i, card);

        }
        else
        {
            newplace.Insert(a, card);


        }

        if (og_place == hand)
        {
            if (newplace != hand)
            {
                card.index = -1;
            }
            HandCheckIndex();
        }
        else if (newplace == hand)
        {
            HandCheckIndex();
            card.play = false;
        }


        UpdateAnimation();
    }

    private void HandCheckIndex()
    {
        for (int i = 0; i < hand.Count; i ++)
        {
            hand[i].index = i;
        }
        rigid.OrderBy(x => x.index); 
    }

    private void UpdateAnimation()
    {

    }

    private List<Card> Where(Card.PlaceCard place)
    {
        switch (place)
        {
            case Card.PlaceCard.hand:
                    return hand;
            case Card.PlaceCard.discard:
                return discard;
            case Card.PlaceCard.exhaust:
                return exhausted;
            case Card.PlaceCard.deck:
                return deck;
            default:
                return null;

        }
    }


    public async Task TryPlay(Card card, List<EffectHolder> chain, bool play_from_reserve = false)
    {
        bool priority = CheckPriorirty(card), rigid = CheckRigid(card), condition_fullfilled = card.condition.Check(card), allow = true;
        if (!priority || !rigid || !condition_fullfilled)
        {
            allow = false;
        }

        CardPlayAction action = await ObjectUtils.LevelManager.ParseActions(card, priority, rigid, condition_fullfilled, allow, chain);

        if (action.allow == true && !play_from_reserve)
        {
            energy -= card.cost;
            energy = energy < 0 ? 0 : energy;
        }
    }

    public void AfterPlay(Card card)
    {
        
            if ( card.pages.LastOrDefault()?.played == false)
            {
                card.play = true;
            }
            else if (card.turn_reserved == 0)
            {
                hand.Remove(card);
                hand.Insert(reserve.Count, card);
                card.index = reserve.Count;
                reserve.Add(card);
            }
            else if (card.place == Card.PlaceCard.hand)
            {
                ChangePlace(card, Card.PlaceCard.discard, -1, false, null);
            }
        
        CardRecheck(card);
    }

}
