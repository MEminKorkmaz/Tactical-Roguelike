using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapObstacleWater : MonoBehaviour
{
    public GameObject[] Rocks;

    void Start(){
        int x = Random.Range(0 , 3);

        if(x == 0){
            int y = Random.Range(0 , 3);
            Instantiate(Rocks[y] , transform.position , transform.rotation);
        }
    }
}
