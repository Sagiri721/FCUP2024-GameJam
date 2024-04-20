using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public Stats stats;
    public float stridePointer = 0f, strideHeightPercentage = 0.15f, strideDelta = 0.09f;
    public static float currentHunger;
    public static bool playerExists, isPaused = false, isStopped = false, isCarrying = false;
    public int direction;
    public Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    public Sprite[] idleSprites;
    public Sprite[] walkFrontSprites;
    public Sprite[] walkBackSprites;
    
    public Sprite[] dragBackSprites;
    public Sprite[] dragFrontprites;

    private bool facingFront = true, walking = false;

    public Vector3 movement;

    public GameObject biteEffect;

    public Vector3 spawn;

    public bool isDragging = false;

    public GameObject[] multipliers;
    public bool[] unlocked;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = rb.GetComponentInChildren<SpriteRenderer>();
        DontDestroyOnLoad(gameObject);
        spawn = transform.position;

        currentHunger = 100f;

        unlocked = new bool[multipliers.Length];

        int i = 0;
        foreach(GameObject g in multipliers) {
            g.SetActive(false);
            unlocked[i] = false;
            i++;
        }
    }

    public void SetPowerUp(int index){
        multipliers[index].SetActive(true);
        unlocked[index] = true;
        currentHunger = stats.maxHunger;
    }

    // Update is called once per frame
    void Update()
    {

        //if (Input.GetKeyDown(KeyCode.Escape))
        //{
        //    isPaused = true;
        //    pauseScreen.SetActive(true);
        //    Time.timeScale = 0f;
        //}
        if (isPaused) { return; }
        currentHunger += stats.hungerDelta * Time.deltaTime;
        if(currentHunger <= 0){
            StopCoroutine(walkCycle());
            //Death sequence
            return;
        }
        if(Input.anyKeyDown){
            if(Input.GetKeyDown(KeyCode.RightArrow)){
                direction = 0;
                playerSprite.transform.localScale = new Vector3(1, 1, 1);
            }
            if(Input.GetKeyDown(KeyCode.UpArrow)){
                direction = 1;
                facingFront = false;
            }
            if(Input.GetKeyDown(KeyCode.LeftArrow)){
                direction = 2;
                playerSprite.transform.localScale = new Vector3(-1, 1, 1);
            }
            if(Input.GetKeyDown(KeyCode.DownArrow)){
                direction = 3;
                facingFront = true;
            }
        }

        if(unlocked[1]){
            multipliers[1].SetActive(
                playerSprite.sprite == dragFrontprites[1] || playerSprite.sprite == dragFrontprites[0] ||
                playerSprite.sprite == walkFrontSprites[1] || playerSprite.sprite == walkFrontSprites[0] ||
                playerSprite.sprite == idleSprites[0]
            );
        }

        float speed = (Utils.GetKeyAll(stats.runKeys) ? stats.runSpeed * stats.runSpeedMult : stats.walkSpeed);
        if (isDragging) speed = stats.dragSpeed * stats.dragSpeedMult;

        Vector2 velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * speed;
        rb.velocity = velocity;

        if(rb.velocity != Vector2.zero && !walking) { 
            
            StartCoroutine(walkCycle()); 
        }
        movement = rb.velocity;
    }

    public void Die(){

        SceneManager.LoadScene("You died");
    }
    
    public IEnumerator walkCycle(){
        walking = true;
        while(walking){
            float previousHeight = playerSprite.transform.localPosition.y;
            playerSprite.transform.localPosition = new Vector2(0, Mathf.Abs(Mathf.Sin(stridePointer) * strideHeightPercentage));
            if(playerSprite.transform.localPosition.y < previousHeight && playerSprite.transform.localPosition.y < Mathf.Abs(Mathf.Sin(stridePointer + strideDelta) * strideHeightPercentage)){
                if(rb.velocity == Vector2.zero){
                    if(facingFront){
                        playerSprite.sprite = idleSprites[0];
                    }else{
                        playerSprite.sprite = idleSprites[1];
                    }
                    stridePointer = 0;
                    walking = false;
                    yield break;
                }

                if(isDragging){

                    if(facingFront){
                        if(playerSprite.sprite == dragFrontprites[1]){
                            playerSprite.sprite = dragFrontprites[0];
                        }else{
                            playerSprite.sprite = dragFrontprites[1];
                        }
                    }else{
                        if(playerSprite.sprite == dragFrontprites[1]){
                            playerSprite.sprite = dragFrontprites[0];
                        }else{
                            playerSprite.sprite = dragFrontprites[1];
                        }
                    }

                }else{

                    if(facingFront){
                        if(playerSprite.sprite == walkFrontSprites[1]){
                            playerSprite.sprite = walkFrontSprites[0];
                        }else{
                            playerSprite.sprite = walkFrontSprites[1];
                        }
                    }else{
                        if(playerSprite.sprite == walkBackSprites[1]){
                            playerSprite.sprite = walkBackSprites[0];
                        }else{
                            playerSprite.sprite = walkBackSprites[1];
                        }
                    }
                }

            }
            stridePointer += strideDelta;
            yield return null;
        }
    }
}
