using UnityEngine;

public class Test : MonoBehaviour
{

    string description = "Walk a short distance. Clear all [Puddle,Fire], [Puddle,BiggerFire] and [Puddle,BiggestFire] gain 1 Decaying Power Up for tile 5/3/1 tile cleared. Do a medium melee attack that hit in straight line";


    private void Start()
    {
        idk2 e = new()
        {
            id = 10,
            e = 4
        };


        idk a = e.ShallowClone();
        a.e = 5;
        a.Gun();

        Debug.Log(a.e);
        Debug.Log(e.e);
    }
}

public class idk
{
    public int e;

    public idk ShallowClone()
    {
        return (idk)MemberwiseClone();
    }

    public virtual void Gun()
    {
        Debug.Log("fuck you");
    }
}

public class idk2 : idk
{
    public int id = 6;

    public override void Gun()
    {
        Debug.Log("love you");
    }
}