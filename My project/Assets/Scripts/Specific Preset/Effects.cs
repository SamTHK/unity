using Febucci.UI.Examples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static UnityEngine.Rendering.GPUSort;


public class Condition 
{
    public virtual async Task<bool> Proc(string proc, string[] variables, List<EffectHolder> chain, EffectHolder owner, object[] args)
    {
        if (chain.Contains(owner))
        {
            return false;
        }    
        return true;
    }
}
public class Effect 
{
    public async virtual Task Proc(string proc, string[] variables, List<EffectHolder> chain, EffectHolder owner, object[] args)
    {

    }
}

public class EffectPair
{

    public Effect effect { get; protected set; }
    public Condition condition { get; protected set; }
    public EffectHolder holder { get; protected set; }

    public string name { get; protected set; }
    public bool defaultEffect { get; protected set; }
    public string[] firstproc { get; protected set; }

    public string[] cVariables;
    public string[] eVariables;

    public EffectPair(EffectHolder holder, EffectPreset effect, ConditionPreset condition, bool defaultEffect = false )
    {
        this.defaultEffect = defaultEffect;
        this.holder = holder;
        name = effect.filename;

        firstproc = condition.firstproc;

        AssetManager aS = ObjectUtils.AssetManager;
        if (condition.basecondition == "")
        {
            this.condition = new Condition();
        }
        else
        {
            this.condition = aS.LoadInternalWork<Condition>(condition.basecondition);
            cVariables = condition.variables;
        }

        if (condition.basecondition == "")
        {
            this.effect = null;
            
        }
        else
        {
            this.effect = aS.LoadInternalWork<Effect>(effect.baseeffect);

        }

        eVariables = effect.variables;

    }


    public async Task Proc(string proc, List<EffectHolder> chain, object[] args)
    {
       
            string proc_optimized = EffectsUtils.StandardString(proc);
            if (await condition.Proc(proc_optimized, cVariables, chain, holder, args))
            {
                List<EffectHolder> new_chain;
                if (chain != null)
                {
                    new_chain = new List<EffectHolder>(chain);
                }
                else
                {
                    new_chain = new List<EffectHolder>();
                }
                await effect.Proc(proc_optimized, eVariables, new_chain, holder, args);
            }
        
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