using System.Threading.Tasks;

public class ChooseToken
{
    public string Name { get; protected set; }
    public int size, range, targets;
    public ChooseResult result;
    public EffectHolder holder; ///most of the it's the card
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

    protected async virtual Task AlgoVisualize(LevelManager man, ChooseResult result)
    {
        return;
    }

    public async virtual Task<ChooseResult> Find()
    {
        return null;
    }

    protected async virtual Task Visualize(LevelManager man, ChooseResult result)
    {
        return;
    }


    public async Task TryVisualize(LevelManager man)
    {
        if (result == null)
            result = await Find();

        if (man.turn_of.controllable)
        {
            Visualize(man, result);
        }    
        else
        {
            AlgoVisualize(man, result);
        }    
    }

    public async Task TryVisualize()
    {
        LevelManager man = ObjectUtils.LevelManager;
        await TryVisualize(man);
    }
    /// remember to make this
}

public class ChooseResult
{

}

public class ChooseVisualize
{

}
