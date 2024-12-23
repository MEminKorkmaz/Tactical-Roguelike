using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForrestBatLittle : MonoBehaviour
{

    public TurnManager turnManager;
    public Animator anim;

    public GameObject HitFx;

    public GameObject ForrestBatObject;
    public ForrestBat forrestBat;

    public Rigidbody2D rb;

    public GameObject Trail;

    [Header("WHILE IDLE")]
    public float MinSpeed;
    public float MaxSpeed;
    public float Speed;

    public Vector3 TargetPositionToMoveWhileIdle;

    public float MinTimeForPos;
    public float MaxTimeForPos;

    [Header("WHILE IN ATTACK MODE")]
    public int Damage;

    private bool isCritic;
    public int critChance;
    public int critMultiplier;

    public float SpeedInAttackMode;
    public float RotatingSpeedInAttackMode;

    public GameObject TemporaryEnemy;

    [Header("MISSCELLANEOUS")]
    public bool isInAttackMode;

    private float TimerForNewPositionWhileIdle = 0f;



    void Awake(){
        ForrestBatObject = transform.parent.gameObject;

        forrestBat = ForrestBatObject.GetComponent<ForrestBat>();

        turnManager = GameObject.FindWithTag("GameManager").GetComponent<TurnManager>();

        rb = GetComponent<Rigidbody2D>();

        Trail = transform.GetChild(0).gameObject;
    }

    void Start()
    {
        anim = GetComponent<Animator>();

        var state = anim.GetCurrentAnimatorStateInfo(0);

        anim.Play(state.fullPathHash , 0 , Random.Range(0f , 1f));

        // InvokeRepeating("GetPositionAndSpeed" , 0f , Random.Range(MinTimeForPos , MaxTimeForPos));

        Trail.SetActive(false);
    }

    void DamageAndCritDetermination(){
        int minDamage = Damage - ((Damage * 50) / 100);
        int maxDamage = Damage + ((Damage * 50) / 100);

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
        if(isInAttackMode){
            // Vector2 dir = (Vector2)TemporaryEnemy.transform.position - rb.position;
            // dir.Normalize();
            // float rotateAmount = Vector3.Cross(dir , transform.up).z;
            // rb.angularVelocity = -rotateAmount * RotatingSpeedInAttackMode;
            // rb.velocity = transform.up * SpeedInAttackMode * Time.fixedDeltaTime;

            if(TemporaryEnemy == null){
                SetSelfInIdleMode();
                // isTheLastOneToAttack = false;
                // if(transform.parent.gameObject.GetComponent<EnemyAction>().isEnemyDone == false){
                if(!isTheLastOneToAttack) return;
                transform.parent.gameObject.GetComponent<EnemyAction>().isEnemyDone = true;
                turnManager.isDuringTurn = false;
                // }
                // return;
            }

            else{
                transform.position = Vector2.MoveTowards(transform.position , TemporaryEnemy.transform.position ,
                SpeedInAttackMode * Time.fixedDeltaTime);
            }
        }

        else{
            transform.position = Vector2.MoveTowards(transform.position , TargetPositionToMoveWhileIdle , Speed * Time.fixedDeltaTime);
        }
    }


    void Update(){
        Flip();

        if(TimerForNewPositionWhileIdle > 0)
        TimerForNewPositionWhileIdle -= Time.deltaTime;
        
        if(TimerForNewPositionWhileIdle <= 0 && !isInAttackMode){
            GetPositionAndSpeed();

            TimerForNewPositionWhileIdle = Random.Range(MinTimeForPos , MaxTimeForPos);
        }

        // ROTATING WHILE CHARGING TO ENEMY
        if(isInAttackMode){
            transform.Rotate(0f , 100f * Time.deltaTime , 0f);
        }
    }

    void GetPositionAndSpeed(){
        TargetPositionToMoveWhileIdle = forrestBat.SetPositionForLittleBat();

        Speed = Random.Range(MinSpeed , MaxSpeed);

        float distance = Vector2.Distance(transform.position , transform.parent.position);

        // AFTER THE ATTACK MODE WHILE COMING BACK TO IT'S PARENT, LITTLE BATS MUST GO A LITTLE FASTER
        if(distance >= 1.5f){
        Speed *= distance * 2f;
        }
    }


    // ATTACK MODE
    public void SetSelfInAttackMode(float tempTimer){
        
        Invoke(nameof(SetSelfInAttackModeTrigger) , tempTimer);
    }

    void SetSelfInAttackModeTrigger(){
        isInAttackMode = true;

        DamageAndCritDetermination();

        Trail.SetActive(true);
    }

    // ATTACK MODE

    // IDLE MODE
    public void SetSelfInIdleMode(){
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x , 0f , 0f);
        
        // FOR HOMING ATTACK
        // rb.velocity = Vector2.zero;
        // rb.angularVelocity = 0f;

        isInAttackMode = false;

        Trail.SetActive(false);
    }
    // IDLE MODE

    void Flip(){
        if(TargetPositionToMoveWhileIdle.x > transform.position.x && transform.localScale.x < 0){
            Vector3 scaler = transform.localScale;
            scaler.x *= -1;
            transform.localScale = scaler;
        }

        else if(TargetPositionToMoveWhileIdle.x < transform.position.x && transform.localScale.x > 0){
            Vector3 scaler = transform.localScale;
            scaler.x *= -1f;
            transform.localScale = scaler;
        }
    }


    public GameObject textPrefab;
    public bool isTheLastOneToAttack; // TO MAKE DURING TURN FALSE AFTER THE LAST LITTLE BAT
    void OnTriggerEnter2D(Collider2D col){
        if(col.gameObject.tag == "Ally"){
            // if(!col.gameObject.GetComponent<AllyTargeted>().isAllyTargeted) return;

            if(!isInAttackMode) return;

            if(isTheLastOneToAttack)
            turnManager.isDuringTurn = false;

            isTheLastOneToAttack = false;

            int missChance = col.gameObject.GetComponent<AllyStats>().Evasion;

            if(isCritic)
            Damage = Damage + ((Damage * critMultiplier) / 100);

            int def = col.gameObject.GetComponent<AllyStats>().Defence;
            Damage = Damage - ((Damage * def) / 100);

            int rnd = Random.Range(0 , 100);
            if(rnd < missChance){
                Damage = 0;
                // Debug.Log("Miss");
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

            // col.gameObject.GetComponent<AllyTargeted>().isAllyTargeted = false;

            // Debug.Log(col.gameObject.name);
            col.gameObject.GetComponent<AllyTakeDamage>().GetDamage(Damage);
            GameObject go = Instantiate(HitFx , transform.position , transform.rotation);
            Destroy(go , 5f);

            // IT KEEPS THE TIMER FROM BEFORE ATTACK MODE, MAKE IT ZERO SO IT WON'T WAIT TO RUSH BACK TO PARENT
            TimerForNewPositionWhileIdle = 0f;

            SetSelfInIdleMode();
        }
    }
}
