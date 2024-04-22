using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class DeadAdventurer : MonoBehaviour
{
    public EnemyStats enemyStats;
    private Transform player;
    private float distanceFromPlayer, eatTime = 5f;
    private bool isEaten = false;
    
    public SpriteRenderer interactArrow, xKey;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        GetComponent<Light2D>().pointLightInnerRadius = enemyStats.checkDistance / 2;
        GetComponent<Light2D>().pointLightOuterRadius = enemyStats.checkDistance / 2 + 0.7f;
        GetComponent<Light2D>().intensity = 0.2f;
    }

    // Update is called once per frame
    void Update()
    {
        eatTime -= Time.deltaTime;
        float distanceFromPlayer = (player.position - transform.position).magnitude;
        if(distanceFromPlayer < enemyStats.killRadius && eatTime <= 0f && !isEaten){
            interactArrow.enabled = true;
            xKey.enabled = true;
            if(Utils.GetKeyDownAll(player.GetComponent<PlayerController>().stats.actionKeys)){
                interactArrow.enabled = false;
                xKey.enabled = false;
                StartCoroutine(feedProcess());
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
        PlayerController.currentHunger += player.GetComponent<PlayerController>().stats.maxHunger * player.GetComponent<PlayerController>().stats.hungerRestorePercent;
        Destroy(gameObject);
    }
}
