using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : GameEntity
{
    public CardHolder cardHolder;



    public override bool CheckStatic()
    {
        return false;
    }
    public Creature()
    {
   
    }    
}
