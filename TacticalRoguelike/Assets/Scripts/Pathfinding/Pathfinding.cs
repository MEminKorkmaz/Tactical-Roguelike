using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public int LayerGround;
    private int cost;

    public List<GameObject> Frontier = new List<GameObject>();


    // ENEMY SIDE START
    // [HideInInspector]
    public List<GameObject> EnemyActionTile = new List<GameObject>();
    public List<GameObject> EnemyFrontier = new List<GameObject>();
    public void EnemyFindPaths(GameObject character , int MaxMove , bool isMovement)
    {
        Frontier = new List<GameObject>();
        Physics2D.queriesStartInColliders = !Physics2D.queriesStartInColliders;
        Queue<GameObject> openSet = new Queue<GameObject>();
        openSet.Enqueue(character);
    
        while (openSet.Count > 0)
        {
            GameObject currentTile = openSet.Dequeue();
            // EnemyFrontier.Add(currentTile);

            foreach (GameObject adjacentTile in GetAdjacentTiles(currentTile , isMovement))
            {
                if (openSet.Contains(adjacentTile))
                continue;
                
                int adjacentTileCost = adjacentTile.GetComponent<Tiles>().cost;
                int currentTileCost = currentTile.GetComponent<Tiles>().cost;
                adjacentTile.GetComponent<Tiles>().cost = currentTile.GetComponent<Tiles>().cost + 1;

                if(!isValidTile(adjacentTile , adjacentTile.GetComponent<Tiles>().cost , MaxMove))
                continue;

                adjacentTile.GetComponent<Tiles>().parent = currentTile.gameObject;

                // adjacentTile.GetComponent<Tiles>().CheckForMovability(true , "MovementColor");

                // adjacentTile.GetComponent<Tiles>().isEnemyMovable = true;
                EnemyActionTile.Add(adjacentTile);
                // adjacentTile.GetComponent<Tiles>().CheckForMovability(true , color);

                openSet.Enqueue(adjacentTile);
                EnemyFrontier.Add(adjacentTile);
            }
        }
        Physics2D.queriesStartInColliders = !Physics2D.queriesStartInColliders;
    }

    private int y;
    public List<GameObject> EnemyPathTiles = new List<GameObject>();
    public void EnemyMakePath(GameObject destination, GameObject origin)
    {
        // GameObject.FindWithTag("Map").GetComponent<Ground>().ParentNull();
        // PathTiles = new List<GameObject>();
        GameObject current = destination;

        while (current != origin && x < 575)
        {
            x++;
            EnemyPathTiles.Add(current);
            if (current.GetComponent<Tiles>().parent != null)
                current = current.GetComponent<Tiles>().parent;
            else
            {
                break;
            }
        }

        // PathTiles.Add(origin);
        EnemyPathTiles.Reverse();

        return;
    }

    bool isValidTile(GameObject tile , int currentCost , int MaxMove)
    {
        bool valid = false;

        if (!EnemyFrontier.Contains(tile) && currentCost <= MaxMove)
        {
            valid = true;
        }

        return valid;
    }

    // ENEMY SIDE END


    public void FindPaths(GameObject character , int MaxMove , string color , bool isMovement)
    {
        Frontier = new List<GameObject>();
        Physics2D.queriesStartInColliders = !Physics2D.queriesStartInColliders;
        Queue<GameObject> openSet = new Queue<GameObject>();
        openSet.Enqueue(character);
    
        while (openSet.Count > 0)
        {
            GameObject currentTile = openSet.Dequeue();

            foreach (GameObject adjacentTile in GetAdjacentTiles(currentTile , isMovement))
            {
                if(Frontier.Contains(adjacentTile))
                continue;

                int adjacentTileCost = adjacentTile.GetComponent<Tiles>().cost;
                int currentTileCost = currentTile.GetComponent<Tiles>().cost;
                adjacentTile.GetComponent<Tiles>().cost = currentTile.GetComponent<Tiles>().cost + 1;

                if(!isValidTileEnemy(adjacentTile , adjacentTile.GetComponent<Tiles>().cost , MaxMove))
                continue;

                adjacentTile.GetComponent<Tiles>().parent = currentTile.gameObject;

                adjacentTile.GetComponent<Tiles>().CheckForMovability(true , color);

                openSet.Enqueue(adjacentTile);
                Frontier.Add(adjacentTile);
            }
        }
        Physics2D.queriesStartInColliders = !Physics2D.queriesStartInColliders;
    }

    bool isValidTileEnemy(GameObject tile , int currentCost , int MaxMove){
        bool valid = false;

        if (!Frontier.Contains(tile) && currentCost <= MaxMove){
            valid = true;
            }

        return valid;
    }

    private float RayValue = 1f; // IF RAYVALUE CHANGABLE(SOMEHOW) MAKE SURE ONCE RAYCAST HIT FIRST TILE LAYER MAKE IT STOP(THE ARRAY)
    private List<GameObject> GetAdjacentTiles(GameObject tile , bool isMovement){
                
        List<GameObject> tiles = new List<GameObject>();

        LayerGround = LayerMask.NameToLayer("Tile");

        for(int i = 0; i < 4; i++){
        if(i == 0)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(tile.transform.position , Vector2.up , RayValue);

            if(hit.Length > 0)
            {
                for(int j = 0; j < hit.Length; j++)
                {
                    if(hit[j].transform.gameObject.layer == LayerGround)
                    {
                        GameObject HitTile = hit[j].transform.gameObject;

                        if(isMovement)
                        {
                            if(HitTile.gameObject.GetComponent<Tiles>().isOccupiedByAlly
                            || HitTile.gameObject.GetComponent<Tiles>().isOccupiedByEnemy
                            || HitTile.gameObject.GetComponent<Tiles>().isObstacle
                            || HitTile.gameObject.GetComponent<Tiles>().isLowObstacle)
                            continue;
                        }

                        else
                        {
                            if(HitTile.gameObject.GetComponent<Tiles>().isObstacle)
                            continue;
                        }
                    tiles.Add(HitTile);
                    }
                }
            }
        }

        else if(i == 1)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(tile.transform.position , -Vector2.up , RayValue);

            if(hit.Length > 0)
            {
                for(int j = 0; j < hit.Length; j++)
                {
                    if(hit[j].transform.gameObject.layer == LayerGround)
                    {
                        GameObject HitTile = hit[j].transform.gameObject;

                        if(isMovement)
                        {
                            if(HitTile.gameObject.GetComponent<Tiles>().isOccupiedByAlly
                            || HitTile.gameObject.GetComponent<Tiles>().isOccupiedByEnemy
                            || HitTile.gameObject.GetComponent<Tiles>().isObstacle
                            || HitTile.gameObject.GetComponent<Tiles>().isLowObstacle)
                            continue;
                        }

                        else
                        {
                            if(HitTile.gameObject.GetComponent<Tiles>().isObstacle)
                            continue;
                        }
                    tiles.Add(HitTile);
                    }
                }
            }
        }

        else if(i == 2)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(tile.transform.position , Vector2.left , RayValue);

            if(hit.Length > 0)
            {
                for(int j = 0; j < hit.Length; j++)
                {
                    if(hit[j].transform.gameObject.layer == LayerGround)
                    {
                        GameObject HitTile = hit[j].transform.gameObject;

                        if(isMovement)
                        {
                            if(HitTile.gameObject.GetComponent<Tiles>().isOccupiedByAlly
                            || HitTile.gameObject.GetComponent<Tiles>().isOccupiedByEnemy
                            || HitTile.gameObject.GetComponent<Tiles>().isObstacle
                            || HitTile.gameObject.GetComponent<Tiles>().isLowObstacle)
                            continue;
                        }

                        else
                        {
                            if(HitTile.gameObject.GetComponent<Tiles>().isObstacle)
                            continue;
                        }
                    tiles.Add(HitTile);
                    }
                }
            }
        }

        else if(i == 3)
        {
            RaycastHit2D[] hit = Physics2D.RaycastAll(tile.transform.position , -Vector2.left , RayValue);

            if(hit.Length > 0)
            {
                for(int j = 0; j < hit.Length; j++)
                {
                    if(hit[j].transform.gameObject.layer == LayerGround)
                    {
                        GameObject HitTile = hit[j].transform.gameObject;

                        if(isMovement)
                        {
                            if(HitTile.gameObject.GetComponent<Tiles>().isOccupiedByAlly
                            || HitTile.gameObject.GetComponent<Tiles>().isOccupiedByEnemy
                            || HitTile.gameObject.GetComponent<Tiles>().isObstacle
                            || HitTile.gameObject.GetComponent<Tiles>().isLowObstacle)
                            continue;
                        }

                        else
                        {
                            if(HitTile.gameObject.GetComponent<Tiles>().isObstacle)
                            continue;
                        }
                    tiles.Add(HitTile);
                    }
                }
            }
        }
        }
        return tiles;
    }


    private int x;
    public List<GameObject> PathTiles = new List<GameObject>();
    public void MakePath(GameObject destination, GameObject origin)
    {
        // GameObject.FindWithTag("Map").GetComponent<Ground>().ParentNull();
        // PathTiles = new List<GameObject>();
        GameObject current = destination;

        while (current != origin && x < 575)
        {
            x++;
            PathTiles.Add(current);
            if (current.GetComponent<Tiles>().parent != null)
                current = current.GetComponent<Tiles>().parent;
            else
            {
                break;
            }
        }

        // PathTiles.Add(origin);
        PathTiles.Reverse();

        return;
    }
}
