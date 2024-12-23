using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyActionableCheck : MonoBehaviour
{
    public GameObject CurrentTile;


    void OnTriggerStay2D(Collider2D col){
        if(col.gameObject.tag == "Tile"){
            CurrentTile = col.gameObject;
            // CurrentTile.gameObject.SetActive(false);
        }
    }
}
