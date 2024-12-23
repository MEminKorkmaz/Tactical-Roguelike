using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArcherProjectile : MonoBehaviour
{
    public GameObject ArrowFxPrefab;
    
    public Rigidbody2D rb;

    public float Speed;
    public float RotateSpeed;

    public int Damage;

    public bool isCritic;
    public int critChance;
    public int critMultiplier;

    public int Check;


    void Awake(){
        // Damage = GameObject.FindWithTag("Archer").GetComponent<AllyStats>().Damage;
        // critMultiplier = GameObject.FindWithTag("Archer").GetComponent<AllyStats>().CritMultiplier;
        // critChance = GameObject.FindWithTag("Archer").GetComponent<AllyStats>().CritChance;

        // int minDamage = Damage - ((Damage * 10) / 100);
        // int maxDamage = Damage + ((Damage * 10) / 100);

        // Damage = Random.Range(minDamage , maxDamage + 1);

        // int rnd = Random.Range(0 , 100);
        // if(rnd < critChance){
        //     isCritic = true;
        // }
        // else{
        //     isCritic = false;
        // }
    }
    void Start()
    {
        int minDamage = Damage - ((Damage * 10) / 100);
        int maxDamage = Damage + ((Damage * 10) / 100);

        Damage = Random.Range(minDamage , maxDamage + 1);

        int rnd = Random.Range(0 , 100);
        if(rnd < critChance){
            isCritic = true;
        }
        else{
            isCritic = false;
        }
    }


    void FixedUpdate()
    {
        Move();
    }

    public Vector3 EnemyPos;
    void Move(){
        // rb.velocity = transform.up * Speed * Time.fixedDeltaTime;

        Vector2 dir = (Vector2)EnemyPos - rb.position;
        dir.Normalize();
        float rotateAmount = Vector3.Cross(dir , transform.up).z;
        rb.angularVelocity = -rotateAmount * RotateSpeed;
        rb.velocity = transform.up * Speed * Time.fixedDeltaTime;
    }

    public GameObject textPrefab;
    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "Enemy"){
            // GameObject.FindWithTag("GameManager").GetComponent<TurnManager>().isDuringTurn = false;
            if(!col.gameObject.GetComponent<EnemyTargeted>().isEnemyTargeted) return;

            GameObject.FindWithTag("GameManager").GetComponent<TurnManager>().isDuringTurn = false;

            int missChance = col.gameObject.GetComponent<EnemyStats>().Evasion;
            // Debug.Log(Damage + "MAIN");

            if(isCritic)
            Damage = Damage + ((Damage * critMultiplier) / 100);

            int def = col.gameObject.GetComponent<EnemyStats>().Defence;
            Damage = Damage - ((Damage * def) / 100);

            // Debug.Log(Damage + "AFTER DEF");

            int rnd = Random.Range(0 , 100);
            if(rnd < missChance){
                Damage = 0;
                Debug.Log("Miss");
            }
            // Debug.Log(Damage);

            if(Damage == 0){
                GameObject TempText = Instantiate(textPrefab , col.transform.position , Quaternion.identity);
                TempText.transform.gameObject.GetComponent<RectTransform>().localScale = new Vector2(1f , 1f);
                TempText.transform.GetChild(0).gameObject.GetComponent<Text>().text = "MISS!!";
                Destroy(TempText , 3f);
            }

            else
            {
                GameObject TempText = Instantiate(textPrefab , col.transform.position , Quaternion.identity);
                TempText.transform.gameObject.GetComponent<RectTransform>().localScale = new Vector2(1f , 1f);
                if(isCritic)
                    TempText.transform.GetChild(0).gameObject.GetComponent<Text>().color = Color.red;
                TempText.transform.GetChild(0).gameObject.GetComponent<Text>().text = Damage.ToString() + "!";
                Destroy(TempText , 3f);
            }

            col.gameObject.GetComponent<EnemyTargeted>().isEnemyTargeted = false;
            // int def = col.gameObject.GetComponent<EnemyStats>().Defence;
            // def = def / 5;
            // TempDamage = Damage - ((Damage * def) / 100);
            
            // TEMPORARY
            GameObject TempGo = transform.GetChild(1).gameObject;
            TempGo.GetComponent<ParticleSystem>().Stop(true);
            transform.GetChild(1).parent = null;
            Destroy(TempGo , 2f);
            // TEMPORARY

            // Debug.Log(col.gameObject.name);
            col.gameObject.GetComponent<EnemyTakeDamage>().GetDamage(Damage);
            GameObject go = Instantiate(ArrowFxPrefab , transform.position , transform.rotation);
            Destroy(go , 5f);
            Destroy(gameObject);
        }
    }
}
