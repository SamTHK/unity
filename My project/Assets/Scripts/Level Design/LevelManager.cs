using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

 
        playable = true;

    }

    public void Update()
    {
        if (init == true)
        {
            preset?.Update(this);


            PreviousMouseCell = MouseToCell();



            CheckActions();
        }
    }

    #region init
    [SerializeField] GameObject Opuddle, Ochoose, Orange;
    [SerializeField] TileBase tileBase;
    public Tilemap wallMap { get; protected set; }
    public Tilemap voidMap { get; protected set; }
    public Tilemap puddleMap { get; protected set; }

    public List<string> tags;
    public Vector3Int minBound, maxBound;
    public Puddle[,] puddleGrid;
    public GameEntity[,] entityGrid;
    public List<Puddle> puddles = new();
    public List<GameEntity> entities = new();

    public enum TileType
    {
        floor_,
        void_,
        barrier_,
        wall_,
            null_
    }

    public Tilemap floorMap { get; protected set; }
    public Tilemap barrierMap { get; protected set; }
    public Tilemap chooseMap { get; protected set; }
    public Tilemap rangeMap { get; protected set; }

    public GameObject grid { get; protected set; }
    public Grid gridComponent { get; protected set; }

    public LevelPreset preset;

    bool init;
    readonly Dictionary<string, List<EffectHolder>> Trigger = new();
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

    public async Task Proc(string trigger, List<EffectHolder> chain, object[] arg)
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
    public int turn, round;
 
    public Creature turn_of;
    public List<Creature> creature_list = new(); /// remember to add to entities as well
    public int turn_of_int = 0;
    public bool priority_disturbed = false; /// check if the a non-prioirty card had been played, for precheck, check in CardHolder instead

    public bool playable = true, running = false;
    public Card cardPlaying;
    public ActionToken actionDoing;
    public ChooseToken chooseDoing;
    public OnTurnPage currentPage;
    public List<Page> pages = new();
    public List<Page> previousPage = new();
    public List<Card> cardPlayed = new();

    private async void CheckActions()
    {
        if (!running)
        {
            if (pages.Count > 0)
            {
                running = true;
                playable = false;

                Page first = pages[0];
                if (first.CheckLeft())
                {
                    if (!first.OnTurnCheck())
                    {
                        DefensivePage newpage = (DefensivePage)first;

                        DefensiveToken a = newpage.actions[0];
                        newpage.actions.RemoveAt(0);

                        DefenseReservedAction DRa = new(this, a);
                        await a.SpecificFullDelayProc(null, DRa);

                    }
                    else
                    {
                        OnTurnPage newpage = (OnTurnPage)first;

                        OnTurnToken a = newpage.actions[0];
                        ChooseToken b = newpage.choose;
                        newpage.actions.RemoveAt(0);

                        if (b.result == null || a.forceRechoose)
                        {
                            await b.TryVisualize(this);
                        }

                        await a.Activate(this);
                    }
                }
                else
                {
                    previousPage.Add(first);
                    pages.RemoveAt(0);
                }

                running = false;
            }
            else
            {
                playable = true;
                running = false;
                previousPage.Clear();
                if (cardPlaying != null)
                {
                    cardPlaying.SpecificFullProc("cardend", null, true, cardPlaying);
                    cardPlayed.Add(cardPlaying);
                }
                cardPlaying = null;
            }
        }
    }

    

    public async Task<CardPlayAction> ParseActions(Card card, List<EffectHolder> chain)
    {
        cardPlaying = card;


        CardPlayAction parseAction = new(this, card, chain);

        await card.SpecificFullDelayProc(chain, parseAction, 1, card);




        playable = false;
        return parseAction;
    }


    public async Task AddAction(GameEntity gameEntity, EffectHolder holder, int action_index, OnTurnToken action, bool forcerechoose, List<EffectHolder> chain)
    {

        action.forceRechoose = forcerechoose;
        await AddAction(gameEntity, holder, action_index, action, chain);
    }

    public async Task AddAction(GameEntity gameEntity, EffectHolder holder, int action_index, OnTurnToken action, List<EffectHolder> chain)
    {
        action.player = gameEntity;
        action.holder = holder;
        action.chain = new(chain);
        AddActionAction aA = new(this, action, action_index);
        await holder.SpecificFullDelayProc(chain, aA, 1, holder);
        playable = false;
    }

    public async Task AddPage(GameEntity gameEntity, EffectHolder holder, int page_index, OnTurnPage page, List<EffectHolder> chain)
    {
        List<OnTurnToken> a = page.actions;

        for (int i = 0; i < a.Count; i++)
        {
            a[i].player = gameEntity;
            a[i].holder = holder;
            a[i].chain = new(chain);
        }
        AddPageAction aA = new(this, page, page_index);
        await holder.SpecificFullDelayProc(chain, aA, 1, holder);
        playable = false;
    }

    public async Task AddPage(GameEntity gameEntity, EffectHolder holder, int page_index, DefensivePage page, List<EffectHolder> chain)
    {
        List<DefensiveToken> a = page.actions;

        for (int i = 0; i < a.Count; i++)
        {
            a[i].player = gameEntity;
            a[i].holder = holder;
            a[i].chain = new(chain);
        }
        AddPageAction aA = new(this, page, page_index);
        await holder.SpecificFullDelayProc(chain, aA, 1, holder);
        playable = false;
    }

    public async Task EndTurn()
    {
        
        await turn_of?.EndTurn();    
        Proc("endturn", null, null);
        
        turn_of_int += 1;
        if (turn_of_int >= creature_list.Count)
        {
            EndRound();
        }
        turn_of = creature_list[turn_of_int];
        cardPlayed.Clear();
        priority_disturbed = false;
        await turn_of?.StartTurn();
    }    

    private async Task EndRound()
    {
        foreach (GameEntity e in entities)
        {
            await e.EndRound();
        }
        Proc("endround", null, null);
        turn_of_int = 0;

        creature_list = creature_list.OrderBy(x => x.speed).ToList();


    }    

    #endregion

    #region range

    public Vector3Int PreviousMouseCell { get; private set; }
    public int rotate, maxrotate;
    public List<Vector3Int> cellAvailable, cellChosen;
    public List<int> rotateChosen;




   

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
            (Mathf.Abs(d.x) == Mathf.Abs(d.y) && d.z == 0) ||
            (Mathf.Abs(d.x) == Mathf.Abs(d.z) && d.y == 0) ||
            (Mathf.Abs(d.y) == Mathf.Abs(d.z) && d.x == 0);
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

    public (Dictionary<Vector3Int, int>, Dictionary<Vector3Int, Vector3Int>, List<Vector3Int>) CellRangeObstacle(Vector3Int center, int N, ObstacleCheck obstacle)
    {
        List<Vector3Int>[] cell = new List<Vector3Int>[N + 1];
        List<Vector3Int> exclude = new();
        for (int o = 0; o < N + 1; o++)
        {
            cell[o] = new();
        }
        Dictionary<Vector3Int, Vector3Int> connections = new();
        Dictionary<Vector3Int, int> smallest_cost = new()
        {
            [center] = 0
        };
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
                results.Add(CubeToOffset(VectorAdd(center_cube, new Vector3Int(q, r, s))));
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
                results.Add(CubeToOffset(VectorAdd(center_cube, new Vector3Int(q, r, s))));
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

    public (List<Vector3Int>, List<Vector3Int>) CellLine(Vector3Int firstcell, Vector3Int secondcell, int overflow = 0, int piercing = 0, ObstacleCheck piercing_numbers = null)
    {
        float distance = CellDistance(firstcell, secondcell);

        List<Vector3Int> list = new();
        List<Vector3Int> exclude = new();

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

    public void CellLine(ref List<Vector3Int> list, ref List<Vector3Int> exclude, Vector3Int firstcell, Vector3Int secondcell, int overflow = 0, int piercing = 0, ObstacleCheck piercing_numbers = null)
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
        return type switch
        {
            TileType.floor_ => floorMap.HasTile(cell),
            TileType.wall_ => wallMap.HasTile(cell),
            TileType.void_ => voidMap.HasTile(cell),
            TileType.barrier_ => barrierMap.HasTile(cell),
            _ => false,
        };
    }

    public TileType HasTile(Vector3Int cell)
    {
        if (wallMap.HasTile(cell))
        {
            return TileType.wall_;
        }
        if (barrierMap.HasTile(cell))
        {
            return TileType.barrier_;
        }    
        if (floorMap.HasTile(cell))
        {
            return TileType.floor_;
        }    
        if (voidMap.HasTile(cell))
        {
            return TileType.void_;
        }    
        return TileType.null_;
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

      
    public int PierceCheck(Vector3Int cell, ObstacleCheck obstacle )
    {
        return 0;
        ///higher value = harder to walk on 


        /*int cost = 0;
        if (piercing_numbers != null)
        {

            switch (HasTile(cell))
            {
                case 
            }

            if (piercing_numbers.TryGetValue("floor", out int delete) && HasTile(cell, TileType.floor_))
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
           

                    if (piercing_numbers.TryGetValue("entity", out delete))
                    {
                        cost += delete;
                    }
                }
            }
            cost = Mathf.Max(0, cost);
        }

        return cost; /// remember to do this  */
    }



    private Vector3Int OffsetToCube(Vector3Int vec)
    {
        int col = vec.x;
        int row = vec.y;
        int parity = row & 1;
        int x = col - ((row - parity) / 2);
        int z = row;
        int y = -x - z;
        return new Vector3Int(x, z, y);
    }

    private Vector3Int CubeToOffset(Vector3Int vec)
    {
        var parity = vec.y & 1;
        var col = vec.x + ((vec.y - parity) / 2);
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

    private readonly Vector3Int[] cube_direction_vectors = { new(+1, 0, -1), new(+1, -1, 0), new(0, -1, +1), new(-1, 0, +1), new(-1, +1, 0), new(0, +1, -1) };
    private readonly Vector3Int[,] offset_direction_vectors = { { new Vector3Int(1, 0), new Vector3Int(0, -1), new Vector3Int(-1, -1), new Vector3Int(-1, 0), new Vector3Int(-1, 1), new Vector3Int(0, +1) }, { new Vector3Int(1, 0, 0), new Vector3Int(1, -1, 0), new Vector3Int(0, -1, 0), new Vector3Int(-1, 0, 0), new Vector3Int(0, 1, 0), new Vector3Int(1, 1, 0) } };

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

