using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class TileManager : MonoBehaviour {

    public Tilemap grass;
    public Tilemap water;
    public Tilemap mountain;
    public Tilemap selectionAreaBlue;
    public Tilemap selectionAreaRed;
    public Tile blueTile;
    public Tile redTile;
    

    Dictionary<Vector3, Character> map = new Dictionary<Vector3, Character>();


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void updateMap(Vector3 pos, Character c)
    {
        if (c == null) map.Remove(pos);
        map[pos] = c;

    }
    public Character getCharacter(Vector3 pos)
    {

        if (map.ContainsKey(pos)) return map[pos];
        return null;

    }

    public bool available(Character c, Vector3 position)
    {
        
        string tileType = this.tileType(position);
        if (c.transform.position == position) return true;
        if (c.canAcross(tileType) && this.getCharacter(position) == null) return true;
        return false;

    }


    public bool hasTile(Vector3 position)
    {

        Vector3Int cellPosition = grass.WorldToCell(position);
        return this.hasTile(cellPosition);

    }

    public bool hasTile(Vector3Int cellPosition)
    {

        return this.tileType(cellPosition) != null;

    }

    public string tileType(Vector3 position)
    {

        Vector3Int cellPosition = grass.WorldToCell(position);
        return tileType(cellPosition);

    }

    public string tileType(Vector3Int cellPosition)
    {

        string type = null;
        if (grass.HasTile(cellPosition))
        {
            type = "grass";
        }
        if (water.HasTile(cellPosition))
        {
            type = "water";
        }
        if (mountain.HasTile(cellPosition))
        {
            type = "mountain";
        }
        return type;

    }

    public bool hasBlueTile(Vector3 position)
    {

        Vector3Int cellPosition = selectionAreaBlue.WorldToCell(position);
        return selectionAreaBlue.HasTile(cellPosition);

    }

    public bool hasRedTile(Vector3 position)
    {

        Vector3Int cellPosition = selectionAreaBlue.WorldToCell(position);
        return selectionAreaRed.HasTile(cellPosition);

    }
    
    public List<Vector3> moveZone(Character character)
    {
        /*
        //find the move zone using DFS
        List<Vector3> tiles = iMoveZone(character, character.transform.position, character.moveStat,new List<Vector3>());
        */

        // finds move zone using BFS
        List<Vector3> tiles2 = this.iMoveZone2(character);

        //return tiles;
        return tiles2;

    }


    private List<Vector3> iMoveZone(Character character,Vector3 pos,int movStat,List<Vector3> visited)
    {
        //Finds the move zone of character using, Depth-first search (DFS) 

        List<Vector3> tiles = new List<Vector3>();

        // generate childrens
        Vector3 topPosition = new Vector3(pos.x, pos.y + 1, pos.z);
        Vector3 botPosition = new Vector3(pos.x, pos.y - 1, pos.z);
        Vector3 rightPosition = new Vector3(pos.x + 1, pos.y, pos.z);
        Vector3 leftPosition = new Vector3(pos.x - 1, pos.y, pos.z);

        if (this.hasTile(pos))
        {
            if (movStat >= 0 && this.available(character,pos) )
            {

                tiles.Add(pos);
                visited.Add(pos);
                
                if (!visited.Contains(topPosition)) tiles.AddRange(iMoveZone(character, topPosition, movStat - 1, visited));
                if (!visited.Contains(botPosition)) tiles.AddRange(iMoveZone(character, botPosition, movStat - 1, visited));
                if (!visited.Contains(rightPosition)) tiles.AddRange(iMoveZone(character, rightPosition, movStat - 1, visited));
                if (!visited.Contains(leftPosition)) tiles.AddRange(iMoveZone(character, leftPosition, movStat - 1, visited));

            }
        }

        return tiles;

    }

    private struct Node {
        public Vector3 position;
        public int moveStat;
        public Node(Vector3 pos, int moveStat) {
            this.position = pos;
            this.moveStat = moveStat;
        }
        public bool Equals(Node n)
        {
            return (this.position == n.position && this.moveStat==n.moveStat);
        }
    }
    
    private List<Vector3> iMoveZone2(Character c) {
        // Finds the move zone of character using Breadth-first search BFS
        List<Vector3> moveZone = new List<Vector3>();
        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();
        //add start point to openList
        openList.Add(new Node(c.transform.position,c.moveStat));

        while (openList.Count > 0) {
            Node currentNode = openList[0];
            openList.Remove(currentNode);
            closedList.Add(currentNode);
            
            if (!moveZone.Contains(currentNode.position) && this.available(c,currentNode.position))
                moveZone.Add(currentNode.position);

            Vector3[] adjacent = { new Vector3(-1f, 0f, 0f), new Vector3(1f, 0f, 0f), new Vector3(0f, -1f, 0f), new Vector3(0f, 1f, 0f) };
            foreach (Vector3 v in adjacent)
            {
                Vector3 newPos = currentNode.position + v;

                if (this.available(c, newPos) && currentNode.moveStat > 0)
                {
                    Node newNode = new Node(newPos, currentNode.moveStat - 1);

                    if ( closedList.FindAll(x => x.position == newNode.position && x.moveStat >= newNode.moveStat).Count == 0 )
                    {
                        openList.Add(newNode);
                    }
                    /*
                    if (newNode.moveStat > openList.Find(x => x.position == newNode.position).moveStat) {
                        openList.Remove(openList.Find(x => x.position == newNode.position));
                    }
                    */
                   // openList.Add(newNode);

                }
            }


        }

        return moveZone;
    }

    void paintBlueTiles(List<Vector3> tiles)
    {

        foreach (Vector3 v in tiles)
        {
            Vector3Int cellPosition = selectionAreaBlue.WorldToCell(v);
            selectionAreaBlue.SetTile(cellPosition, blueTile);
        }

    }
    


    public void paintMoveZone(Character character)
    {

        List<Vector3> tiles = moveZone(character);
        paintBlueTiles(tiles);

    }


    public void paintAttackArea(HashSet<Vector3> attackArea)
    {
        
        foreach (Vector3 v in attackArea)
        {
            Vector3Int newVector = selectionAreaRed.WorldToCell(v);
            if (this.hasTile(newVector)) selectionAreaRed.SetTile(newVector,redTile);

        }

    }
    
    
    

    public void clearSpecialTiles()
    {

        selectionAreaBlue.ClearAllTiles();
        selectionAreaRed.ClearAllTiles();

    }

    public int getBonusAtk(Vector3 pos)
    {

        int bonusATK = 0;
        string tileType = this.tileType(pos);

        switch (tileType)
        {
            case "grass":
                bonusATK = 0;
                break;
            case "water":
                bonusATK = 0;
                break;
            case "mountain":
                bonusATK = 2;
                break;
            default:
                break;
        }

        return bonusATK;
    }

    public int getBonusDef(Vector3 pos)
    {

        int bonusDEF = 0;
        string tileType = this.tileType(pos);

        switch (tileType)
        {
            case "grass":
                bonusDEF = 0;
                break;
            case "water":
                bonusDEF = 2;
                break;
            case "mountain":
                bonusDEF = 0;
                break;
            default:
                break;
        }

        return bonusDEF;
    }
    



}
