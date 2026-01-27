using System.Collections.Generic;

public abstract class Effector
{

}


public class EffectHolder
{
    public List<string> tags = new List<string>();
    public Dictionary<string, object> save_vars = new();
    public Dictionary<string, List<EffectPair>> effects = new();
    public Effector owner = null;
    public bool level_effect = true;

    public EffectHolder (Effector effector = null)
    {
        if (effector != null)
        {
            level_effect = false;
            owner = effector;
        }
    }
    public void Proc(string proc, List<EffectPair> chain, List<object> arg)
    {
        List<EffectPair> effects = this.effects[proc];
        foreach (EffectPair e in effects)
        {
                e.Proc(proc, chain, arg);
        }
        if (!level_effect)
        {
            ObjectUtils.LevelManager.Proc(proc, chain, arg);
        }
    }

    public async void AddEffect(bool base_, string effect, string condition, List<EffectPair> chain, params object[] arg)
    {
        AssetManager a = ObjectUtils.AssetManager;
        EffectPreset efPr = await a.LoadPresetAsync<EffectPreset>(effect);
        ConditionPreset cnPr = await a.LoadPresetAsync<ConditionPreset>(condition);
        EffectPair eP = new(base_, this, efPr, cnPr);
        string[] firstproc = cnPr.firstproc;

        foreach (string proc in firstproc)
        {
            if (effects[proc] == null)
            {
                effects[proc] = new();
            }
            effects[proc].Add(eP);
        }

        Proc("EffectAdded", chain, EffectsUtils.ObjectList(arg, eP));
    }

    public void RemoveEffect(EffectPair effectPair, List<EffectPair> chain, params object[] arg)
    {
        foreach (string key in effectPair.firstproc)
        {
            if (effects[key] != null)
            {
                effects[key].Remove(effectPair);
                if (effects[key].Count <= 0)
                {
                    effects[key] = null;
                }
            }
        }
      
        Proc("EffectRemoved", chain, EffectsUtils.ObjectList(arg, effectPair));
    }
}




