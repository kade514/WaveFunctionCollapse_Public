using GD.MinMaxSlider;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GridGen : MonoBehaviour
{
    public int GridHeight;
    public int GridWidth;
    public List<TileDetails> AllTileObjects;
    public Dictionary<Coordinates, TileController> TileDictionary;
    public bool GetIsGenerating() => _isGenerating;

    private List<Coordinates> _tilesToIterateOn;
    private bool _isGenerating;

    public struct Coordinates
    {
        public int X;
        public int Y;

        public Coordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"({X},{Y})";
        }
    }

    [System.Serializable]
    public struct TileDetails
    {
        public TileSO TileSO;

        [MinMaxSlider(0, 100)]
        public Vector2Int SpawnChance;
    }

    // Start is called before the first frame update
    void Start()
    {
        _tilesToIterateOn = new List<Coordinates>();
    }

    public void SetTileDebug(bool state)
    {
        foreach (var kvp in TileDictionary)
        {
            kvp.Value.Debug = state;
        }
    }

    [ContextMenu("Generate Grid and Set Tiles")]
    public void GenerateGridAndSetTiles()
    {
        if (!Application.isPlaying)
            return;

        StartCoroutine(coro());

        IEnumerator coro()
        {
            ClearCells();
            _isGenerating = true;
            yield return HexSpawn();
            yield return AutoSetTiles();

            foreach (var tile in TileDictionary)
            { 
                tile.Value.UpdatePosition();
            }

            _isGenerating = false;

            var moduleList = GetComponents<GeneratorModule>().ToList();

            foreach (var module in moduleList)
            {
                print($"Running {module.name}");

                yield return module.Run();

                print($"Finished {module.name}");
            }

            foreach (var keyValuePair in TileDictionary)
            {
                keyValuePair.Value.UpdateTileDetails(true);
            }
        }
    }
    
    public IEnumerator HexSpawn()
    {
        var typeList = AllTileObjects.Select(option => option.TileSO.Tile.TileType).ToList();
        
        for (var y = 0; y < GridHeight; y++)
        {
            yield return new WaitForEndOfFrame();

            for (var x = 0; x < GridWidth; x++)
            {
                var coords = new Coordinates
                {
                    Y = y,
                    X = x
                };

                var tile = HexPool.Instance.GetPoolableObject();
                tile.transform.SetParent(transform);
                tile.name = $"({coords.X},{coords.Y})";
                tile.Init(coords, typeList, this);
                tile.transform.localEulerAngles = Vector3.zero;
                tile.transform.position = tile.Hex.Position();
                tile.gameObject.SetActive(true);

                TileDictionary.Add(coords, tile);
            }
        }
    }

    public void ClearCells()
    {
        if (TileDictionary == null)
        {
            TileDictionary = new Dictionary<Coordinates, TileController>();
            return;
        }

        foreach (var kvp in TileDictionary)
        {
            HexPool.Instance.ReturnPoolableObject(kvp.Value);
        }

        TileDictionary.Clear();
    }

    public IEnumerator AutoSetTiles()
    {
        if (TileDictionary == null || TileDictionary.Count <= 0)
            yield break;

        var anyTileUndefined = TileDictionary.Any(x => x.Value.TileDetails.TileType == ETileType.Undefined);
        
        var count = 0;
        while (anyTileUndefined)
        {
            DefineTiles();
            count++;

            if (count >= 20)
            {
                count = 0;
                yield return new WaitForEndOfFrame();
            }
            anyTileUndefined = TileDictionary.Any(x => x.Value.TileDetails.TileType == ETileType.Undefined);
        }
    }

    public void DefineTiles(bool propagate = true)
    {
        if(TileDictionary is not { Count: > 0 })
            return;

        var randomCoord = new Coordinates
        {
            X = Random.Range(0, GridWidth),
            Y = Random.Range(0, GridHeight)
        };

        var attempts = 0;
        TileController centreTile = null;
        
        do
        {
            centreTile = TileDictionary[randomCoord];
            attempts++;

        } while (attempts < 100 && centreTile.TileDetails.TileType != ETileType.Undefined);

        if (centreTile.TileDetails.TileType != ETileType.Undefined)
        {
            centreTile = TileDictionary.FirstOrDefault(x => x.Value.TileDetails.TileType == ETileType.Undefined).Value;
        }

        if (centreTile == null || centreTile.TileDetails.TileType != ETileType.Undefined)
            return;

        var options = AllTileObjects.Where(x => centreTile.TileDetails.AvailableTypes.Contains(x.TileSO.Tile.TileType))
            .ToList();
        
        var randNum = Random.Range(0, 100);

        for (var i = 0; i < options.Count; i++)
        {
            var option = options[i];

            if (randNum >= option.SpawnChance.x && randNum <= option.SpawnChance.y)
            {
                centreTile.DefineTile(options[i].TileSO.Tile);
                break;
            }
        }

        if (propagate)
            Propagate(centreTile.Coords);
    }

    //propagates the changes to the tiles through the grid as far as it can
    private void Propagate(Coordinates coords)
    {
        _tilesToIterateOn.Add(coords);

        while (_tilesToIterateOn.Count > 0)
        {
            var tileCoords = _tilesToIterateOn[^1];
            var tile = TileDictionary[tileCoords];
            _tilesToIterateOn.Remove(tileCoords);

            //if tile reduced to one possibility, collapse it to whatever that possibility is.
            if (tile.TileDetails.TileType == ETileType.Undefined && tile.TileDetails.AvailableTypes.Count == 1)
            {
                tile.DefineTile(AllTileObjects.First(x => x.TileSO.Tile.TileType == tile.TileDetails.AvailableTypes[0]).TileSO.Tile);
            }

            //was going to change this to respect wrapping, but I think it's fine to leave it disabled as
            //I don't really care if the system thinks it has a couple extra neighbours at the edges
            var neighbours = GetNeighbours(tileCoords, 1, false);

            var curCellPossibles = tile.TileDetails.AcceptableNeighbours.ToList();

            //update each neighbouring tile's possible states to account for whatever state the current tile collapsed to. 
            foreach (var surroundingCell in neighbours)
            {
                var neighoursPossibleOptions = surroundingCell.TileDetails.AvailableTypes.ToList();
                var addToList = false;

                for (var i = neighoursPossibleOptions.Count - 1; i >= 0; i--)
                {
                    var option = neighoursPossibleOptions[i];

                    //I.E don't remove a state if it's a valid neighbour for the current tile, do remove it if it isn't.
                    if (curCellPossibles.Contains(option))
                        continue;

                    surroundingCell.RemoveOption(option);
                    addToList = true;
                }

                //if the neighbour tile has had its possibilities affected, add it to the list of tiles to iterate through
                //after updating its own valid neighbours list
                if (!addToList) 
                    continue;

                var typeList = new List<ETileType>();

                foreach (var tileObj in AllTileObjects)
                {
                    foreach (var acceptableNeighbor in tileObj.TileSO.Tile.AcceptableNeighbours)
                    {
                        if (surroundingCell.TileDetails.AvailableTypes.Contains(acceptableNeighbor))
                            typeList.Add(tileObj.TileSO.Tile.TileType);
                    }
                }

                surroundingCell.UpdateNeighbours(typeList);
                _tilesToIterateOn.Add(surroundingCell.Coords);
            }
        }
    }
    
    public List<TileController> GetNeighbours(Coordinates centerTileCoords, int distance = 1,
        bool checkDistance = false)
    {
        var neighbours = new List<TileController>();

        for (var i = -1 * distance; i < 2 * distance; i++)
        {
            for (var j = -1 * distance; j < 2 * distance; j++)
            {
                if (i == 0 && j == 0)
                    continue;

                var centerTile = GetHexAt(centerTileCoords.X, centerTileCoords.Y);

                var coord = new Coordinates
                {
                    X = centerTileCoords.X + i,
                    Y = centerTileCoords.Y + j
                };

                var newHex = GetHexAt(coord.X, coord.Y);

                if (newHex == null || newHex == centerTile || neighbours.Contains(newHex)) 
                    continue;

                //TODO: Fix the distance check to respect wrapping
                //this needs to change so that we account for tile wrapping 
                if (checkDistance)
                {
                    var dist = Vector3.Distance(GetHexAt(centerTileCoords.X, centerTileCoords.Y).transform.position,
                        newHex.transform.position);

                    var max = (newHex.Hex.HexWidth() * distance) + 0.1f;

                    if (dist >= max)
                    {
                        continue;
                    }
                }

                neighbours.Add(newHex);
            }
        }

        return neighbours;
    }

    public TileController GetHexAt(int x, int y)
    {
        if (TileDictionary == null)
        {
            Debug.LogError("Hexes array not yet instantiated.");
            return null;
        }

        x = (int)Mathf.Repeat(x, GridWidth);

        if (y < 0 || y >= GridHeight)
            return null;

        var coord = new Coordinates(x, y);
        if (TileDictionary.ContainsKey(coord))
        {
            return TileDictionary[coord];
        }

        Debug.LogError("GetHexAt: " + x + "," + y);
        return null;
    }

    public Coordinates GetWrappedCoordinates(Coordinates targetCoords)
    {
        var newX = (int)Mathf.Repeat(targetCoords.X, GridWidth);
        var newY = (int)Mathf.Repeat(targetCoords.Y, GridHeight);

        return new Coordinates(newX, newY);
    }
}
