﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForrestBigGuy : MonoBehaviour
{
    public TurnManager turnManager;

    public Animator anim;

    public GameObject MoveFxPrefab;
    public Pathfinding pf;
    public EnemyTakeDamage enemyTakeDamage;
    public EnemyAction enemyAction;
    public EnemyAIManager enemyAIManager;

    public EnemyStats enemyStats;

    //public int MaxMove;

    private Camera Cam;

    private bool isMoving;
    private bool CanMove;

    public Ground ground;

    private int MoveListCount;

    public bool CanAction;

    public GameObject ArrowPrefab;

    public float Speed;

    [Header("MOVEMENT")]
    public int MoveRange;
    public int MovementCooldown;
    private int TempMovementCounter;
    private bool isMovementInCooldown;

    [Header("ATTACK")]

    public float AttackRange;
    public float ProtectedRange;
    public int AttackCooldown;
    private int TempAttackCounter;
    private bool isAttackInCooldown;

    [Header("UI")]
    public Text HealthBar;
    public GameObject HealthBarParent;

    [Header("MISSCELLANEOUS")]
    public float DelayTimeBeforeEachSkill;

    void Awake(){
        ground = GameObject.FindWithTag("Map").GetComponent<Ground>();

        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();

        Cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        turnManager = GameObject.FindWithTag("GameManager").GetComponent<TurnManager>();

        anim = transform.GetChild(0).GetComponent<Animator>();

        enemyTakeDamage = this.GetComponent<EnemyTakeDamage>();

        enemyAction = this.GetComponent<EnemyAction>();

        enemyAIManager = GameObject.FindWithTag("GameManager").GetComponent<EnemyAIManager>();

        enemyStats = this.GetComponent<EnemyStats>();
    }
    void Start()
    {
        var state = anim.GetCurrentAnimatorStateInfo(0);

        anim.Play(state.fullPathHash , 0 , Random.Range(0f , 1f));
    }


    // CHECK IF ATTACKING RANGE - IF NOT MOVE TO ATTACKING RANGE (MAYBE FARTHEST ATTACKING RANGE) -- CAN MOVE ? INSTEAD OF DURING TURN
    void Update()
    {
        // HealthBar.text = enemyTakeDamage.currentHealth.ToString();
        HealthBar.text = enemyStats.CurrentHealth.ToString();
        if(transform.localScale.x == -1f)
        HealthBarParent.transform.localScale = new Vector3(-1f , 1f , 0f);

        if(transform.localScale.x == 1f)
        HealthBarParent.transform.localScale = new Vector3(1f , 1f , 0f);
        // if(CanMove){
        //     Move();
        // }

        CheckForCooldowns();

        if(turnManager.Turn == 1 || turnManager.isDuringTurn) return;

        // if(Input.GetKeyDown(KeyCode.Alpha4)){
        //     MovementSkill();
        // }

        // if(Input.GetKeyDown(KeyCode.Alpha1)){
        //     CanAction = true;
        // }

        // if(Input.GetKeyDown(KeyCode.Alpha5)){
            if(!enemyAction.CanAction) return;
            EnemiesInSight.Clear();

            StartCoroutine("Skills");
            // isAttackInRange();
            // if(isAttackInRangeBool)
            // AttackSkill();
            // else if(!isAttackInRangeBool)
            // MovementSkill();

            enemyAction.CanAction = false;
            // enemyAction.isEnemyDone = true;
        // }
    }

    private bool isUsingSkill;
    IEnumerator Skills(){
        isUsingSkill = false;
        // int i = 0;
        while(!isUsingSkill){
            // i++;
            yield return new WaitForSeconds(0.25f);
            isUsingSkill = true;
        
            isAttackInRange();
            if(isAttackInRangeBool){
                AttackSkill();

            yield return new WaitForSeconds(DelayTimeBeforeEachSkill);
            }

            else if(!isAttackInRangeBool)
                MovementSkill();

            yield return new WaitForSeconds(DelayTimeBeforeEachSkill);
                
                // enemyAction.CanAction = false;
                // yield return new WaitForSeconds(0.25f);
                enemyAction.isEnemyDone = true;
                }
    }

    void FixedUpdate(){
        if(CanMove){
            Move();
        }
    }

    void CheckForCooldowns(){
        if(turnManager.TurnCounter > TempMovementCounter || TempMovementCounter == 0)
            isMovementInCooldown = false;
        else
            isMovementInCooldown = true;

        if(turnManager.TurnCounter > TempAttackCounter || TempAttackCounter == 0)
            isAttackInCooldown = false;
        else
            isAttackInCooldown = true;
    }


    void AttackSkill(){

        if(isAttackInCooldown){
            ClearingListsAndTemps();
            return;
        }

        if(AttackCooldown != 0)
        TempAttackCounter = turnManager.TurnCounter + AttackCooldown;

        turnManager.EnemyEnergy--;
        turnManager.isDuringTurn = true;
        
        TempAllyPos = new Vector3(TempClosestEnemy.position.x , TempClosestEnemy.position.y + 0.25f , TempClosestEnemy.position.z);

        if(TempAllyPos.x <= transform.position.x && transform.localScale.x > 0){
        transform.localScale = new Vector3(-transform.localScale.x , transform.localScale.y , transform.localScale.z);
        }

        else if(TempAllyPos.x > transform.position.x && transform.localScale.x < 0){
        transform.localScale = new Vector3(-transform.localScale.x , transform.localScale.y , transform.localScale.z);
        }

        anim.SetTrigger("Attack");
        Invoke(nameof(Attack) , 0.2f);
    }

    void Attack(){
        GameObject go = Instantiate(ArrowPrefab , TempAllyPos , Quaternion.Euler(0f , 0f , 180f) , this.transform);

        go.GetComponent<ForrestBigGuyProjectile>().Damage = enemyStats.Damage;
        go.GetComponent<ForrestBigGuyProjectile>().critMultiplier = enemyStats.CritMultiplier;
        go.GetComponent<ForrestBigGuyProjectile>().critChance = enemyStats.CritChance;
    }



    // GET DISTANCE OF ALL THE ALLIES -- SHOOT THE CLOSEST -- IF NO CLOSE ENOUGH, ATTACK
    private bool isAttackInRangeBool;
    // private GameObject TempAlly;
    private Vector3 TempAllyPos;
    private Transform TempClosestEnemy;
    private GameObject[] Enemies;
    private List<GameObject> EnemiesInSight = new List<GameObject>();
    private float distanceOfEnemyInSight;
    private bool isEnemySeen;
    public void isAttackInRange(){
        TempClosestEnemy = null;
        Enemies = GameObject.FindGameObjectsWithTag("Ally");

        for(int i = 0; i < Enemies.Length; i++){
            RaycastHit2D[] hitLOS;
                    hitLOS = Physics2D.RaycastAll(transform.position, Enemies[i].transform.position - transform.position);
                    for(int j = 0; j < hitLOS.Length; j++)
                    {
                        if(hitLOS[j].collider.tag == "Ally"){
                            isEnemySeen = true;
                                for(int k = 0; k < j; k++){
                                if(hitLOS[k].collider.tag == "Obstacle")
                                {
                                    isEnemySeen = false;
                                    // Not in line of sight
                                }
                            }
                                if(isEnemySeen){
                                    EnemiesInSight.Add(Enemies[i]);
                                }
                        }
                    }
                }

                if(EnemiesInSight.Count == 0){
                    isAttackInRangeBool = false;
                    return;
                }

                float closestDistance = Mathf.Infinity;

                for(int i = 0; i < EnemiesInSight.Count; i++){
                    distanceOfEnemyInSight = Vector2.Distance(transform.position , EnemiesInSight[i].transform.position);
                    if(distanceOfEnemyInSight < closestDistance && distanceOfEnemyInSight <= AttackRange)
                    {
                        closestDistance = distanceOfEnemyInSight;
                        TempClosestEnemy = EnemiesInSight[i].transform;
                    }
                }

                if(TempClosestEnemy != null){
                TempClosestEnemy.gameObject.GetComponent<AllyTargeted>().isAllyTargeted = true;
                isAttackInRangeBool = true;
                }

                else
                isAttackInRangeBool = false;
    }



    public Transform GetClosestEnemy()
    {
        GameObject[] allies;
        allies = GameObject.FindGameObjectsWithTag("Ally");
        float closestDistance = Mathf.Infinity;
        Transform trans = null;

        foreach(GameObject go in allies)
        {
            float currentDistance;
            currentDistance = Vector3.Distance(transform.position , go.transform.position);
            if(currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                trans = go.transform;
            }
        }
        return trans;
    }


    private List<float> TempEnemyMovementTileDistance = new List<float>();
    private GameObject TempGo;
    private Vector3 TempPos;
    private float closestDistance;
    private int TempTile;
    private bool isOutOfSight;


    public void MovementSkill(){

        if(isMovementInCooldown){
            ClearingListsAndTemps();
            return;
        }

        if(MovementCooldown != 0)
        TempMovementCounter = turnManager.TurnCounter + MovementCooldown;
        
        Transform TempTransformForClosestEnemy = GetClosestEnemy();

        TempTile = ground.GetWhichTile(this.gameObject);

        pf.EnemyFindPaths(ground.children[TempTile].gameObject , MoveRange , true);

        // ENEMY MOVABLE

        for(int i = 0; i < pf.EnemyActionTile.Count; i++){
            if(TempTransformForClosestEnemy != null)
            {
                float distance = Vector2.Distance(pf.EnemyActionTile[i].transform.position , TempTransformForClosestEnemy.position);
                TempEnemyMovementTileDistance.Add(distance);
            }
        }

        if(TempEnemyMovementTileDistance.Count == 0){
            ClearingListsAndTemps();
            return;
        }

        closestDistance = TempEnemyMovementTileDistance[0];

        for(int i = 0; i < TempEnemyMovementTileDistance.Count; i++){
            if(TempEnemyMovementTileDistance[i] < closestDistance && TempEnemyMovementTileDistance[i] > ProtectedRange){
                closestDistance = TempEnemyMovementTileDistance[i];
                TempGo = pf.EnemyActionTile[i];
            }
        }
        // If closest tile is the tile this character is currently on
        if(closestDistance == TempEnemyMovementTileDistance[0])
            TempGo = pf.EnemyActionTile[0];

        CanMove = true;

        pf.EnemyMakePath(TempGo , ground.children[TempTile].gameObject);
        MoveListCount = 0;
    }

    void Move(){
        if(MoveListCount < pf.EnemyPathTiles.Count)
        {
        isMoving = true;

        anim.SetFloat("Move" , 1);

        TempPos = new Vector3(pf.EnemyPathTiles[MoveListCount].transform.position.x ,
        pf.EnemyPathTiles[MoveListCount].transform.position.y , 0f);

        transform.position = Vector2.MoveTowards(transform.position , TempPos , Speed * Time.deltaTime);

            float distance = Vector2.Distance(transform.position , TempPos);
            if(transform.position == TempPos)
            {
                MoveListCount++;
            }
            if(transform.position == pf.EnemyPathTiles[pf.EnemyPathTiles.Count - 1].transform.position){
                isMoving = false;
                CanMove = false;

                anim.SetFloat("Move" , 0);
            
                // transform.localScale = new Vector3(1f , 1f , 1f);

                ClearingListsAndTemps();
                
                turnManager.EnemyEnergy--;
                turnManager.isDuringTurn = false;

                isAttackInRange();
                if(isAttackInRangeBool)
                AttackSkill();
            }
            else{
                turnManager.isDuringTurn = true;
            }
        }

        if(TempPos.x > transform.position.x && isMoving){
            Vector3 scaler = transform.localScale;
            scaler.x = 1;
            transform.localScale = scaler;
        }

        else if(TempPos.x < transform.position.x && isMoving){
            Vector3 scaler = transform.localScale;
            scaler.x = -1f;
            transform.localScale = scaler;
        }
    }

    void ClearingListsAndTemps()
    {
        CanMove = false;
        pf.EnemyPathTiles.Clear();
        TempEnemyMovementTileDistance.Clear();
        pf.EnemyFrontier.Clear();
        pf.EnemyActionTile.Clear();
        ground.UnHighlighTheMovableGrids();
        EnemiesInSight.Clear();
        TempGo = null; // TILE TO MOVE, IF NO MOVEMENT, MAKE TEMPGO NULL SO TEMPGO WON'T BE THE PREVIOUS TILE
    }
}
