using System;
using System.Collections.Generic;
using System.Linq;


public class Condition : InternalWork
{
    
    public virtual bool Proc(string proc, string[] variables, List<EffectPair> chain, EffectPair owner, List<object> args)
    {
        if (chain.Contains(owner))
        {
            return false;
        }    
        return true;
    }
}
public class Effect : InternalWork
{
    public virtual void Proc(string proc, string[] variables, List<EffectPair> chain, EffectPair owner, List<object> args)
    {

    }
}

public class EffectPair
{
    public bool base_effect { get; private set; }
    public Effect effect { get; private set; }
    public Condition condition { get; private set; }
    public EffectHolder holder;
    private string name;
    public string[] firstproc { get; private set; }
    public string[] cVariables { get; private set; }
    public string[] eVariables { get; private set; }

    public EffectPair(bool base_effect, EffectHolder holder, EffectPreset effect, ConditionPreset condition)
    {
        this.base_effect = base_effect;
        this.holder = holder;
        name = effect.filename + ";" + condition.filename;
        firstproc = condition.firstproc;

        AssetManager aS = ObjectUtils.AssetManager;
        if (condition.basecondition == "default")
        {
            this.condition = new Condition();
        }
        else
        {
            this.condition = aS.LoadInternalWork<Condition>(condition.basecondition);
        }

        this.effect = aS.LoadInternalWork<Effect>(effect.baseeffect);

    }

    public void Proc(string proc, List<EffectPair> chain, List<object> args)
    {
        string proc_optimized = EffectsUtils.StandardString(proc);
        if (condition.Proc(proc_optimized, cVariables, chain, this, args))
        {
            List<EffectPair> new_chain;
            if (chain != null)
            {
                new_chain = new List<EffectPair>(chain);
            }
            else
            {
                new_chain = new List<EffectPair>();
            }    
            effect.Proc(proc_optimized, eVariables, new_chain, this, args);
        }
    }
    public bool CompareEffectName(string name)
    {
        string eName = this.name.Split(';').First();
        if (eName == name)
        {
            return true;
        }
        return false;
    }
    public bool CompareConditionName(string name)
    {
        string cName = this.name.Split(';').Last();
        if (cName == name)
        {
            return true;
        }
        return false;
    }
    public bool CompareName(string eName, string cName)
    {
        if (CompareEffectName(eName) && CompareConditionName(cName))
        {
            return true;
        }
        return false;
    }
}

[Serializable]
public class EffectPreset : Preset
{
    public string[] variables;
    public string baseeffect;
    public EffectPreset ()
    {
        type = PresetType.Effect;
    }
}
[Serializable]
public class ConditionPreset : Preset
{
    public string[] firstproc;
    public string[] variables;
    public string basecondition;
    public ConditionPreset()
    {
        type = PresetType.Condition;
    }
}