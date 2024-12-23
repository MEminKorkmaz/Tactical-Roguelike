using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSkills : MonoBehaviour
{

    [Header("MISCELLANEOUS")]
    public TurnManager turnManager;
    public Transform FirePoint;

    public Ground ground;
    public GameObject TileThisObjectIsOn;

    public Archer archer;
    public Animator anim;

    public AllyStats allyStats;

    [Header("MOVEMENT")]
    public int MoveRange;
    public float Speed;
    public bool isMoveSkillSelected;
    private string MovementColor = "MovementColor";
    public int MovementCoolDown;
    public bool isMovementInCooldown;

    [Header("NORMAL ATTACK")]
    public GameObject NormalAttackProjectilePrefab;
    public int NormalAttackRange;
    private string NormalAttackColor = "NormalAttackColor";
    public bool isNormalAttackSelected;
    public int NormalAttackCooldown;
    public bool isNormalAttackInCooldown;
    private int TempNormalAttackTurnCounter;



    void Awake(){
        ground = GameObject.FindWithTag("Map").GetComponent<Ground>();

        archer = GetComponent<Archer>();

        turnManager = GameObject.FindWithTag("GameManager").GetComponent<TurnManager>();

        // anim = transform.GetChild(0).GetComponent<Animator>();

        allyStats = this.GetComponent<AllyStats>();
    }

    void Start()
    {
        anim = transform.GetChild(0).GetComponent<Animator>();
    }


    void Update()
    {
        CheckForCooldowns();
    }

    void CheckForCooldowns(){
        if(turnManager.TurnCounter > TempNormalAttackTurnCounter || TempNormalAttackTurnCounter == 0){
            isNormalAttackInCooldown = false;
        }

        else{
            isNormalAttackInCooldown = true;
        }
    }


    // MOVEMENT
    public void MoveSkill(){
        ground.HighlightTheMovableGrids(TileThisObjectIsOn , MoveRange , MovementColor , true);
        isMoveSkillSelected = true;
    }


    // NORMAL ATTACK
    public void NormalAttackSkill(){
        ground.HighlightTheMovableGrids(TileThisObjectIsOn , NormalAttackRange , NormalAttackColor , false);
        isNormalAttackSelected = true;
    }

    private Vector3 EnemyPos;
    public void NormalAttack(Vector3 Pos){
        anim.SetTrigger("Attack");
        EnemyPos = new Vector3(Pos.x , Pos.y + 5f , 0f);
        Invoke("ArrowCreating" , 0.5f);

        ground.UnHighlighTheMovableGrids();

        if(NormalAttackCooldown == 0) return;
        TempNormalAttackTurnCounter = turnManager.TurnCounter + NormalAttackCooldown;
        Debug.Log(TempNormalAttackTurnCounter);
        // TEMPORARY

        // GameObject go = Instantiate(NormalAttackProjectilePrefab , EnemyPos , Quaternion.Euler(0f , 0f , 180f));
        // ground.UnHighlighTheMovableGrids();

        // TEMPORARY
    }

    // TEMPORARY
    void ArrowCreating(){
        // GameObject go = Instantiate(NormalAttackProjectilePrefab , EnemyPos , Quaternion.Euler(0f , 0f , 180f));

        Vector3 TempEnemyPosition = new Vector3(EnemyPos.x , EnemyPos.y - 5f , 0f);

        float rndForRotationOffset = 0f;

        rndForRotationOffset = Random.Range(2f , 4f);

        // rndForRotationOffset = Random.Range(7f , 14f);

        float distanceBetweenThisAndEnemyPos = Vector2.Distance(transform.position , TempEnemyPosition);
        rndForRotationOffset *= distanceBetweenThisAndEnemyPos;

        float offset = -90f;
        Vector3 dir = (TempEnemyPosition - FirePoint.position).normalized;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        FirePoint.rotation = Quaternion.Euler(0f , 0f , (angle + offset) + rndForRotationOffset);
        
        GameObject go = Instantiate(NormalAttackProjectilePrefab , FirePoint.position , FirePoint.rotation);

        go.GetComponent<ArcherProjectile>().Damage = allyStats.Damage;
        go.GetComponent<ArcherProjectile>().critMultiplier = allyStats.CritMultiplier;
        go.GetComponent<ArcherProjectile>().critChance = allyStats.CritChance;

        go.GetComponent<ArcherProjectile>().EnemyPos = TempEnemyPosition;

        // go.transform.SetParent(this.transform);
        // ground.UnHighlighTheMovableGrids();
    }
    // TEMPORARY

    void OnTriggerStay2D(Collider2D col){
        TileThisObjectIsOn = col.gameObject;
    }
}
