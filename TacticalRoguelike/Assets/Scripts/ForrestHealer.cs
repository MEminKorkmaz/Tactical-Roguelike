using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ForrestHealer : MonoBehaviour
{
    public TurnManager turnManager;

    public Animator anim;

    public GameObject MoveFxPrefab;
    public Pathfinding pf;
    public EnemyTakeDamage enemyTakeDamage;
    public EnemyAction enemyAction;

    public EnemyStats enemyStats;

    //public int MaxMove;

    private Camera Cam;

    private bool isMoving;
    private bool CanMove;

    public Ground ground;

    private int MoveListCount;

    public bool CanAction;

    public GameObject ProjectilePrefab;

    public Transform FirePoint;

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

    [Header("HEALING")]
    public float HealingRange;
    public int HealingAmountPercentage;

    public int HealingCooldown;
    private int TempHealingCounter;
    private bool isHealingInCooldown;

    [Header("UI")]
    public Text HealthBar;
    public GameObject HealthBarParent;

    [Header("MISSCELLANEOUS")]
    public float DelayTimeBeforeEachSkill;
    public bool isInAttackMode;

    void Awake(){
        ground = GameObject.FindWithTag("Map").GetComponent<Ground>();

        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();

        Cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        turnManager = GameObject.FindWithTag("GameManager").GetComponent<TurnManager>();

        anim = transform.GetChild(0).GetComponent<Animator>();

        enemyTakeDamage = this.GetComponent<EnemyTakeDamage>();

        enemyAction = this.GetComponent<EnemyAction>();

        enemyStats = this.GetComponent<EnemyStats>();
    }
    void Start()
    {
        // IF A FORREST ALLY HAS LOW HP, GO TO THEM AND HEAL -- IF AN ENEMY IS IN RANGE ATTACK -- IF NONE, STAY AWAY AS FAR
        var state = anim.GetCurrentAnimatorStateInfo(0);

        anim.Play(state.fullPathHash , 0 , Random.Range(0f , 1f));
    }


    // CHECK IF ATTACKING RANGE - IF NOT MOVE TO ATTACKING RANGE (MAYBE FARTHEST ATTACKING RANGE) -- CAN MOVE ? INSTEAD OF DURING TURN

    // IF MOVE TO ATTACK RANGE THEN ATTACK -- IF OUT OF SIGHT DONT GO TO ENEMY(MAYBE - TEST FIRST AFTER IF MOVE TO ATTACK RANGE ..)

    void Update()
    {
        // HealthBar.text = enemyTakeDamage.currentHealth.ToString();
        HealthBar.text = enemyStats.CurrentHealth.ToString();
        if(transform.localScale.x == -1f)
            HealthBarParent.transform.localScale = new Vector3(-1f , 1f , 0f);

        if(transform.localScale.x == 1f)
            HealthBarParent.transform.localScale = new Vector3(1f , 1f , 0f);

        CheckForCooldowns();

        if(turnManager.Turn == 1 || turnManager.isDuringTurn) return;

            if(!enemyAction.CanAction) return;
                EnemiesInSight.Clear();


                StartCoroutine("Skills");
                enemyAction.CanAction = false;
            // isHealingInRangeAndNoCooldown();
            // if(isHealingInRangeAndNoCooldownBool && !isHealingInCooldown)
            //     HealingSkill();

            // else if(!isHealingInRangeAndNoCooldownBool)
            //     MovementSkill();

            // isAttackInRange();
            // if(isAttackInRangeBool)
            //     AttackSkill();

                
            //     enemyAction.CanAction = false;
            //     enemyAction.isEnemyDone = true;
    }

    private bool isUsingSkill;
    IEnumerator Skills(){
        isUsingSkill = false;
        // int i = 0;
        while(!isUsingSkill){
            // i++;
            yield return new WaitForSeconds(0.25f);
            isUsingSkill = true;
            isHealingInRangeAndNoCooldown();
            if(isHealingInRangeAndNoCooldownBool && !isHealingInCooldown)
            {
                HealingSkill();

                enemyAction.isEnemyDone = true;
                yield return null; // ?
                // yield return new WaitForSeconds(DelayTimeBeforeEachSkill);
            }

            else if(!isHealingInRangeAndNoCooldownBool)
                MovementSkill();
                yield return new WaitForSeconds(DelayTimeBeforeEachSkill);

            isAttackInRange();
            if(isAttackInRangeBool)
                AttackSkill();

                
                // enemyAction.CanAction = false;
                // yield return new WaitForSeconds(1f);
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

        if(turnManager.TurnCounter > TempHealingCounter || TempHealingCounter == 0)
            isHealingInCooldown = false;
        else
            isHealingInCooldown = true;
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
        
        TempEnemyPos = new Vector3(TempClosestEnemy.position.x , TempClosestEnemy.position.y , TempClosestEnemy.position.z);

        if(TempEnemyPos.x <= transform.position.x && transform.localScale.x > 0){
        transform.localScale = new Vector3(-transform.localScale.x , transform.localScale.y , transform.localScale.z);
        }

        else if(TempEnemyPos.x > transform.position.x && transform.localScale.x < 0){
        transform.localScale = new Vector3(-transform.localScale.x , transform.localScale.y , transform.localScale.z);
        }

        anim.SetTrigger("Attack");
        Invoke(nameof(Attack) , 0.28f);
    }

    void Attack(){

        float rndForRotationOffset = 0f;

        rndForRotationOffset = Random.Range(2f , 4f);

        float distanceBetweenThisAndEnemyPos = Vector2.Distance(transform.position , TempEnemyPos);
        rndForRotationOffset *= distanceBetweenThisAndEnemyPos;

        float offset = -90f;
        Vector3 dir = (TempEnemyPos - FirePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        FirePoint.rotation = Quaternion.Euler(0f , 0f , (angle + offset) + rndForRotationOffset);
        
        GameObject go = Instantiate(ProjectilePrefab , FirePoint.position , FirePoint.rotation);

        go.GetComponent<ForrestHealerOrb>().Damage = enemyStats.Damage;
        go.GetComponent<ForrestHealerOrb>().critMultiplier = enemyStats.CritMultiplier;
        go.GetComponent<ForrestHealerOrb>().critChance = enemyStats.CritChance;

        go.GetComponent<ForrestHealerOrb>().EnemyPos = TempEnemyPos;

        EnemiesInSight.Clear();
    }


    // HEALING PART START !'^!'^!'^!'^!'^!'^!'^!'^!'^!'^!'^!'^!'
    private bool isHealingInRangeAndNoCooldownBool;
    private Vector3 AllyPos;
    private Transform TempAllyWithLeastHealthTransform;
    private GameObject[] Allies;
    private GameObject TempAllyToHeal;


    void isHealingInRangeAndNoCooldown(){
        TempAllyWithLeastHealthTransform = null;
        Allies = GameObject.FindGameObjectsWithTag("Enemy");

        // HEALING THE FORREST ALLY WITH LEAST HEALTH
        int LeastAllyHealth = 99999;

        // CHECKING THE CURRENT HEALTH WITH PERCENTAGE
        int TempAllyCurrentHealth = 0;
        int TempAllyMaxHealth = 0;
        
        for(int i = 0; i < Allies.Length; i++){
            TempAllyMaxHealth = Allies[i].GetComponent<EnemyStats>().MaxHealth;
            TempAllyCurrentHealth = Allies[i].GetComponent<EnemyStats>().CurrentHealth;

            TempAllyCurrentHealth = (TempAllyCurrentHealth * 100) / TempAllyMaxHealth;

            if(TempAllyCurrentHealth < LeastAllyHealth){
                LeastAllyHealth = TempAllyCurrentHealth;
                if(LeastAllyHealth <= 50){
                    TempAllyWithLeastHealthTransform = Allies[i].transform;
                    isInAttackMode = false;
                }
            }
        }

        if(TempAllyWithLeastHealthTransform != null)
        {
            float distance = Vector2.Distance(transform.position , TempAllyWithLeastHealthTransform.position);

            if(distance <= HealingRange)
            {
                // ACTUAL HEALING (CHECK FOR RANGE FIRST)
                    if(TempAllyWithLeastHealthTransform != null)
                    {
                        TempAllyToHeal = TempAllyWithLeastHealthTransform.gameObject;
                        isHealingInRangeAndNoCooldownBool = true;

                        TempAllyWithLeastHealthTransform = null;
                        
                        // isInAttackMode = false;
                }
            }
        }

        else{
            isHealingInRangeAndNoCooldownBool = false;
            isInAttackMode = true;
        }
    }


    void HealingSkill(){
        anim.SetTrigger("Attack");
        Invoke(nameof(Healing) , 0.28f);

        turnManager.isDuringTurn = true;
    }

    void Healing(){
        int HealingAmount;
        HealingAmount = (TempAllyToHeal.GetComponent<EnemyStats>().MaxHealth * HealingAmountPercentage) / 100;
        TempAllyToHeal.GetComponent<EnemyBuffs>().Healing(HealingAmount);

        TempHealingCounter = turnManager.TurnCounter + HealingCooldown;

        if(TempAllyToHeal.transform.position.x > transform.position.x){
            Vector3 scaler = transform.localScale;
            scaler.x = 1;
            transform.localScale = scaler;
        }

        else if(TempAllyToHeal.transform.position.x < transform.position.x){
            Vector3 scaler = transform.localScale;
            scaler.x = -1f;
            transform.localScale = scaler;
        }

        TempAllyToHeal = null;

        turnManager.isDuringTurn = false;
    }

    // HEALING PART END !'^!'^!'^!'^!'^!'^!'^!'^!'^!'^!'^!'^!'


    // private bool isHealingInRangeAndNoCooldownBool;
    // private Vector3 AllyPos;
    // private Transform TempAllyWithLeastHealthTransform;
    // private GameObject[] Allies;
    // private GameObject TempAllyToHeal;


    // void isHealingInRangeAndNoCooldown(){
    //     TempAllyWithLeastHealthTransform = null;
    //     Allies = GameObject.FindGameObjectsWithTag("Enemy");

    //     // HEALING THE FORREST ALLY WITH LEAST HEALTH
    //     int LeastAllyHealth = 99999;

    //     // CHECKING THE CURRENT HEALTH WITH PERCENTAGE
    //     int TempAllyCurrentHealth = 0;
    //     int TempAllyMaxHealth = 0;
        
    //     for(int i = 0; i < Allies.Length; i++){
    //         TempAllyMaxHealth = Allies[i].GetComponent<EnemyStats>().MaxHealth;
    //         TempAllyCurrentHealth = Allies[i].GetComponent<EnemyStats>().CurrentHealth;

    //         TempAllyCurrentHealth = (TempAllyCurrentHealth * 100) / TempAllyMaxHealth;

    //         if(TempAllyCurrentHealth < LeastAllyHealth){
    //             LeastAllyHealth = TempAllyCurrentHealth;
    //             if(LeastAllyHealth <= 50){
    //                 TempAllyWithLeastHealthTransform = Allies[i].transform;
    //                 isInAttackMode = false;
    //             }
    //         }
    //     }

    //     if(TempAllyWithLeastHealthTransform != null)
    //     {
    //         float distance = Vector2.Distance(transform.position , TempAllyWithLeastHealthTransform.position);

    //         if(distance <= HealingRange)
    //         {
    //             // ACTUAL HEALING (CHECK FOR RANGE FIRST)
    //                 if(TempAllyWithLeastHealthTransform != null)
    //                 {
    //                     TempAllyToHeal = TempAllyWithLeastHealthTransform.gameObject;
    //                     isHealingInRangeAndNoCooldownBool = true;

    //                     TempAllyWithLeastHealthTransform = null;
                        
    //                     // isInAttackMode = false;
    //             }
    //         }
    //     }

    //     else{
    //         isHealingInRangeAndNoCooldownBool = false;
    //         isInAttackMode = true;
    //     }
    // }



    // GET DISTANCE OF ALL THE ALLIES -- SHOOT THE CLOSEST -- IF NO CLOSE ENOUGH, ATTACK
    private bool isAttackInRangeBool;
    private Vector3 TempEnemyPos;
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



    public Transform GetClosestAlly()
    {
        GameObject[] allies;
        allies = GameObject.FindGameObjectsWithTag("Enemy");
        float closestDistance = Mathf.Infinity;
        Transform trans = null;

        foreach(GameObject go in allies)
        {
            if(go == this.gameObject || go.transform.GetChild(0).name == "ForrestHealerSprite"){
            continue;
            }

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


    private List<float> TempAllyMovementTileDistance = new List<float>();
    private GameObject TempGo;
    private Vector3 TempPos;
    private float closestDistance;
    private int TempTile;


    public void MovementSkill(){
        
        if(isMovementInCooldown){
            ClearingListsAndTemps();
            return;
        }

        if(MovementCooldown != 0)
        TempMovementCounter = turnManager.TurnCounter + MovementCooldown;


        Transform TempTransformForClosestAlly = GetClosestAlly();

        if(TempTransformForClosestAlly == null)
        return; // IF NO ALLY DON'T MOVE

        TempTile = ground.GetWhichTile(this.gameObject);

        pf.EnemyFindPaths(ground.children[TempTile].gameObject , MoveRange , true);

        if(!isInAttackMode)
        {
            for(int i = 0; i < pf.EnemyActionTile.Count; i++)
            {
                if(TempTransformForClosestAlly != null)
                {
                    float distance = Vector2.Distance(pf.EnemyActionTile[i].transform.position , TempTransformForClosestAlly.position);
                    TempAllyMovementTileDistance.Add(distance);
                }
            }
        }

        else
        {
            for(int i = 0; i < pf.EnemyActionTile.Count; i++)
            {
                float distance = Vector2.Distance(pf.EnemyActionTile[i].transform.position , TempTransformForClosestAlly.position);
                TempAllyMovementTileDistance.Add(distance);
            }
        }
        // for(int i = 0; i < pf.EnemyActionTile.Count; i++)
        // {
        //     float distance = Vector2.Distance(pf.EnemyActionTile[i].transform.position , TempTransformForClosestAlly.position);
        //     TempAllyMovementTileDistance.Add(distance);
        // }

        if(TempAllyMovementTileDistance.Count == 0)
        {
            ClearingListsAndTemps();
            return;
        }

        closestDistance = TempAllyMovementTileDistance[0];

        for(int i = 0; i < TempAllyMovementTileDistance.Count; i++){
            if(TempAllyMovementTileDistance[i] < closestDistance && TempAllyMovementTileDistance[i] > ProtectedRange){
                closestDistance = TempAllyMovementTileDistance[i];
                TempGo = pf.EnemyActionTile[i];
            }
        }

        if(TempGo == null){
            ClearingListsAndTemps();
            return;
        }
        
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
            
                ClearingListsAndTemps();
                
                turnManager.EnemyEnergy--;
                turnManager.isDuringTurn = false;
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
        TempAllyMovementTileDistance.Clear();
        pf.EnemyFrontier.Clear();
        pf.EnemyActionTile.Clear();
        ground.UnHighlighTheMovableGrids();
        EnemiesInSight.Clear();
        TempGo = null; // TILE TO MOVE, IF NO MOVEMENT, MAKE TEMPGO NULL SO TEMPGO WON'T BE THE PREVIOUS TILE
    }
}
