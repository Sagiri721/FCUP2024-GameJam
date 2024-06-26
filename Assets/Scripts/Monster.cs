using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;

public class Monster : MonoBehaviour
{
    private Transform player;
    public StateMachine monsterState = StateMachine.WANDER;
    private Light2D monsterLight;

    public Sprite[] images, imded;
    public int index = 0;

    public enum StateMachine {
        WANDER,
        CHASE,
        STOP,
        DEAD,
        DRAG,
        SHOCK,
        ROT
    }

    public EnemyStats enemyStats;
    public Vector2[] path;
    public float nextPointSnap = 0.01f;

    public enum KillReward {
        KILLRANGE = 0,
        DRAGSPEED = 1,
        RUNSPEED = 2,
        ENDURANCE
    }
    
    public KillReward killReward;
    public float eatTime = 5f;

    private Vector2 startPosition;
    private Vector3 direction, spawn;
    private int pointer = 0;

    private Animator effects;
    private Transform reviveTarget;

    public Vector3 dragOffset;
    private ParticleSystem particles;
    public SpriteRenderer shockSign, interactArrow, zKey, xKey;

    private bool isRotten = false, isEaten = false, vulnerable = false;

    private bool floating = false;
    public float stridePointer = 0f, strideHeightPercentage = 0.15f, strideDelta = 0.09f;

    public static bool targetInSight = false;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        startPosition = transform.position;

        effects = GetComponent<Animator>();
        spawn = transform.position;

        particles = GetComponentInChildren<ParticleSystem>();
        GetComponentsInChildren<Light2D>()[0].pointLightInnerAngle = enemyStats.checkAngle;
        GetComponentsInChildren<Light2D>()[0].pointLightOuterAngle = enemyStats.checkAngle + 20;
        GetComponentsInChildren<Light2D>()[0].pointLightInnerRadius = enemyStats.checkDistance;
        GetComponentsInChildren<Light2D>()[0].pointLightOuterRadius = enemyStats.checkDistance + 1.7f;
        GetComponentsInChildren<Light2D>()[1].pointLightInnerRadius = enemyStats.checkDistance;
        GetComponentsInChildren<Light2D>()[1].pointLightOuterRadius = enemyStats.checkDistance + 0.7f;

        index = 0;
        if (killReward == KillReward.KILLRANGE) index = 0;
        if (killReward == KillReward.DRAGSPEED) index = 1;
        if (killReward == KillReward.RUNSPEED) index = 2;

        GetComponent<SpriteRenderer>().sprite = images[index];

