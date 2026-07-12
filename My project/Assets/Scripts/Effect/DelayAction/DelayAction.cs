using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class DelayAction
{
    public string procname;
    
    public bool ran = false;


    protected async virtual Task<bool> DefaultRun()
    {
        return true;
    }




    public List<Pair<Func<DelayAction, Task>, int>> function_list = new();
    

    public virtual void AddFunc(Func<DelayAction, Task> func, int priority)
    {


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

    public virtual async Task Run()
    {

        bool tried_running = false;
        foreach (Pair<Func<DelayAction, Task>, int> fu in function_list)
        {
            if (fu.value < 0 || ran)
            {
                await fu.key(this);
            }
            else
            {
                tried_running = true;
                ran = await DefaultRun();
                
                if (!ran) { return;  }

                await fu.key(this);
            }
        }

        if (!tried_running)
        {
            ran = await DefaultRun();
        }
    }

    
}



public class Pair<T, N>
{
    public T key;
    public N value;

    public Pair(T key, N value)
    {
        this.key = key;
        this.value = value;
    }
}