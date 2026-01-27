using CodeMonkey.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(Grid))]
public class LevelManager : MonoBehaviour
{

    
    [SerializeField] public GameObject Opuddle, Ochoose, Orange;
    public Tilemap wallMap { get; private set; }
    public Tilemap voidMap { get; private set; }
    public Tilemap puddleMap { get; private set; }

    public List<string> tags;

    public Tilemap floorMap { get; private set; }
    public Tilemap barrierMap { get; private set; }
    public Tilemap chooseMap { get; private set; } 
    public Tilemap rangeMap { get; private set; } 
    public Tilemap beaconMap { get; private set; }

    public List<Vector3Int> beaconPoints { get; private set; }
   
    public GameObject grid { get; private set; }

    public LevelPreset preset;

    Dictionary<string, List<EffectHolder>> Trigger;
    public EffectHolder effect = new();

    public async void Init(LevelPreset preset = null)
    {
        grid = GameObject.Find("Grid");

        GameObject oP = Instantiate(Opuddle, grid.transform);
        puddleMap = oP.GetComponent<Tilemap>();

        GameObject oC = Instantiate(Ochoose, grid.transform);
        chooseMap = oC.GetComponent<Tilemap>();

        GameObject oR = Instantiate(Orange, grid.transform);
        rangeMap = oR.GetComponent<Tilemap>();

        beaconMap = GameObject.Find("Beacon").GetComponent<Tilemap>();
        wallMap = GameObject.Find("Wall").GetComponent<Tilemap>();
        floorMap = GameObject.Find("Floor").GetComponent<Tilemap>();
        voidMap = GameObject.Find("Void").GetComponent<Tilemap>();
        barrierMap = GameObject.Find("Barrier").GetComponent<Tilemap>();

       
        beaconMap.CompressBounds();
        BoundsInt bounds = beaconMap.cellBounds;
        beaconPoints = new List<Vector3Int>();
        for (int y = bounds.min.y; y <= bounds.max.y; y++)
        {
            for (int x = bounds.min.x; x <= bounds.max.x; x++)
            {
                Vector3Int i = new Vector3Int(x, y, 0);
           
                if (beaconMap.HasTile(i))
                {
                    beaconPoints.Add(i);
                    Debug.Log(i);
                }
            }
        }

        if (preset != null)
        {
            this.preset = preset;
            this.tags = preset.tags;
            preset.Init(this);
            ///remember to proc start here
        }
    }

    public void Start()
    {
        
    }

    public void Update()
    {
        if ( preset != null )
        {
            preset.Update(this);
        }
        
    }

    #region trigger
    public void AddTrigger(string trigger, EffectHolder holder)
    {
        if (Trigger[trigger] == null)
        {

            Trigger[trigger] = new()
            {
                holder
            };
        }
        else if (!Trigger[trigger].Contains(holder))
        {
            Trigger[trigger].Add(holder);
        }
    }    

    public void RemoveTrigger(string trigger, EffectHolder holder)
    {
        if (Trigger[trigger] != null && Trigger[trigger].Contains(holder))
        {
            Trigger[trigger].Remove(holder);
            if (Trigger[trigger].Count <= 0 )
            {
                Trigger.Remove(trigger);
            }    
        }
    }    

    public void Proc(string trigger, List<EffectPair> chain, List<object> arg)
        {
        effect.Proc(trigger, chain, arg);
            if (Trigger[trigger] != null)
        {
            for (int i = 0; i < Trigger[trigger].Count; i++)
            {
                Trigger[trigger][i].Proc(trigger, chain, arg);
            }
        }
    }
    #endregion
}

