using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Burst.Intrinsics;
using Unity.Serialization.Json;
using UnityEngine;
using static UnityEngine.Rendering.GPUSort;




public class EffectHolder
{
    public string Oname;
    public List<string> tags = new List<string>();
    public Dictionary<string, object> save_vars = new();
    public Dictionary<string, List<EffectPair>> effects = new();
    private Dictionary<string, EffectPair> defaulteffects;
    public EffectHolderType type;

    public enum EffectHolderType
    {
        Level,
        Card,
        Action
    }


    public EffectHolder()
    {
        type = EffectHolderType.Level;
    }
    #region overload
    public async Task Proc(EffectsUtils.Proc procname, List<EffectHolder> chain, params object[] arg)
    {
        string proc = EffectsUtils.procname[procname];
        if (effects[proc] != null)
        {
            for (int i = 0; i < effects[proc].Count; i++)
            {
                await effects[proc][i].Proc(proc, chain, arg);
            }
        }
    }
   
    public async Task FullProc(EffectsUtils.Proc procname, List<EffectHolder> chain, params object[] arg)
    {
        string proc = EffectsUtils.procname[procname];
        await GlobalProc(proc, chain, arg);
        await Proc(proc, chain, arg);
    }

    public virtual async Task SpecificFullProc(EffectsUtils.Proc procname, List<EffectHolder> chain, params object[] arg)
    {
        string proc = EffectsUtils.procname[procname];
        await FullProc(proc, chain, arg);
    }

    public virtual async Task SpecificFullDelayProc(EffectsUtils.Proc procname, List<EffectHolder> chain, DelayAction action, int action_position, params object[] arg)
    {
        string proc = EffectsUtils.procname[procname];
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await Proc(proc, chain, arg);
        await action.Run();
    }



    #endregion


    public async Task Proc(string proc, List<EffectHolder> chain, object[] arg)
    {
        if (effects[proc] != null)
        {
            for (int i = 0; i < effects[proc].Count; i++)
            {
                await effects[proc][i].Proc(proc, chain, arg);
            }
        }
    }


    public async Task FullProc(string proc, List<EffectHolder> chain,  params object[] arg)
    {
        await GlobalProc(proc, chain, arg);
        await Proc(proc, chain, arg);
    }

    public virtual async Task SpecificFullProc(string proc, List<EffectHolder> chain, params object[] arg)
    {
        await FullProc(proc, chain, arg);   
    }

    public virtual async Task SpecificFullDelayProc(string proc, List<EffectHolder> chain, DelayAction action, int action_position, params object[] arg)
    {
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action);  }
        
        await Proc(proc, chain, arg);
        await action.Run();
    }

    protected async Task GlobalProc(string proc, List<EffectHolder> chain, params object[] arg)
    {
        if (type != EffectHolderType.Level)
        {
            await ObjectUtils.LevelManager.Proc(proc, chain, arg);
        }
    }

 

    public async Task AddEffectPreset(string defaultname, string effect, string condition, List<EffectHolder> chain,  object[] arg)
    {
        AssetManager a = ObjectUtils.AssetManager;
        EffectPreset efPr = await a.LoadPresetAsync<EffectPreset>(effect);
        ConditionPreset cnPr = await a.LoadPresetAsync<ConditionPreset>(condition);
        EffectPair eP = new(this, efPr, cnPr, true);
        string[] firstproc = cnPr.firstproc;

        foreach (string proc in firstproc)
        {
            if (effects[proc] == null)
            {
                effects[proc] = new();
            }
            effects[proc].Add(eP);
        }
        defaulteffects[defaultname] = eP;

        await FullProc("effectadded", chain, eP);
    }

    public async Task AddEffect(string defaultname, string effect, string condition, List<EffectHolder> chain)
    {
        AssetManager a = ObjectUtils.AssetManager;
        EffectPreset efPr = JsonUtility.FromJson<EffectPreset>(effect);
        ConditionPreset cnPr = JsonUtility.FromJson<ConditionPreset>(condition);
        EffectPair eP = new(this, efPr, cnPr, true);
        string[] firstproc = cnPr.firstproc;

        foreach (string proc in firstproc)
        {
            if (effects[proc] == null)
            {
                effects[proc] = new();
            }
            effects[proc].Add(eP);
        }
        defaulteffects[defaultname] = eP;

        await FullProc("effectadded", chain,  eP);
    }

    public async Task AddEffectPreset(string effect, string condition, List<EffectHolder> chain)
    {
        AssetManager a = ObjectUtils.AssetManager;
        EffectPreset efPr = await a.LoadPresetAsync<EffectPreset>(effect);
        ConditionPreset cnPr = await a.LoadPresetAsync<ConditionPreset>(condition);
        EffectPair eP = new(this, efPr, cnPr, false);
        string[] firstproc = cnPr.firstproc;

        foreach (string proc in firstproc)
        {
            if (effects[proc] == null)
            {
                effects[proc] = new();
            }
            effects[proc].Add(eP);
        }

        await FullProc("effectadded", chain, eP);
    }

    public async Task AddEffect(string effect, string condition, List<EffectHolder> chain, params object[] arg)
    {
        AssetManager a = ObjectUtils.AssetManager;
        EffectPreset efPr = JsonUtility.FromJson<EffectPreset>(effect);
        ConditionPreset cnPr = JsonUtility.FromJson<ConditionPreset>(condition);
        EffectPair eP = new(this, efPr, cnPr, false);
        string[] firstproc = cnPr.firstproc;

        foreach (string proc in firstproc)
        {
            if (effects[proc] == null)
            {
                effects[proc] = new();
            }
            effects[proc].Add(eP);
        }

        await FullProc("effectadded", chain, eP);
    }

    public async Task RemoveDefaultEffect(string defaultname, List<EffectHolder> chain, params object[] arg)
    {
        EffectPair effectPair = defaulteffects[defaultname];
        if (effectPair != null)
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


            await FullProc("effectremoved", chain, effectPair);
        }
    }
    public async Task RemoveEffect(EffectPair effectPair, List<EffectHolder> chain, params object[] arg)
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

        await FullProc("effectremoved", chain, effectPair);
    }

 

    public void AddTag(string tag)
    {
        if (!tags.Contains(tag))
        {
            tags.Add(tag);
        }    
    }

    public void RemoveTag(string tag)
    {
        if (tags.Contains(tag))
        {
            tags.Remove(tag);
        }
    }
}