        if(index == 0 || index == 2) {
            floating = true;
        }


    }

    void Update()
    {
        if(monsterState == StateMachine.CHASE){ transform.GetChild(0).up = (player.position - transform.position).normalized; }
        else transform.GetChild(0).up = direction;

        float distanceFromPlayer = (player.position - transform.position).magnitude;

        if(monsterState != StateMachine.DEAD && monsterState != StateMachine.DRAG) {

            if(distanceFromPlayer < enemyStats.killRadius * player.GetComponent<PlayerController>().stats.killRangeMult && !CooldownHandler.onCooldown &&
                            (monsterState == StateMachine.WANDER || monsterState == StateMachine.STOP) && (!targetInSight || (targetInSight && vulnerable))){
                interactArrow.enabled = true;
                xKey.enabled = true;
                targetInSight = true;
                vulnerable = true;
                if (Utils.GetKeyDownAll(player.gameObject.GetComponent<PlayerController>().stats.actionKeys)){
                    interactArrow.enabled = false;
                    xKey.enabled = false;
                    targetInSight = false;
                    vulnerable = false;
                    GameObject effect = player.GetComponent<PlayerController>().biteEffect;
                    Instantiate(effect, transform.position, Quaternion.identity);
                    GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("Bite");
                    monsterState = StateMachine.DEAD;
                    GetComponentsInChildren<Light2D>()[0].enabled = false;
                    CooldownHandler.killTrigger = true;

                    // DON'T Remove light, just lower intensity and radius
                    GetComponentsInChildren<Light2D>()[1].pointLightInnerRadius = enemyStats.checkDistance / 2;
                    GetComponentsInChildren<Light2D>()[1].pointLightOuterRadius = enemyStats.checkDistance / 2 + 0.7f;
                    GetComponentsInChildren<Light2D>()[1].intensity = 0.2f;

                    // Remove collider
                    GetComponent<BoxCollider2D>().isTrigger = true;

                    //StopAllCoroutines();
                    GetComponent<SpriteRenderer>().sprite = imded[index];

                    // Start rotting process
                    Invoke(nameof(Rot), enemyStats.rotTime - 3);
                    return;
                }
            }else{
                interactArrow.enabled = false;
                xKey.enabled = false;
                if(vulnerable){
                    targetInSight = false;
                    vulnerable = false;
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

            if (floating) {}

            handlePlayerDetection();

        } else {

            eatTime -= Time.deltaTime;
            if(distanceFromPlayer < enemyStats.killRadius && !isRotten && !isEaten){

                if(eatTime <= 0f && monsterState == StateMachine.DEAD){

                    zKey.enabled = false;
                    xKey.enabled = true;
                    interactArrow.enabled = true;
                    if(Utils.GetKeyDownAll(player.GetComponent<PlayerController>().stats.actionKeys)){
                        xKey.enabled = false;
                        interactArrow.enabled = false;
                        StartCoroutine(feedProcess());
                    }
                }else{
                    zKey.enabled = true;
                    interactArrow.enabled = true;
                }
                
              
                bool dragKeys = Utils.GetKeyAll(player.GetComponent<PlayerController>().stats.runKeys);
              
                monsterState = dragKeys ? StateMachine.DRAG : StateMachine.DEAD;  
                player.GetComponent<PlayerController>().isDragging = dragKeys;

            }else{
                zKey.enabled = false;
                xKey.enabled = false;
                interactArrow.enabled = false;
            }

            if (monsterState == StateMachine.DRAG) {

                dragOffset.x = player.GetComponentInChildren<SpriteRenderer>().transform.localScale.x == 1 ? -0.3f : 0.3f;

                // Apply drag force
                transform.position = player.transform.position + dragOffset;
            }
        }
    }

    public IEnumerator feedProcess(){
        isEaten = true;
        int counter = 0;
        GameObject effect = player.GetComponent<PlayerController>().biteEffect;
        while(counter < 5){
            GameObject a = Instantiate(effect, transform.position, Quaternion.identity);
            GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("Bite");
            while(a != null) { yield return null; }
            counter++;
        }
        switch (killReward){
            case KillReward.KILLRANGE: player.GetComponent<PlayerController>().stats.killRangeMult += player.GetComponent<PlayerController>().stats.killRangeMultDelta;
                player.GetComponent<PlayerController>().SetPowerUp(0);
                CooldownHandler.killType = 1;
                break;
            case KillReward.DRAGSPEED: player.GetComponent<PlayerController>().stats.dragSpeedMult += player.GetComponent<PlayerController>().stats.dragSpeedMultDelta;
                player.GetComponent<PlayerController>().SetPowerUp(1);
                CooldownHandler.killType = 0;
                break;
            case KillReward.RUNSPEED: player.GetComponent<PlayerController>().stats.runSpeedMult += player.GetComponent<PlayerController>().stats.runSpeedMultDelta;
                player.GetComponent<PlayerController>().SetPowerUp(2);
                CooldownHandler.killType = 2;
                break;
            case KillReward.ENDURANCE: player.GetComponent<PlayerController>().stats.enduranceMult += player.GetComponent<PlayerController>().stats.enduranceMultDelta;
                break;
        }
        PlayerController.currentHunger += player.GetComponent<PlayerController>().stats.maxHunger * player.GetComponent<PlayerController>().stats.hungerRestorePercent;
        if(PlayerController.currentHunger > 100f) { PlayerController.currentHunger = 100f; }
        Destroy(gameObject);
    }

    void handlePlayerDetection(){

        // Check if in vision cone
        Collider2D[] targetsInRadius = Physics2D.OverlapCircleAll(transform.position, enemyStats.checkDistance, enemyStats.visionMask);

        for(int i = 0; i < targetsInRadius.Length; i++){

            Transform target = targetsInRadius[i].transform;


            // Check if in view angle
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            //Debug.Log(Vector3.Angle(direction, dirToTarget));
            if(Vector3.Angle(direction, dirToTarget) < (enemyStats.checkAngle / 2)){

                //Check for walls in the way
                RaycastHit2D ray = Physics2D.Raycast(transform.position, dirToTarget, dirToTarget.magnitude, enemyStats.collisionLayer);

                if (!ray && monsterState != StateMachine.CHASE){

                    if(monsterState != StateMachine.SHOCK){
                        FindObjectOfType<AudioManager>().Play("Spot");
                    }
                    // Is within angle
                    shockSign.enabled = true;
                    interactArrow.enabled = false;
                    zKey.enabled = false;
                    xKey.enabled = false;
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

        shockSign.enabled = false;
        monsterState = StateMachine.CHASE;
        effects.SetBool("twitch", false); 
    }

    void Rot(){
        if(isEaten){return;}

        // can't drag anymore
        player.GetComponent<PlayerController>().isDragging = false;

        isRotten = true;
        monsterState = StateMachine.ROT;

        // Is within angle
        effects.SetBool("twitch", true);
        Invoke(nameof(StopTwitch), 3);
        particles.Play();
        Invoke(nameof(RottenDestroy), 3);
        effects.SetBool("die", true);
    }

    void RottenDestroy(){

        Destroy(gameObject);
    }

    void playerDie(){

        monsterState = StateMachine.WANDER;
        transform.position = spawn;
        player.GetComponent<PlayerController>().Die();
    }

    void OnCollisionEnter2D(Collision2D collision){
        if(collision.gameObject.tag == "Player" && 
            (monsterState != StateMachine.WANDER && monsterState != StateMachine.STOP) && monsterState != StateMachine.DEAD && monsterState != StateMachine.DRAG){
            Transition.getInstance().fadeImage = GameObject.Find("FadeImage").GetComponent<Image>();
            GameObject.Find("AudioManager").GetComponent<AudioManager>().Play("Death");
            FindObjectOfType<AudioManager>().StopBackground();
            StartCoroutine(Transition.getInstance().DoTransition(playerDie));
        }
    }
}
