using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameEntity : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public List<string> tags = new();
    public string Oname;
    public Vector3Int position;
    public List<DefensiveAction> defenses = new();
    public int health {  get; protected set; }
    


    public virtual bool CheckStatic()
    {
        return false;
    }    

    protected virtual void Start()
    {

    }    

    // Update is called once per frame
    protected virtual void Update()
    {

    }

    public async Task Proc(string proc, List<EffectHolder> chain, object[] arg)
    {

    }
}

