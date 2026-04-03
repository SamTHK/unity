using System.Threading.Tasks;

public class ChooseToken
{
    public string Name { get; protected set; }
    public int size, range, targets;
    public ChooseResult result;
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

    public ChooseToken ShallowClone()
    {
        return (ChooseToken)MemberwiseClone();
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

}

public class ChooseResult
{

}

public class ChooseVisualize
{

}
