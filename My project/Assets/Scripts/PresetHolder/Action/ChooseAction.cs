using System;
using System.Threading.Tasks;

public class ChooseToken : ICloneable
{
    public bool returnable;
    public ChooseResult result;

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
