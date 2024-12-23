using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstacleTree : MonoBehaviour
{

    public Animator anim;


    void Awake(){
        anim = GetComponent<Animator>();
    }


    void Start()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);

        anim.Play(state.fullPathHash , 0 , Random.Range(0f , 1f));
    }


    void Update()
    {
        
    }
}
