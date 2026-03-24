using System;
using System.Threading.Tasks;
using UnityEngine;

public class ChooseToken : ICloneable
{
    public string Name { get; protected set; }
    public int size, range, targets;
    public ChooseResult result;
    public Vector3Int center;
    public EffectHolder holder;
    public GameEntity player;

    public string sizeShape, rangeShape;
    public ChooseType type;

    public enum ChooseType
    {
        positive,
        negative,
        other
    }

    protected async virtual Task<ChooseResult> AlgoVisualize(LevelManager man)
    {
        return null;
    }

    protected async virtual Task<ChooseResult> Visualize(LevelManager man)
    {
        return null;
    }

    public async Task<ChooseResult> TryVisualize(LevelManager man)
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
