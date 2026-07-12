using System.Collections.Generic;

public class CardHolder
{
    public GameEntity player;
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
        if (priority.Count == 0)
            return true;

        if (card == rigid[0])
            return true;

        return false;
    }
}
