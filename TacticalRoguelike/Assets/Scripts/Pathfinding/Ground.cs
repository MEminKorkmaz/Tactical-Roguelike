using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour
{
    public Pathfinding pf;

    public Transform[] children = new Transform[98];

    public Tiles[] tiles = new Tiles[98];

    private bool SpriteDecision;


    void Awake(){
        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();
    }

    void Start(){
        GetChildrenObjects();
    }

    void GetChildrenObjects(){
        // for (int i = 0; i < transform.childCount; i++) {
        // children[i] = transform.GetChild(i);
        // }
        bool childrenEvenOrOdd = false;

        for(int i = 0; i < 98; i++)
        {
            if(!childrenEvenOrOdd)
            {
                children[i] = transform.GetChild(0).transform.GetChild(i / 2);
                // children[i] = transform.GetChild(1).transform.GetChild(i);
                childrenEvenOrOdd = !childrenEvenOrOdd;
            }
            else
            {
                children[i] = transform.GetChild(1).transform.GetChild(i / 2);
                // children[i] = transform.GetChild(0).transform.GetChild(i);
                childrenEvenOrOdd = !childrenEvenOrOdd;
            }
        }

        for(int i = 0; i < 98; i++){
            tiles[i] = children[i].GetComponent<Tiles>();
        }
    }

    public void HighlightTheMovableGrids(GameObject go , int MaxMove , string color , bool isMovement){
        // for(int i = 0; i < children.Length; i++){
            // MAKE IT FASTER maybe in findpaths
            // Vector3 pos = new Vector3(go.transform.position.x , go.transform.position.y + -0.25f , 0);
            // Vector3 pos = new Vector3(go.transform.position.x , go.transform.position.y , 0);
            // if(pos == children[i].position){
                pf.FindPaths(go , MaxMove , color , isMovement);
            // }
        // }
    }

    public void UnHighlighTheMovableGrids(){
        for(int i = 0; i < children.Length; i++){
            // children[i].GetComponent<Tiles>().CheckForMovability(false , null);
            // children[i].GetComponent<Tiles>().cost = 0;

            tiles[i].CheckForMovability(false , null);
            tiles[i].GetComponent<Tiles>().cost = 0;

            // children[i].GetComponent<Tiles>().isOccupied = false;
            //children[i].GetComponent<Tiles>().parent = null;
        }
    }

    public int GetWhichTile(GameObject go){
        int Temp = 0;
        for(int i = 0; i < children.Length; i++){
            // Vector3 pos = new Vector3(go.transform.position.x , go.transform.position.y + -0.25f , 0);
            Vector3 pos = new Vector3(go.transform.position.x , go.transform.position.y , 0);
            if(pos == children[i].position){
                Temp = i;
            }
        }
        return Temp;
    }

    public void ParentNull(){
        for(int i = 0; i < children.Length; i++){
            children[i].GetComponent<Tiles>().parent = null;
        }
    }

    // public List<GameObject> EnemyMovementTiles = new List<GameObject>();
    // public void EnemyMovementTileCollection(){
    //     for(int i = 0; i < children.Length; i++){
    //         if(children[i].GetComponent<Tiles>().isEnemyMovable){
    //             EnemyMovementTiles[i] = children[i].gameObject;
    //         }
    //     }
    // }

    // public void OccupyDetection(int k){
    //     for (int i = 0; i < children.Length; i++)
    //     {
    //         if(i == k)
    //         {
    //             children[i].gameObject.GetComponent<Tiles>().isOccupied = true;
    //         }
    //         else
    //         children[i].gameObject.GetComponent<Tiles>().isOccupied = false;
    //     }
    // }
}
