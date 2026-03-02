using System;
using System.Threading.Tasks;
using UnityEngine;

public class ChooseToken : ICloneable
{
    public string Name { get; protected set; }
    public int size, range, targets;
    public ChooseResult result;
    public Vector3Int center;

    public async virtual Task<ChooseResult> Visualize(LevelManager man)
    {
        return null;
    }

   
    public virtual object Clone()
    {
        return new ChooseToken() { };
    }
}

public class ChooseResult
{

}

public class ChooseVisualize
{

}
