using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Timeline.Actions;



public class AttackWrapper : DelayAction
{
    public enum Phase
    {

        target,
        beforeattack,
        defense_get,
        roll,
        clash,
        resolve,
        finalroll,
        hit,
        end
    }
    static int number_of_phase = (int)Phase.end + 1;

    public List<EffectHolder> chain;
    public GameEntity[] targets;
    public TargetWrapper[] target_wrappers;
    public HitWrapper[] hit_wrappers;
    public OffensiveToken action;

    public List<int> left_to;


    public int roll_value = -1, minroll, maxroll;
    public int bonus_value = 0, bonus_damage_value = 0, general_damage_value = 0, total_damage_value = 0;
    public int final_clash_value = -1, bonus_clash_value = 0; ///for offensive token to interact with
    public float bonus_clash_mult = 1, bonus_mult = 1, bonus_damage_mult = 1;


    public Phase phase;
    public bool reroll = true;


    public AttackWrapper(OffensiveToken action, GameEntity[] targets, List<EffectHolder> chain)
    {
        this.procname = "attack";
        this.action = action;
        this.targets = targets;
        this.chain = chain;

        minroll = action.minRoll;
        maxroll = action.maxRoll;
    }
    private readonly int filler;
    public List<Pair<Func<DelayAction, Task>, int>>[] function_lists = new List<Pair<Func<DelayAction, Task>, int>>[number_of_phase-1];
    public override void AddFunc(Func<DelayAction, Task> func, int priority)
    {
        priority = (int)MathF.Abs(priority);
        int layer = (int)MathF.Floor(priority / filler);
        List<Pair<Func<DelayAction, Task>, int>> function_list;

        if (layer <= 0)
        {
            function_list = this.function_list;
        }
        else
        {
            if (function_lists[layer - 1] == null)
            {
                function_lists[layer - 1] = new();
            }
            function_list = function_lists[layer - 1];
        }

        priority = priority - (filler * layer);

        if (function_list.Count == 0 || function_list.Last().value <= priority)
        {
            function_list.Add(new Pair<Func<DelayAction, Task>, int>(func, priority));
        }
        else
        {
            for (int i = 0; i < function_list.Count; i++)
            {
                if (function_list[i].value > priority)
                {
                    function_list.Insert(i, new Pair<Func<DelayAction, Task>, int>(func, priority));
                }
            }
        }
    }

    public void AddFunc(Func<DelayAction, Task> func, int priority, Phase phase)
    {
        priority = priority + (MathF.Sign(priority) * filler * (int)phase);
        AddFunc(func, priority);
    }

    

   

    public override async Task Run()
    {
        int phase_ = (int)this.phase;
        List<Pair<Func<DelayAction, Task>, int>> function_list;
        if (phase_ <= 0)
        {
            function_list = this.function_list;
        }
        else
        {
            if (function_lists[phase_ - 1] == null)
            {
                function_lists[phase_ - 1] = new();
            }
            function_list = function_lists[phase_ - 1];
        }

        foreach (Pair<Func<DelayAction, Task>, int> fu in function_list)
        {
            await fu.key(this);
        }

        function_list.Clear();


        ran = await DefaultRun();
        
        
    }

