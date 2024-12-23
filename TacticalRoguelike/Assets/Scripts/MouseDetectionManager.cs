using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseDetectionManager : MonoBehaviour
{

    // IF YOU ARE CHANNELING SKILL, CHARACTER THAT IS TARGETED AND CLICKED SHOULDN'T BE SELECTED
    public bool isSkillSelected;

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            if(GameObject.FindWithTag("GameManager").GetComponent<TurnManager>().isDuringChannel ||
            GameObject.FindWithTag("GameManager").GetComponent<TurnManager>().Turn == -1) return;
            RaycastHit2D[] hit = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

            for(int i = 0; i < hit.Length; i++)
            {
                if(hit[i].collider != null)
                {
                    if(hit[i].collider.tag == "Ally")
                    {
                        hit[i].collider.gameObject.GetComponent<AllyIsSelected>().isClicked = true;
                        CheckWhichAllyIsSelected();
                    }
                }
            }
        }
    }

    public GameObject[] Allies;
    void CheckWhichAllyIsSelected(){
        Allies = GameObject.FindGameObjectsWithTag("Ally");
        
        for(int i = 0; i < Allies.Length; i++){
            if(Allies[i].GetComponent<AllyIsSelected>().isClicked == true){
            Allies[i].GetComponent<AllyIsSelected>().isSelected = true;
            Allies[i].GetComponent<AllyIsSelected>().isClicked = false;
            }

            else
            Allies[i].GetComponent<AllyIsSelected>().isSelected = false;
        }
    }
}
