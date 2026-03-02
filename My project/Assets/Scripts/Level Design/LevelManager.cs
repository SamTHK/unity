using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class LevelManager : MonoBehaviour
{
    public InputAction pointAction, interactAction, cancelAction, bigcancelAction, biginteractAction, rotateAction;
    public void Awake()
    {
        pointAction = InputSystem.actions.FindAction("Point");
        interactAction = InputSystem.actions.FindAction("Click");
        cancelAction = InputSystem.actions.FindAction("RightClick");
        bigcancelAction = InputSystem.actions.FindAction("Escape");
        rotateAction = InputSystem.actions.FindAction("Rotate");
        biginteractAction = InputSystem.actions.FindAction("Enter");

        default_walk["wall"] = 999;
        default_walk["barrier"] = 999;
        default_walk["entity"] = 999;
        default_walk["tiny"] = -999;

        default_bullet["wall"] = 1;
        default_bullet["entity"] = 1;
        default_walk["tiny"] = -1;
        playable = true;

    }

    public void Update()
    {
        if (init == true)
        {
            if (preset != null)
            {
                preset.Update(this);
            }


            PreviousMouseCell = MouseToCell();



            TempTest();
        }
    }

    #region init
    [SerializeField] public GameObject Opuddle, Ochoose, Orange;
    [SerializeField] TileBase tileBase;
    public static Dictionary<string, int> default_walk = new();
    public static Dictionary<string, int> default_bullet = new();
    public Tilemap wallMap { get; protected set; }
    public Tilemap voidMap { get; protected set; }
    public Tilemap puddleMap { get; protected set; }

    public List<string> tags;
    public Vector3Int minBound, maxBound;
    public Puddle[,] puddleGrid;
    public GameEntity[,] entityGrid;

    public enum TileType
    {
        floor_,
        void_,
        barrier_,
        wall_
    }

    public Tilemap floorMap { get; protected set; }
    public Tilemap barrierMap { get; protected set; }
    public Tilemap chooseMap { get; protected set; }
    public Tilemap rangeMap { get; protected set; }

    public GameObject grid { get; protected set; }
    public Grid gridComponent { get; protected set; }

    public LevelPreset preset;

    bool init;
    Dictionary<string, List<EffectHolder>> Trigger;
    public EffectHolder effect = new();

    public async void Init(LevelPreset preset = null)
    {
        grid = GameObject.Find("Grid");
        gridComponent = grid.GetComponent<Grid>();

        GameObject oP = Instantiate(Opuddle, grid.transform);
        puddleMap = oP.GetComponent<Tilemap>();

        GameObject oC = Instantiate(Ochoose, grid.transform);
        chooseMap = oC.GetComponent<Tilemap>();

        GameObject oR = Instantiate(Orange, grid.transform);
        rangeMap = oR.GetComponent<Tilemap>();


        wallMap = GameObject.Find("Wall").GetComponent<Tilemap>();
        floorMap = GameObject.Find("Floor").GetComponent<Tilemap>();
        voidMap = GameObject.Find("Void").GetComponent<Tilemap>();
        barrierMap = GameObject.Find("Barrier").GetComponent<Tilemap>();

        wallMap.CompressBounds();
        floorMap.CompressBounds();
        voidMap.CompressBounds();
        barrierMap.CompressBounds();

        minBound = new Vector3Int(Mathf.Min(wallMap.cellBounds.xMin, floorMap.cellBounds.xMin, voidMap.cellBounds.xMin, barrierMap.cellBounds.xMin), Mathf.Min(wallMap.cellBounds.yMin, floorMap.cellBounds.yMin, voidMap.cellBounds.yMin, barrierMap.cellBounds.yMin));
        maxBound = new Vector3Int(Mathf.Max(wallMap.cellBounds.xMax, floorMap.cellBounds.xMax, voidMap.cellBounds.xMax, barrierMap.cellBounds.xMax), Mathf.Max(wallMap.cellBounds.yMax, floorMap.cellBounds.yMax, voidMap.cellBounds.yMax, barrierMap.cellBounds.yMax));

        entityGrid = new GameEntity[maxBound.x - minBound.x, maxBound.y - minBound.y];
        puddleGrid = new Puddle[maxBound.x - minBound.x, maxBound.y - minBound.y];

        Debug.Log(new Vector3Int(maxBound.x - minBound.x, maxBound.y - minBound.y));
        if (preset != null)
        {
            this.preset = preset;
            this.tags = preset.tags;
            preset.Init(this);
        }

        init = true;
    }



    #endregion

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
            if (Trigger[trigger].Count <= 0)
            {
                Trigger.Remove(trigger);
            }
        }
    }

    public async Task Proc(string trigger, List<EffectPair> chain, List<object> arg)
    {
        await effect.Proc(trigger, chain, arg);
        if (Trigger[trigger] != null)
        {
            for (int i = 0; i < Trigger[trigger].Count; i++)
            {
                await Trigger[trigger][i].Proc(trigger, chain, arg);
            }
        }
    }
    #endregion

    #region action
    public bool returnable, playable;
    public Card cardPlaying;
    public ActionToken actionDoing;
    public ChooseToken chooseDoing;
    public List<ActionToken> ActionTokens;
    public List<ChooseToken> ChooseTokens;

    private void CheckActions()
    {
        if (chooseDoing == null)
        { 
            if (ActionTokens.Count > 0)
            {
                ActionToken first = ActionTokens[0];
                if (ActionTokens[0] is OnTurnAction)
                {
                    OnTurnAction oTA = (OnTurnAction)first;
                    if ( oTA.chooseTokenUsage == -1)
                    {
                        oTA.Activate(this);
                    }
                    else if (ChooseTokens[oTA.chooseTokenUsage].result != null || oTA.forceRechoose)
                    {
                        chooseDoing = ChooseTokens[oTA.chooseTokenUsage];
                        if (cardPlaying != null)
                        {
                            chooseDoing.center = cardPlaying.player.position;
                        }    
                        chooseDoing.Visualize(this);
                    }    
                }
                else
                {
                    first.Activate(this);
                    returnable = false;
                }    
            }
            else if (!playable)
            {
                chooseDoing = null;
                returnable = true;
                ChooseTokens.Clear();
                playable = true;
                if (cardPlaying != null)
                {
                    cardPlaying.CardFullProc("cardend", null, new List<object>() { cardPlaying });
                    cardPlaying = null;
                }
                
            }
        }
    }    

    public async void ParseActions(Card card, List<EffectPair> chain, params object[] arg)
    {
        cardPlaying = card;
        ActionTokens.Clear();
        ChooseTokens.Clear();


        ParseAction parseAction = new(this, card.actions, card.choices);
        List<object> o = EffectsUtils.ObjectList(arg, card);
        await card.CardFullDelayProc("parseaction", chain, parseAction, 1, o);
        

        if (ActionTokens.Count > 0)
        {
            actionDoing = null;
            chooseDoing = null;
        }

        playable = false;
        returnable = true;
    }

    public void AddAction(int action_index, ActionToken action)
    {
        playable = false;
        returnable = true;
        if (action_index >= 0)
        {
            ActionTokens.Insert(action_index, action);

        }
        else
        {
            int i = ActionTokens.Count + 1 - action_index;
            if (i >= 0)
            {
                ActionTokens.Insert(i, action);
            }
        }
      
    }

    

    #endregion

    #region range

    public Vector3Int PreviousMouseCell { get; private set; }
    public int rotate, maxrotate;
    public List<Vector3Int> cellAvailable, cellChosen;
    public List<int> rotateChosen;




    public void TempTest()
    {
        chooseMap.ClearAllTiles();


    }

    public Vector3Int CellRelative(Vector3Int cell)
    {
        return new Vector3Int(cell.x - minBound.x, cell.y - minBound.y);
    }

    public (int x, int y) CellRelativeInt(Vector3Int cell)
    {
        return (cell.x - minBound.x, cell.y - minBound.y);
    }

    public bool IsInStraightLine(Vector3Int center, Vector3Int cell)
    {
        Vector3Int a = OffsetToCube(center);
        Vector3Int b = OffsetToCube(cell);

        var d = b - a;
        int len = (Mathf.Abs(d.x) + Mathf.Abs(d.y) + Mathf.Abs(d.z)) / 2;

        if (len == 0) return true;

        // Check if moving along the line keeps exactly one coord at 0 difference after scaling
        return
            d.x == 0 || d.y == 0 || d.z == 0 ||                     // short lines / cardinal
            (d.x == d.y * -1 && d.z == 0) ||                        // one diagonal type
            (d.x == d.z * -1 && d.y == 0) ||
            (d.y == d.z * -1 && d.x == 0) ||
            // For longer lines — better to normalize:
            Mathf.Abs(d.x) == Mathf.Abs(d.y) && d.z == 0 ||
            Mathf.Abs(d.x) == Mathf.Abs(d.z) && d.y == 0 ||
            Mathf.Abs(d.y) == Mathf.Abs(d.z) && d.x == 0;
    }

    
    
    public Vector3Int MouseToCell()
    {
        Vector2 pointValue = pointAction.ReadValue<Vector2>();
        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(pointValue);
        return gridComponent.WorldToCell(mouseWorld);
    }

    public int CellDistance(Vector3Int firstcell, Vector3Int secondcell)
    {
        Vector3Int a = OffsetToCube(firstcell);
        Vector3Int b = OffsetToCube(secondcell);


        float dx = Mathf.Abs(a.x - b.x);
        float dy = Mathf.Abs(a.y - b.y);
        float dz = Mathf.Abs(a.z - b.z);

        return (int)Mathf.Floor((dx + dy + dz) / 2);
    }

    public (Dictionary<Vector3Int, int>, Dictionary<Vector3Int, Vector3Int>, List<Vector3Int>) CellRangeObstacle(Vector3Int center, int N, Dictionary<string, int> obstacle)
    {
        List<Vector3Int>[] cell = new List<Vector3Int>[N + 1];
        List<Vector3Int> exclude = new();
        for (int o = 0; o < N + 1; o++)
        {
            cell[o] = new();
        }
        Dictionary<Vector3Int, Vector3Int> connections = new();
        Dictionary<Vector3Int, int> smallest_cost = new Dictionary<Vector3Int, int>();
        smallest_cost[center] = 0;
        cell[0].Add(center);

        for (int i = 0; i < N; i++)
        {
            foreach (Vector3Int vec in cell[i])
            {
                for (int j = 0; j < 6; j++)
                {
                    Vector3Int nei = OffsetNeighbor(vec, j);
                    int movement_cost = i + 1 + PierceCheck(nei, obstacle);

                    if (movement_cost < N + 1)
                    {
                        if (smallest_cost.TryGetValue(nei, out int small))
                        {
                            if (small > movement_cost)
                            {
                                cell[small].Remove(nei);
                                connections[nei] = vec;
                                smallest_cost[nei] = movement_cost;
                                cell[movement_cost].Add(nei);
                            }
                        }
                        else
                        {

                            smallest_cost[nei] = movement_cost;
                            connections[nei] = vec;
                            cell[movement_cost].Add(nei);


                            if (CheckEntity(nei))
                            {
                                exclude.Add(nei);
                            }
                        }
                    }

                }
            }
        }
        return (smallest_cost, connections, exclude);
    }

    public List<Vector3Int> MapCreate(Dictionary<Vector3Int, int> Dic, List<Vector3Int> exclude)
    {
        List<Vector3Int> result = new();

        foreach (Vector3Int vec in Dic.Keys)
        {
            if (!exclude.Contains(vec))
            { result.Add(vec); }
        }

        return result;
    }

    public List<Vector3Int> ConnectPath(Vector3Int vec, Dictionary<Vector3Int, Vector3Int> connections)
    {
        List<Vector3Int> result = new() { vec };
        while (connections.TryGetValue(vec, out Vector3Int new_val))
        {
            result.Add(new_val);
            vec = new_val;
        }
        return result;
    }




    public List<Vector3Int> CellRange(Vector3Int center, int N)
    {
        List<Vector3Int> results = new();

        Vector3Int center_cube = OffsetToCube(center);
        for (int q = -N; q <= N; q++)
        {
            for (int r = Mathf.Max(-N, -q - N); r <= Mathf.Min(+N, -q + N); r++)
            {
                var s = -q - r;
                results.Add(CubeToOffset((VectorAdd(center_cube, new Vector3Int(q, r, s)))));
            }
        }
        return results;
    }

    public void CellRange(ref List<Vector3Int> results, Vector3Int center, int N)
    {

        Vector3Int center_cube = OffsetToCube(center);
        for (int q = -N; q <= N; q++)
        {
            for (int r = Mathf.Max(-N, -q - N); r <= Mathf.Min(+N, -q + N); r++)
            {
                var s = -q - r;
                results.Add(CubeToOffset((VectorAdd(center_cube, new Vector3Int(q, r, s)))));
            }
        }

    }

    public void CellDrawChoose(List<Vector3Int> choose_cells, Color color)
    {

        foreach (Vector3Int vec in choose_cells)
        {
            chooseMap.SetTile(vec, tileBase);
            chooseMap.SetColor(vec, color);
        }
    }

    public void CellDrawRange(List<Vector3Int> choose_cells, Color color)
    {

        foreach (Vector3Int vec in choose_cells)
        {
            rangeMap.SetTile(vec, tileBase);
            rangeMap.SetColor(vec, color);
        }
    }

    public (List<Vector3Int>, List<Vector3Int>) CellLine(Vector3Int firstcell, Vector3Int secondcell, int overflow = 0, int piercing = 0, Dictionary<string, int> piercing_numbers = null)
    {
        float distance = CellDistance(firstcell, secondcell);

        List<Vector3Int> list = new List<Vector3Int>();
        List<Vector3Int> exclude = new List<Vector3Int>();

        Vector3 firstpos = gridComponent.CellToWorld(firstcell);
        Vector3 secondpos = gridComponent.CellToWorld(secondcell);
        for (int i = 0; i <= distance + overflow; i++)
        {
            Vector3Int cell = gridComponent.WorldToCell(LerpVector3(firstpos, secondpos, (float)(1.0 / distance * i)));
            if (piercing_numbers != null)
            {
                piercing -= PierceCheck(cell, piercing_numbers);
            }
            if (piercing < 0)
            {
                break;
            }
            else
            {
                if (CheckEntity(cell) != null)
                {
                    exclude.Add(cell);
                }
                list.Add(cell);

            }
        }
        return (list, exclude);
    }

    public void CellLine(ref List<Vector3Int> list, ref List<Vector3Int> exclude, Vector3Int firstcell, Vector3Int secondcell, int overflow = 0, int piercing = 0, Dictionary<string, int> piercing_numbers = null, bool remove_entity = true)
    {
        float distance = CellDistance(firstcell, secondcell);

        Vector3 firstpos = gridComponent.CellToWorld(firstcell);
        Vector3 secondpos = gridComponent.CellToWorld(secondcell);
        for (int i = 0; i <= distance + overflow; i++)
        {
            Vector3Int cell = gridComponent.WorldToCell(LerpVector3(firstpos, secondpos, (float)(1.0 / distance * i)));
            if (piercing_numbers != null)
            {
                piercing -= PierceCheck(cell, piercing_numbers);
            }
            if (piercing < 0)
            {
                break;
            }
            else
            {
                if (CheckEntity(cell) != null)
                {
                    exclude.Add(cell);
                }
                list.Add(cell);
            }
        }

    }

    public List<Vector3Int> CellRing(Vector3Int center, int radius)
    {
        Vector3Int cube_center = OffsetToCube(center);
        List<Vector3Int> results = new();

        var hex = VectorAdd(cube_center, VectorScale(CubeDirection(4), radius));

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                results.Add(CubeToOffset(hex));

                hex = CubeNeighbor(hex, i);
            }
        }
        return results;
    }

    public void CellRing(ref List<Vector3Int> results, Vector3Int center, int radius)
    {
        Vector3Int cube_center = OffsetToCube(center);

        var hex = VectorAdd(cube_center, VectorScale(CubeDirection(4), radius));

        for (int i = 0; i < 6; i++)
        {
            for (int j = 0; j < radius; j++)
            {
                results.Add(CubeToOffset(hex));

                hex = CubeNeighbor(hex, i);
            }
        }
    }

    public bool HasTile(Vector3Int cell, TileType type)
    {
        switch (type)
        {
            case TileType.floor_:
                return floorMap.HasTile(cell);
            case TileType.wall_:
                return wallMap.HasTile(cell);
            case TileType.void_:
                return voidMap.HasTile(cell);
            case TileType.barrier_:
                return barrierMap.HasTile(cell);
        }
        return false;
    }

    public Puddle CheckPuddle(Vector3Int cell)
    {
        (int x, int y) = CellRelativeInt(cell);
        return puddleGrid[x, y];
    }

    public GameEntity CheckEntity(Vector3Int cell)
    {
        (int x, int y) = CellRelativeInt(cell);
        return entityGrid[x, y];
    }

    public int PierceCheck(Vector3Int cell, Dictionary<string, int> piercing_numbers)
    {
        int cost = 0;
        if (piercing_numbers != null)
        {

            int delete = 0;
            if (piercing_numbers.TryGetValue("floor", out delete) && HasTile(cell, TileType.floor_))
            {
                cost += delete;
            }
            if (piercing_numbers.TryGetValue("wall", out delete) && HasTile(cell, TileType.wall_))
            {
                cost += delete;
            }
            if (piercing_numbers.TryGetValue("void", out delete) && HasTile(cell, TileType.void_))
            {
                cost = 9999;
            }
            if (piercing_numbers.TryGetValue("barrier", out delete) && HasTile(cell, TileType.barrier_))
            {
                cost += delete;
            }

            Puddle pud = CheckPuddle(cell);
            if (pud != null)
            {
                if (piercing_numbers.TryGetValue(pud.Oname, out delete))
                {
                    cost += delete;
                }
                else
                {
                    cost += pud.slow;
                }
            }

            GameEntity en = CheckEntity(cell);
            if (en != null)
            {
                foreach (string ta in en.tags)
                {
                    if (piercing_numbers.TryGetValue(ta, out delete))
                    {
                        cost += delete;
                    }

                    if (piercing_numbers.TryGetValue("entity", out delete))
                    {
                        cost += delete;
                    }
                }
            }
            cost = Mathf.Max(0, cost);
        }

        return cost; /// remember to do this
    }



    private Vector3Int OffsetToCube(Vector3Int vec)
    {
        int col = vec.x;
        int row = vec.y;
        int parity = row & 1;
        int x = col - (row - parity) / 2;
        int z = row;
        int y = -x - z;
        return new Vector3Int(x, z, y);
    }

    private Vector3Int CubeToOffset(Vector3Int vec)
    {
        var parity = vec.y & 1;
        var col = vec.x + (vec.y - parity) / 2;
        var row = vec.y;
        return new Vector3Int(col, row);
    }


    private Vector3 LerpVector3(Vector3 a, Vector3 b, float t)
    {
        return new Vector3(Mathf.Lerp(a.x, b.x, t),
            Mathf.Lerp(a.y, b.y, t),
            Mathf.Lerp(a.z, b.z, t));
    }



    public Vector3Int VectorScale(Vector3Int hex, int factor)
    {
        return new Vector3Int(hex.x * factor, hex.y * factor, hex.z * factor);
    }

    int[,] matrix = { { 1, 2, 3 }, { 4, 5, 6 } };

    private Vector3Int[] cube_direction_vectors = { new Vector3Int(+1, 0, -1), new Vector3Int(+1, -1, 0), new Vector3Int(0, -1, +1), new Vector3Int(-1, 0, +1), new Vector3Int(-1, +1, 0), new Vector3Int(0, +1, -1) };
    private Vector3Int[,] offset_direction_vectors = { { new Vector3Int(1, 0), new Vector3Int(0, -1), new Vector3Int(-1, -1), new Vector3Int(-1, 0), new Vector3Int(-1, 1), new Vector3Int(0, +1) }, { new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0) } };

    public Vector3Int VectorAdd(Vector3Int hex, Vector3Int vec)
    {
        return new Vector3Int(hex.x + vec.x, hex.y + vec.y, hex.z + vec.z);
    }
    public Vector3Int CubeNeighbor(Vector3Int cube, int direction)
    {
        return VectorAdd(cube, CubeDirection(direction));
    }
    public Vector3Int CubeDirection(int direction)
    { return cube_direction_vectors[direction]; }

    public Vector3Int OffsetNeighbor(Vector3Int hex, int direction)
    {
        int parity = hex.y & 1;
        Vector3Int diff = offset_direction_vectors[parity, direction];
        return new Vector3Int(hex.x + diff.x, hex.y + diff.y);
    }

    #endregion
}

public class CellBase
{
    public int x, y, hcost, fcost, gcost;

    public CellBase(Vector3Int coord)
    {
        x = coord.x;
        y = coord.y;
    }

    public CellBase(int x, int y)
    {
        this.x = x;
        this.y = y;
    }
}