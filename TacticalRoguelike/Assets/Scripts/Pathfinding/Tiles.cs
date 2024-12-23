using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tiles : MonoBehaviour
{
    public bool isEnemyMovable;
    // [HideInInspector]
    // public bool isMovable;
    // public bool isAttackable;

    public bool isActionable;
    //public int value;
    public int cost;
    public bool isOccupiedByAlly;
    public bool isOccupiedByEnemy;
    public bool isObstacle;
    public bool isLowObstacle;
    public GameObject parent;

    public Color DefaultColor;
    public Color MovementColor;
    public Color NormalAttackColor;
    public Color HealingColor;

    // TEMPORARY

    public Color TempDefaultColor;
    public Color TempMovementColor;
    public Color TempNormalAttackColor;
    public Color TempHealingColor;

    // TEMPORARY

    private SpriteRenderer sr;
    
    public int LayerGround;

    public GameObject TileHightlightPrefab;
    public GameObject TempTileHighlight;



    void Awake(){
        sr = GetComponent<SpriteRenderer>();
    }
    void Start(){
        //isMovable = false;
        TempTileHighlight = Instantiate(TileHightlightPrefab , this.gameObject.transform);
        TempTileHighlight.transform.localPosition = new Vector3(0f , 0f , 0f);

        // TEMPORARY FOR TRYING SPRITES
        TempTileHighlight.transform.localScale = new Vector3(6f , 6f , 0f);
        // TEMPORARY FOR TRYING SPRITES

        TempTileHighlight.SetActive(false);
        // go.transform = this.Child(0);
    }

    void Update(){
        //CheckForMovability;
    }

    public void CheckForMovability(bool x , string color){
        if(x){
            if(color == "MovementColor"){
            isActionable = true;
            // isMovable = true;

            // sr.color = MovementColor;
            // TEMPORARY
            TempTileHighlight.SetActive(true);
            TempTileHighlight.GetComponent<SpriteRenderer>().color = TempMovementColor;
            // TEMPORARY
            return;
            }
            else if(color == "NormalAttackColor"){
                isActionable = true;
                // isAttackable = true;

                // sr.color = NormalAttackColor;
                // TEMPORARY
                TempTileHighlight.SetActive(true);
                TempTileHighlight.GetComponent<SpriteRenderer>().color = TempNormalAttackColor;
                // TEMPORARY
                return;
            }

            else if(color == "HealingColor"){
                isActionable = true;
                // sr.color = NormalAttackColor;
                // TEMPORARY
                TempTileHighlight.SetActive(true);
                TempTileHighlight.GetComponent<SpriteRenderer>().color = TempHealingColor;
                // TEMPORARY
                return;
            }
        }

        else{
            // TEMPORARY
            TempTileHighlight.SetActive(false);
            TempTileHighlight.GetComponent<SpriteRenderer>().color = TempDefaultColor;
            // TEMPORARY
            // isMovable = false;
            // isAttackable = false;
            isActionable = false;
            sr.color = DefaultColor;
            return;
        }
    }

    public GameObject OccupiedAlly;
    void OnTriggerStay2D(Collider2D col){
        if(col.gameObject.tag == "Ally"){
            OccupiedAlly = col.gameObject;
            isOccupiedByAlly = true;
        }
        if(col.gameObject.tag == "Enemy"){
            isOccupiedByEnemy = true;
        }
    }

    void OnTriggerExit2D(Collider2D col){
        if(col.gameObject.tag == "Ally"){
            OccupiedAlly = null;
            isOccupiedByAlly = false;
        }
        if(col.gameObject.tag == "Enemy"){
            isOccupiedByEnemy = false;
        }
    }

    // void OnTriggerStay2D(Collider2D col){
    //     if(col.gameObject.CompareTag("Archer")){
    //         Debug.Log("Hello");
    //         isOccupied = true;
    //     }
    // }

    // void OnTriggerExit2D(Collider2D col){
    //     if(col.gameObject.CompareTag("Archer")){
    //         isOccupied = false;
    //     }
    // }

    // private List<GameObject> GetAdjacentTiles(GameObject tile){
    //     // if(value != 1) return;

    //     List<GameObject> tiles = new List<GameObject>();

    //     LayerGround = LayerMask.NameToLayer("Tile");

    //     for(int i = 0; i < 4; i++){

    //     if(i == 0){
    //     RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.up , RayValue);

    //     if(hit && hit.transform.gameObject.layer == LayerGround){

    //         GameObject HitTile = hit.transform.gameObject;

    //         float distance = Mathf.Abs(hit.point.y - transform.position.y);

    //         int value = hit.transform.gameObject.GetComponent<PTTiles>().value;

    //         hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

    //         Debug.Log(distance + " " + value);
    //             }
    //         }

    //     else if(i == 1){
    //         RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up , RayValue);

    //         if(hit && hit.transform.gameObject.layer == LayerGround){

    //         GameObject HitTile = hit.transform.gameObject;

    //         float distance = Mathf.Abs(hit.point.y - transform.position.y);

    //         int value = hit.transform.gameObject.GetComponent<PTTiles>().value;

    //         hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

    //         Debug.Log(distance + " " + value);
    //             }
    //         }

    //     else if(i == 2){
    //         RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left , RayValue);

    //         if(hit && hit.transform.gameObject.layer == LayerGround){

    //         GameObject HitTile = hit.transform.gameObject;

    //         float distance = Mathf.Abs(hit.point.x - transform.position.x);

    //         int value = hit.transform.gameObject.GetComponent<PTTiles>().value;

    //         hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

    //         Debug.Log(distance + " " + value);
    //             }
    //         }

    //     else if(i == 3){
    //         RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.left , RayValue);

    //         if(hit && hit.transform.gameObject.layer == LayerGround){

    //         GameObject HitTile = hit.transform.gameObject;

    //         float distance = Mathf.Abs(hit.point.x - transform.position.x);

    //         int value = hit.transform.gameObject.GetComponent<PTTiles>().value;

    //         hit.transform.gameObject.GetComponent<SpriteRenderer>().color = Color.green;

    //         Debug.Log(distance + " " + value);
    //             }
    //         }
    //     }
    //     return tiles;
    // }
}
