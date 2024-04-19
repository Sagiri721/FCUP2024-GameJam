using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : MonoBehaviour
{
    private Transform player;
    public StateMachine monsterState = StateMachine.WANDER;

    public enum StateMachine {
        WANDER,
        CHASE,
        STOP,
        DEAD,
        DRAG,
        SHOCK
    }

    public EnemyStats enemyStats;
    public Vector2[] path;
    public float nextPointSnap = 0.01f;

    public enum KillReward {
        KILLRANGE,
        DRAGSPEED,
        RUNSPEED,
        ENDURANCE
    }
    public KillReward killReward;

    private Vector2 startPosition;
    private Vector3 direction, spawn;
    private int pointer = 0;

    private Animator effects;

    public Vector3 dragOffset;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        startPosition = transform.position;

        effects = GetComponent<Animator>();
        spawn = transform.position;
    }

    void Update()
    {
        float distanceFromPlayer = (player.position - transform.position).magnitude;

        if(monsterState != StateMachine.DEAD && monsterState != StateMachine.DRAG){

            if(distanceFromPlayer < enemyStats.killRadius * player.GetComponent<PlayerController>().stats.killRangeMult){
                if (Utils.GetKeyDownAll(player.gameObject.GetComponent<PlayerController>().stats.actionKeys) && 
                            (monsterState == StateMachine.WANDER || monsterState == StateMachine.STOP)){

                    GameObject effect = player.GetComponent<PlayerController>().biteEffect;
                    Instantiate(effect, transform.position, Quaternion.identity);
                    monsterState = StateMachine.DEAD;

                    switch (killReward){
                        case KillReward.KILLRANGE: player.GetComponent<PlayerController>().stats.killRangeMult += player.GetComponent<PlayerController>().stats.killRangeMultDelta;
                            break;
                        case KillReward.DRAGSPEED: player.GetComponent<PlayerController>().stats.dragSpeedMult += player.GetComponent<PlayerController>().stats.dragSpeedMultDelta;
                            break;
                        case KillReward.RUNSPEED: player.GetComponent<PlayerController>().stats.runSpeedMult += player.GetComponent<PlayerController>().stats.runSpeedMultDelta;
                            break;
                        case KillReward.ENDURANCE: player.GetComponent<PlayerController>().stats.enduranceMult += player.GetComponent<PlayerController>().stats.enduranceMultDelta;
                            break;
                    }

                    GetComponent<SpriteRenderer>().color = Color.black;

                    // Remove light
                    Destroy(transform.GetChild(0).gameObject);

                    // Remove collider
                    GetComponent<BoxCollider2D>().isTrigger = true;

                    StopAllCoroutines();
                    return;
                }
            }

            if(monsterState == StateMachine.WANDER) {

                // Follow path
                if(pointer >= path.Length){ pointer = 0; }

                Vector3 targetDirection = 
                    ((pointer == 0 || pointer == (path.Length - 1)) ? startPosition : 
                    path[pointer - 1]) + path[pointer];

                // Move towards point
                float step = enemyStats.wanderSpeed * Time.deltaTime;

                // Set monster direction
                direction = -(transform.position - targetDirection).normalized;

                Vector3 tween = Vector3.Lerp(transform.position, targetDirection, Time.deltaTime * enemyStats.tweenRate);
                transform.position = Vector3.MoveTowards(transform.position, tween, step);

                // Check proximity to next point
                if(Vector3.Distance(transform.position, targetDirection) < nextPointSnap) {

                    if (enemyStats.hasRestTime){

                        // Prepare rest
                        monsterState = StateMachine.STOP;
                        Invoke(nameof(EndRest), enemyStats.restTime);
                    }

                    pointer++; 
                }
            }

            if(monsterState == StateMachine.CHASE) {

                // Move towards player
                float step = enemyStats.chaseSpeed * Time.deltaTime;

                Vector3 tween = Vector3.Lerp(transform.position, player.transform.position, Time.deltaTime * enemyStats.tweenRate);
                transform.position = Vector3.MoveTowards(transform.position, tween, step);
            }

            handlePlayerDetection();

        } else {

            // Check for dragging
            if(distanceFromPlayer < enemyStats.killRadius){
              
              bool dragKeys = Utils.GetKeyAll(player.GetComponent<PlayerController>().stats.actionKeys);
              
              monsterState = dragKeys ? StateMachine.DRAG : StateMachine.DEAD;  
              player.GetComponent<PlayerController>().isDragging = dragKeys;
            } 

            if (monsterState == StateMachine.DRAG) {

                dragOffset.x = player.GetComponentInChildren<SpriteRenderer>().transform.localScale.x == 1 ? -0.3f : 0.3f;

                // Apply drag force
                transform.position = player.transform.position + dragOffset;
            }
        }
    }

    void handlePlayerDetection(){

        // Check if in vision cone
        Collider2D[] targetsInRadius = Physics2D.OverlapCircleAll(transform.position, enemyStats.checkDistance, enemyStats.visionMask);

        for(int i = 0; i < targetsInRadius.Length; i++){
            
            Transform target = targetsInRadius[i].transform;

            // Check if in view angle
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if(Vector3.Angle(direction, dirToTarget) < (enemyStats.checkAngle / 2)){

                //Check for walls in the way
                RaycastHit2D ray = Physics2D.Raycast(transform.position, dirToTarget, float.MaxValue, enemyStats.collisionLayer);

                if (!ray){

                    // Is within angle
                    effects.SetBool("twitch", true);
                    monsterState = StateMachine.SHOCK;

                    Invoke(nameof(StopTwitch), 1);    
                } else {

                    monsterState = StateMachine.WANDER;
                }
                
            }
        }
        
    }

    public Vector3 DirFromAngle(float angle, bool global){

        if(!global){ angle += transform.eulerAngles.y; }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    void EndRest(){ 

        if(monsterState == StateMachine.DEAD || monsterState == StateMachine.DRAG) return;
        monsterState = StateMachine.WANDER; 
    }

    void StopTwitch(){ 

        monsterState = StateMachine.CHASE;
        effects.SetBool("twitch", false); 
    }

    void playerDie(){

        monsterState = StateMachine.WANDER;
        transform.position = spawn;
        player.GetComponent<PlayerController>().Die();
    }

    void OnCollisionEnter2D(Collision2D collision){

        if(collision.gameObject.tag == "Player" && 
            (monsterState != StateMachine.WANDER && monsterState != StateMachine.STOP) && monsterState != StateMachine.DEAD && monsterState != StateMachine.DRAG){
            StartCoroutine(Transition.getInstance().DoTransition(playerDie));
        }
    }
}
