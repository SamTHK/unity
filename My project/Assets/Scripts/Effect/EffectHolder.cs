using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;




public class EffectHolder
{
    public string Oname;
    public List<string> tags = new();
    public Dictionary<string, object> save_vars = new();
    public Dictionary<string, List<EffectPair>> effects = new();
    public EffectHolderType type;

    public bool silenced = false;

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

  

    public virtual async Task SpecificFullProc(EffectsUtils.Proc procname, List<EffectHolder> chain, bool global = true, params object[] arg)
    {
        string proc = EffectsUtils.procname[procname];
        await SpecificFullProc(proc, global, chain, arg);
    }


    public async Task Proc(string proc, List<EffectHolder> chain, object[] arg)
    {
        if (!silenced)
        {
            if (effects[proc] != null)
            {
                for (int i = 0; i < effects[proc].Count; i++)
                {
                    await effects[proc][i].Proc(proc, chain, arg);
                }
            }
        }
    }


    public async Task FullProc(string proc, List<EffectHolder> chain, params object[] arg)
    {
        await GlobalProc(proc, chain, arg);
        await Proc(proc, chain, arg);
    }

    public async Task SpecificFullProc(string proc, List<EffectHolder> chain, bool global = true, params object[] arg)
    {
        await SpecificFullProc(proc, global, chain, arg);
    }

    protected virtual async Task SpecificFullProc(string proc, bool global, List<EffectHolder> chain, object[] arg)
    {
        if (global)
        { await GlobalProc(proc, chain, arg); }
        await Proc(proc, chain, arg);
    }

    public async Task SpecificFullDelayProc(List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await SpecificFullProc(proc, true, chain , arg);
        await action.Run();
    }

    public async Task SpecificFullDelayProc(List<EffectHolder> holders, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await SpecificFullProc(proc, true, chain, arg);
        arg = EffectsUtils.ObjectList(arg, 0, 0);

        for (int i = 0; i < holders.Count; i++)
        {
            arg[0] = i;
            holders[i]?.SpecificFullProc(proc, false, chain, arg);
        }

        await action.Run();
    }

    public async Task SpecificFullDelayProc(EffectHolder[] holders, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await SpecificFullProc(proc, true, chain, arg);
        arg = EffectsUtils.ObjectList(arg, 0, 0);

        for (int i = 0; i < holders.Length; i++)
        {
            arg[0] = i;
            holders[i]?.SpecificFullProc(proc, false, chain, arg);
        }

        await action.Run();
    }

    public static async Task StaticSpecificFullDelayProc(List<EffectHolder> holders, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        arg = EffectsUtils.ObjectList(arg, 0, 0);

        for (int i = 0; i < holders.Count; i++)
        {
            arg[0] = i;
            holders[i]?.SpecificFullProc(proc, false, chain, arg);
        }

        await action.Run();
    }

    public static async Task StaticSpecificFullDelayProc(EffectHolder[] holders, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        arg = EffectsUtils.ObjectList(arg, 0, 0);

        for (int i = 0; i < holders.Length; i++)
        {
            arg[0] = i;
            holders[i]?.SpecificFullProc(proc, false, chain, arg);
        }

        await action.Run();
    }

    public async Task SpecificFullDelayProc(TargetWrapper[] holders, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await SpecificFullProc(proc, true, chain, arg);
        arg = EffectsUtils.ObjectList(arg, 0, 0);

        for (int i = 0; i < holders.Length; i++)
        {
            arg[0] = i;
            holders[i]?.reserve?.SpecificFullProc(proc, false, chain, arg);
        }

        await action.Run();
    }

    public async Task SpecificFullDelayProc(HitWrapper[] holders, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await SpecificFullProc(proc, true, chain, arg);
        arg = EffectsUtils.ObjectList(arg, 0, 0);

        for (int i = 0; i < holders.Length; i++)
        {
            arg[0] = i;
            if (holders[i] != null && holders[i].wrapper != null)
            {
                foreach (DefensiveToken a in holders[i].wrapper.actions)
                {
                    a?.SpecificFullProc(proc, false, chain, arg);
                }    
            }    
        }
        await action.Run();
    }

    public async Task SpecificFullDelayProc(EffectHolder holder, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }
         
        await SpecificFullProc(proc, true, chain, arg);
        arg = EffectsUtils.ObjectList(arg, 0, 0);

        holder?.SpecificFullProc(proc, false, chain, arg);
        await action.Run();
    }

    public async Task SpecificFullDelayProc(GameEntity entity, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await SpecificFullProc(proc, true, chain, arg);
        arg = EffectsUtils.ObjectList(arg, 0, 0);
        entity?.Proc(proc, chain, arg);
        

        await action.Run();
    }


    public async Task SpecificFullDelayProc(List<GameEntity> entities, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await SpecificFullProc(proc, true, chain, arg);
        arg = EffectsUtils.ObjectList(arg, 0, 0);
        for (int i = 0; i < entities.Count; i++)
        {
            arg[0] = i;
            entities[i]?.Proc(proc,chain, arg);
        }

        await action.Run();
    }

    public async Task SpecificFullDelayProc(GameEntity[] entities, List<EffectHolder> chain, DelayAction action, int action_position = 0, params object[] arg)
    {
        string proc = action.procname;
        if (arg == null) { arg = new object[1] { action }; } else { arg = EffectsUtils.ObjectList(arg, action_position, action); }

        await SpecificFullProc(proc, true, chain, arg);
        arg = EffectsUtils.ObjectList(arg, 0, 0);
        for (int i = 0; i < entities.Length; i++)
        {
            arg[0] = i;
            entities[i]?.Proc(proc, chain, arg);
        }

        await action.Run();
    }

    protected async Task GlobalProc(string proc, List<EffectHolder> chain, object[] arg)
    {
        if (type != EffectHolderType.Level)
        {
            await ObjectUtils.LevelManager.Proc(proc, chain, arg);
        }
    }



 

    public async Task AddEffect(string effect, string condition, bool og, List<EffectHolder> chain)
    {

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
            eP.original = og;
            effects[proc].Add(eP);
        }
        

        await FullProc("effect_add", chain, eP);
    }

    public async Task AddEffectPreset(string effect, string condition, bool og, List<EffectHolder> chain)
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
            eP.original = og;
            effects[proc].Add(eP);
        }

       

        await FullProc("effect_add", chain, eP);
    }


    public async Task RemoveDefaultEffect(EffectPair effectPair, List<EffectHolder> chain)
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


        await FullProc("effect_remove", chain, effectPair);
        
    }
    public async Task RemoveEffect(EffectPair effectPair, List<EffectHolder> chain)
    {
        foreach (string key in effectPair.firstproc)
        {
            if (effects[key] != null)
            {
                if (effectPair.defaultEffect != true && effectPair.original != true)
                {
                    effects[key].Remove(effectPair);
                    if (effects[key].Count <= 0)
                    {
                        effects[key] = null;
                    }
                }
            }
        }

        await FullProc("effect_remove", chain, effectPair);
    }

    public async Task RemoveOriginalEffect(EffectPair effectPair, List<EffectHolder> chain)
    {
        foreach (string key in effectPair.firstproc)
        {
            if (effects[key] != null)
            {
                if (effectPair.defaultEffect != true)
                {
                    effects[key].Remove(effectPair);
                    if (effects[key].Count <= 0)
                    {
                        effects[key] = null;
                    }
                }
            }
        }

        await FullProc("effect_remove", chain, effectPair);
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




