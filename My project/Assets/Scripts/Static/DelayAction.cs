using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;

public class DelayAction
{
    public string name;
    public List<Pair<Func<DelayAction, Task>, int>> function_list = new();

    protected async virtual Task DefaultRun()
    {

    }

    public void AddFunc(Func<DelayAction, Task> func, int priority)
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

    public async Task Run()
    {
        bool ran = false;
        foreach(Pair < Func<DelayAction, Task>, int> fu in function_list)
        {
            if (fu.value < 0 || ran)
            {
                await fu.key(this);
            }
            else 
            {
                await DefaultRun();
            }    
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