    protected override async Task<bool> DefaultRun()
    {
        if (action == null)
        {

            return false;
        }
        TargetWrapper[] targets = target_wrappers;
        switch (phase)
        {
            case Phase.target:
                GameEntity[] a = this.targets;
                if (a == null || a.Length == 0 || a.All(x => x == null) )
                {
                    return false;
                }
                int b = a.Length;

                targets = new TargetWrapper[b];
                for (int i = 0; i < b; i++)
                {
                    targets[i] = new TargetWrapper(a[i]);
                }
                break;

            case Phase.defense_get:
                for (int i = 0; i < targets.Length; i++)
                {
                    if (targets[i].reserve != null && targets[i].exist == true)
                    {
                        targets[i].actions.Add(targets[i].reserve);
                    }
                    else
                    {
                        targets[i].reserve = null;
                        left_to.Remove(i);
                    }
                }
                break;
            case Phase.roll:
                foreach (int i in left_to)
                {
                    TargetWrapper targeted = targets[i];
                    if (targeted.reserve != null)
                    {
                        targeted.Roll(this);
                    }
                }
                Reset();
                Roll();

                break;
            case Phase.clash:
                Calculate_Clash();
                foreach (int i in left_to)
                {
                    TargetWrapper targeted = targets[i];
                    if (targeted.reserve != null)
                    {
                        targeted.Calculate_Clash();
                        targeted.Clash(this);
                    }
                }
                break;
            case Phase.resolve:
                foreach (int i in left_to)
                {
                    targets[i].Resolve();
                }
                break;
            case Phase.finalroll:
                Roll();
                general_damage_value = roll_value + (int)MathF.Floor((bonus_damage_value+bonus_value)*(bonus_damage_mult+bonus_mult));
                bool hitatall = false;
                for (int i = 0; i < hit_wrappers.Length; i++)
                {
                    if (hit_wrappers[i].hit == true)
                    { 
                        hitatall = true;
                        total_damage_value += await hit_wrappers[i].Hit(general_damage_value);
                    }    
                }
                if (!hitatall) { return false; }
                break;
            case Phase.hit:
                break;
           
            
        }
        return true;
    }


    public async Task Activate()
    {
        phase = Phase.target;
        await action.SpecificFullDelayProc(targets, chain, this);

        if (ran == false) { End(); return; }

        left_to = Enumerable.Range(0, target_wrappers.Length - 1).ToList();

        phase = Phase.beforeattack;
        await action.SpecificFullDelayProc(chain, this);

        if (ran == false) { End(); return; }


        while (left_to.Count > 0)
        {
            foreach (int num in left_to)
            {
                target_wrappers[num].GetDefend(this);
            }

            phase = Phase.target;
            await action.SpecificFullDelayProc(targets, chain, this);

            if (ran == false){ End(); return; }
       

            

            if (left_to.Count > 0)
            {
                phase = Phase.roll;
                await action.SpecificFullDelayProc(target_wrappers, chain, this);

                if (ran == false) { End(); return; }


                phase = Phase.clash;
                await action.SpecificFullDelayProc(target_wrappers, chain, this);

                if (ran == false) { End(); return; }


                phase = Phase.resolve;
                await action.SpecificFullDelayProc(target_wrappers, chain, this);

                if (ran == false) { End(); return; }
                   

            }

           
        }
        Reset();

        hit_wrappers = new HitWrapper[target_wrappers.Length];
        for (int i = 0; i < hit_wrappers.Length; i++)
        {
            hit_wrappers[i] = new(target_wrappers[i]);
        }

        phase = Phase.finalroll;
        await action.SpecificFullDelayProc(hit_wrappers, chain, this);

       

        phase = Phase.hit;
        await action.SpecificFullDelayProc(hit_wrappers, chain, this);

        End();
    }

    private async void End()
    {
        phase = Phase.end;
        await action.SpecificFullDelayProc(chain, this);
    }


    public void Reset()
    {
        bonus_value = 0;
        final_clash_value = -1; bonus_clash_value = 0;
        bonus_clash_mult = 1; bonus_mult = 1;
        general_damage_value = 0; total_damage_value = 0;
        bonus_damage_mult = 1;
        bonus_damage_value = 0;
    }

    public void Roll()
    {
        
        roll_value = Manager.number_seed.Next(Math.Max(0, minroll), Math.Max(0, maxroll));
    }

    public void Calculate_Clash()
    {
        float a = (bonus_clash_value + bonus_value) * (bonus_clash_mult + bonus_mult);
        final_clash_value = roll_value + (int)Math.Floor(a);
    }

    public void StopDefend(int target)
    {
        target_wrappers[target].reserve = null;
    }

  






}

