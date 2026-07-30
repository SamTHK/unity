using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class Creature : GameEntity
{
    public CardHolder cardHolder;
    public Team team;
    public bool controllable;
    public int speed;

    public override bool CheckStatic()
    {
        return false;
    }
    public Creature()
    {
   
    }

    public override async Task EndTurn()
    {

        cardHolder.RunReserve();
    }
}
