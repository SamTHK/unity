using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

public class CardCondition
{

    public virtual bool Check(Card card)
    {
        return true;
    }
}