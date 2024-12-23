using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Healer : MonoBehaviour
{
    public TurnManager turnManager;

    public GameObject MoveFxPrefab;
    public HealerSkills healerSkills;
    public Pathfinding pf;

    public AllyIsSelected allyIsSelected;

    //public int MaxMove;

    private Camera Cam;

    private bool isMoving;
    
    public Ground ground;

    private int MoveListCount;

    public GameObject OutOfSightTextPrefab;

    public Text HealthBar;
    public GameObject HealthBarParent;

    public AllyTakeDamage allyTakeDamage;

    public AllyStats allyStats;

    private bool isEnemyFound;
    private bool isAllyFound;

    public GameObject SelectedTriangle;


    void Awake(){
        ground = GameObject.FindWithTag("Map").GetComponent<Ground>();

        healerSkills = GetComponent<HealerSkills>();
        pf = GameObject.FindWithTag("Pathfinding").GetComponent<Pathfinding>();

        Cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();

        turnManager = GameObject.FindWithTag("GameManager").GetComponent<TurnManager>();

        allyTakeDamage = this.GetComponent<AllyTakeDamage>();

        allyIsSelected = this.GetComponent<AllyIsSelected>();

        allyStats = this.GetComponent<AllyStats>();
    }
    void Start()
    {
        SelectedTriangle.SetActive(false);
    }


    // ENERGY CAUSE PROBLEM, MAKE ENEMY ENERGY INFINITE
    void Update()
    {
        HealthBar.text = allyStats.CurrentHealth.ToString();
        if(transform.localScale.x == -1f)
        HealthBarParent.transform.localScale = new Vector3(-1f , 1f , 0f);

        if(transform.localScale.x == 1f)
        HealthBarParent.transform.localScale = new Vector3(1f , 1f , 0f);

        if(!allyIsSelected.isSelected)
        {
            SelectedTriangle.SetActive(false);
            return;
        }

        if(turnManager.isDuringTurn || turnManager.Turn == -1)
            SelectedTriangle.SetActive(false);
        else
            SelectedTriangle.SetActive(true);
        // Movement();

        if(turnManager.Turn == -1 || turnManager.isDuringTurn) return;
        MouseDetection();

        // if(!allyIsSelected) return;

        if(Input.GetKeyDown(KeyCode.Alpha1)){
            if(healerSkills.isMovementInCooldown)
            {
                GameObject go = Instantiate(OutOfSightTextPrefab , transform.position , Quaternion.identity);
                go.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(4f , 2f);
                go.transform.gameObject.GetComponent<RectTransform>().localScale = new Vector2(1f , 1f);
                go.transform.GetChild(0).GetComponent<Text>().text = "SKILL IS IN COOLDOWN";
                Destroy(go , 3f);
                return;
            }
            DestroyHighlight();
            healerSkills.MoveSkill();
        }

        if(Input.GetKeyDown(KeyCode.Alpha2)){
            if(healerSkills.isNormalAttackInCooldown)
            {
                GameObject go = Instantiate(OutOfSightTextPrefab , transform.position , Quaternion.identity);
                go.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(4f , 2f);
                go.transform.gameObject.GetComponent<RectTransform>().localScale = new Vector2(1f , 1f);
                go.transform.GetChild(0).GetComponent<Text>().text = "SKILL IS IN COOLDOWN";
                Destroy(go , 3f);
                return;
            }
            DestroyHighlight();
            healerSkills.NormalAttackSkill();
        }

        if(Input.GetKeyDown(KeyCode.Alpha3)){
            if(healerSkills.isHealingInCooldown)
            {
                GameObject go = Instantiate(OutOfSightTextPrefab , transform.position , Quaternion.identity);
                go.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(4f , 2f);
                go.transform.gameObject.GetComponent<RectTransform>().localScale = new Vector2(1f , 1f);
                go.transform.GetChild(0).GetComponent<Text>().text = "SKILL IS IN COOLDOWN";
                Destroy(go , 3f);
                return;
            }
            DestroyHighlight();
            turnManager.isDuringChannel = true; // Healer is choosing an ally to heal not to 'select'
            healerSkills.HealingSkill();
        }
    }

    void FixedUpdate(){
        if(!allyIsSelected.isSelected) return;
        Movement();
    }


    void MouseDetection(){
        if(Input.GetMouseButtonDown(0)){
        Collider2D hit = null;
        // Collider2D tempHit = null;
        RaycastHit2D[] hit2 = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

        for(int i = 0; i < hit2.Length; i++){
        if(hit2[i].collider != null)
            {
                // if(hit2[i].collider.tag == "Tile"){
                //     tempHit = hit2[i].collider;
                // }
                if(hit2[i].collider.tag == "Enemy" || hit2[i].collider.tag == "Ally")
                {
                hit = hit2[i].collider;
                break;
                }
                else{
                    hit = hit2[i].collider;
                }
            }
        }

        if(hit == null) return;

        if(hit.tag == "Tile")
        {
            // if(healerSkills.isMoveSkillSelected && hit.gameObject.GetComponent<Tiles>().isMovable)
            if(healerSkills.isMoveSkillSelected && hit.gameObject.GetComponent<Tiles>().isActionable)
            {
            healerSkills.isMoveSkillSelected = false;

            // if(hit.gameObject.GetComponent<Tiles>().isMovable)
            if(hit.gameObject.GetComponent<Tiles>().isActionable)
                {
                    int Temp = ground.GetWhichTile(this.gameObject);

                    pf.MakePath(hit.gameObject , ground.children[Temp].gameObject);
                    MoveListCount = 0;
                    
                    // ground.UnHighlighTheMovableGrids();
                    DestroyHighlight();
                }
            }
        }


        if(hit.tag == "Enemy" && healerSkills.isNormalAttackSelected)
            {
                // float distance = Vector2.Distance(transform.position , hit.transform.position);
                // if(distance > healerSkills.NormalAttackRange)
                // return;

                // if(!hit.gameObject.GetComponent<EnemyAttackableCheck>().CurrentTile.GetComponent<Tiles>().isAttackable)
                if(!hit.gameObject.GetComponent<EnemyActionableCheck>().CurrentTile.GetComponent<Tiles>().isActionable)
                return;
                
                RaycastHit2D[] hitLOS;
                hitLOS = Physics2D.RaycastAll(transform.position, hit.transform.position - transform.position);
                for(int i = 0; i < hitLOS.Length; i++)
                {
                    if(isEnemyFound) break; // ENEMY ARCHER TOO
                    if(hitLOS[i].collider.tag == "Enemy"){
                        isEnemyFound = true;
                        for(int j = 0; j < i; j++){
                            if(hitLOS[j].collider.tag == "Obstacle")
                            {
                                GameObject go = Instantiate(OutOfSightTextPrefab , transform.position , Quaternion.identity);
                                go.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(2f , 1f);
                                go.transform.gameObject.GetComponent<RectTransform>().localScale = new Vector2(1f , 1f);
                                go.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Out Of Sight";
                                Destroy(go , 5f);
                                
                                isEnemyFound = false;
                                return;
                                // Not in line of sight
                            }
                        }
                    }
                }

                isEnemyFound = false;

                if(hit.transform.position.x <= transform.position.x && transform.localScale.x > 0){
                    transform.localScale = new Vector3(-transform.localScale.x , transform.localScale.y , transform.localScale.z);
                    }

                else if(hit.transform.position.x > transform.position.x && transform.localScale.x < 0){
                    transform.localScale = new Vector3(-transform.localScale.x , transform.localScale.y , transform.localScale.z);
                    }
                
                turnManager.AllyEnergy--;
                turnManager.isDuringTurn = true;
                hit.gameObject.GetComponent<EnemyTargeted>().isEnemyTargeted = true;

                healerSkills.isNormalAttackSelected = false;
                Vector3 EnemyPos = new Vector3(hit.transform.position.x , hit.transform.position.y , 0f);
                
                healerSkills.NormalAttack(EnemyPos);
            }
        
        // Debug.Log(hit.tag);
        if(hit.tag == "Ally" && healerSkills.isHealingSelected)
            {
                // if(hit.gameObject.GetComponent<EnemyAttackableCheck>().CurrentTile.GetComponent<Tiles>().isAttackable) return;
                
                if(!hit.gameObject.GetComponent<AllyActionableCheck>().CurrentTile.GetComponent<Tiles>().isActionable) return;

                RaycastHit2D[] hitLOS;
                hitLOS = Physics2D.RaycastAll(transform.position, hit.transform.position - transform.position);
                for(int i = 1; i < hitLOS.Length; i++) // I = 1 because healer himself is an ally and it needs to be ignored
                {
                    if(isAllyFound) break; // ENEMY ARCHER TOO
                    if(hitLOS[i].collider.tag == "Ally"){
                        isAllyFound = true;
                        for(int j = 0; j < i; j++){
                            if(hitLOS[j].collider.tag == "Obstacle")
                            {
                                GameObject go = Instantiate(OutOfSightTextPrefab , transform.position , Quaternion.identity);
                                go.transform.GetChild(0).gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(2f , 1f);
                                go.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Out Of Sight";
                                Destroy(go , 5f);
                                
                                isAllyFound = false;
                                Debug.Log("MAAAAAN");
                                return;
                                // Not in line of sight
                            }
                        }
                    }
                }

                isAllyFound = false;

                if(hit.transform.position.x <= transform.position.x && transform.localScale.x > 0){
                    transform.localScale = new Vector3(-transform.localScale.x , transform.localScale.y , transform.localScale.z);
                    }

                else if(hit.transform.position.x > transform.position.x && transform.localScale.x < 0){
                    transform.localScale = new Vector3(-transform.localScale.x , transform.localScale.y , transform.localScale.z);
                    }
                // hit.gameObject.GetComponent<AllyBuffs>().Healing(hit.gameObject.GetComponent<AllyStats>().MaxHealth / 10);
                turnManager.AllyEnergy--;
                turnManager.isDuringTurn = true;
                healerSkills.isHealingSelected = false;
                healerSkills.Healing(hit.gameObject);
            }
        }

        if(Input.GetMouseButtonDown(1)){
            // ground.UnHighlighTheMovableGrids();
            turnManager.isDuringChannel = false; // If ally isn't using skill then make channel false
            DestroyHighlight();
        }
    }

    void DestroyHighlight(){
        ground.UnHighlighTheMovableGrids();

        healerSkills.isMoveSkillSelected = false;
        healerSkills.isNormalAttackSelected = false;

        turnManager.isDuringChannel = false;
    }



    private Vector3 TempPos;
    void Movement(){

        if(MoveListCount < pf.PathTiles.Count)
        {
        isMoving = true;

        healerSkills.anim.SetFloat("Move" , 1);
        TempPos = new Vector3(pf.PathTiles[MoveListCount].transform.position.x ,
        pf.PathTiles[MoveListCount].transform.position.y , 0f);

        transform.position = Vector2.MoveTowards(transform.position , TempPos ,
        healerSkills.Speed * Time.deltaTime);

            float distance = Vector2.Distance(transform.position , TempPos);
            if(transform.position == TempPos)
            {
                MoveListCount++;
            }
            if(transform.position == pf.PathTiles[pf.PathTiles.Count - 1].transform.position){
                isMoving = false;

                healerSkills.anim.SetFloat("Move" , 0);
            
                // transform.localScale = new Vector3(1f , 1f , 1f);

                pf.PathTiles.Clear();
                pf.Frontier.Clear();
                
                turnManager.AllyEnergy--;
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
}
