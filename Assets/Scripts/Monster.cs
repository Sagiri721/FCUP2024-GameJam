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

    private Vector2 startPosition;
    private Vector3 direction;
    private int pointer = 0;

    Animator effects;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        startPosition = transform.position;

        effects = GetComponent<Animator>();
    }

    void Update()
    {
        float distanceFromPlayer = (player.position - transform.position).magnitude;

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

        if(monsterState != StateMachine.CHASE) handlePlayerDetection();
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
                RaycastHit2D ray = Physics2D.Raycast(transform.position, dirToTarget, enemyStats.checkDistance, enemyStats.collisionLayer);
                if (!ray){

                    // Is within angle
                    effects.SetBool("twitch", true);
                    monsterState = StateMachine.SHOCK;

                    Invoke(nameof(StopTwitch), 1);    
                }
                
            }
        }
        
    }

    public Vector3 DirFromAngle(float angle, bool global){

        if(!global){ angle += transform.eulerAngles.y; }
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    void EndRest(){ monsterState = StateMachine.WANDER; }
    void StopTwitch(){ 

        monsterState = StateMachine.CHASE;
        effects.SetBool("twitch", false); 
    }

    void die(){

        monsterState = StateMachine.WANDER;
        player.GetComponent<PlayerController>().Die();
    }

    void OnTriggerEnter2D(Collider2D collider){

        if(collider.gameObject.tag == "Player" && 
            (monsterState != StateMachine.WANDER && monsterState != StateMachine.STOP)){
            StartCoroutine(Transition.getInstance().DoTransition(die));
        }

        if (Input.GetKeyDown(KeyCode.X)){
            
        }
    }
}