public class HitWrapper
{
    public TargetWrapper wrapper;

    public bool hit;
    public int seperate_damage_value = 0;
    public float seperate_damage_mult = 0;
    public int damage = 0, damage_dealt = 0;

    public async Task<int> Hit(int general_damage)
    {
        damage = general_damage + (int)MathF.Floor(seperate_damage_mult*seperate_damage_value);
        damage_dealt = await wrapper.target.Damage(damage);
        return damage_dealt;
    }

    public HitWrapper(TargetWrapper wrapper)
    {
        this.wrapper = wrapper;
      

        if (wrapper != null)
        {
            hit = true;
            List<TargetWrapper.ClashResult> actions = wrapper.results;
            for (int i = actions.Count - 1; i >= 0; i--)
            {
                if (actions[i] == TargetWrapper.ClashResult.win)
                {
                    return;
                }    
                else if (actions[i] == TargetWrapper.ClashResult.lose || actions[i] == TargetWrapper.ClashResult.draw)
                {
                    hit = false;
                    return;
                }    
            }    
        }
        else
        {
            hit = false;
        }    
    }


}

public class TargetWrapper
{

    public TargetWrapper(GameEntity entity)
    {
        target = entity;

    }
    public enum ClashResult
    {
        win,
        lose,
        draw,
        reclash,
        noclash
    }

    public void GetDefend(AttackWrapper wrapper)
    {

        if (target != null)
        {
            reserve = target.defenses.FirstOrDefault();
            if (reserve != null)
            {
                minroll = reserve.minRoll;
                maxroll = reserve.maxRoll;
            }


            if (loop > 0)
            {
                exist = false;
            }
            else
            {
                exist = true;
            }
        }
    }

    public void Reset()
    {
        final_value = -1;
        bonus_value = 0;
        bonus_mult = 1;
        take_previous = true;
        remove = true;
    }
    public void Roll(AttackWrapper wrapper)
    {
        Reset();
        roll_value = Manager.number_seed.Next(Math.Max(0, minroll), Math.Max(0, maxroll));
    }

    public void Calculate_Clash()
    {
        float a = bonus_value * bonus_mult;
        final_value = roll_value + (int)Math.Floor(a);
    }

    public void Clash(AttackWrapper wrapper)
    {
        if (reserve.clash == false)
        {
            results.Add(ClashResult.noclash);
            final_def.Add(final_value);
            final_off.Add(-1);
        }
        else
        {
            int attacker_value;
            if (take_previous)
            {
                int c = TryTakePrevious();
                if (c != -1)
                {
                    attacker_value = c;
                }
                else
                {
                    attacker_value = wrapper.final_clash_value;
                }
            }
            else
            {
                attacker_value = wrapper.final_clash_value;
            }

            if (final_value > attacker_value)
            {
                results.Add(ClashResult.win);
            }
            else if (final_value < attacker_value)
            {
                results.Add(ClashResult.lose);
            }
            else
            {
                results.Add(ClashResult.draw);
            }

            final_def.Add(final_value);
            final_off.Add(attacker_value);
        }


    }

    private int TryTakePrevious()
    {
        if (final_off.Count > 0)
        {
            for (int i = final_off.Count - 1; i >= 0; i--)
            {
                if (final_off[i] >= 0)
                {
                    return final_off[i];
                }
            }
        }

        return -1;
    }

    public void Resolve()
    {
        if (remove)
        {
            loop += 1;
            target.defenses.Remove(reserve);
        }
        reserve = null;
    }

    public DefensiveToken reserve = null;
    public GameEntity target;
    public int roll_value = -1, minroll, maxroll;
    public int final_value = -1, bonus_value = 0;
    public float bonus_mult = 1;
    public bool take_previous = true;
    public bool remove = true, exist = true;
    public int loop = 0;


    public List<int> final_def = new(), final_off = new();
    public List<ClashResult> results = new();
    public List<DefensiveToken> actions = new();

}
