using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering.Universal;


public class PlayerController : MonoBehaviour
{
    public Stats stats;
    public float stridePointer = 0f, strideHeightPercentage = 0.15f, strideDelta = 0.09f;
    public static bool playerExists, isPaused = false, isStopped = false, isCarrying = false;
    public int direction;
    public Animator animator;
    private Rigidbody2D rb;
    private SpriteRenderer playerSprite;
    public Sprite[] idleSprites;
    public Sprite[] walkFrontSprites;
    public Sprite[] walkBackSprites;
    private bool facingFront = true, walking = false;

    public GameObject biteEffect;

    public Vector3 spawn;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerSprite = rb.GetComponentInChildren<SpriteRenderer>();
        DontDestroyOnLoad(gameObject);
        spawn = transform.position;
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
        if(Utils.GetKeyAll(stats.runKeys)){
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * stats.runSpeed * stats.runSpeedMult;
        }else{
            rb.velocity = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized * stats.walkSpeed;
        }
        if(rb.velocity != Vector2.zero && !walking) { StartCoroutine(walkCycle()); }
        //animator.SetInteger("direction", direction);
    }

    public void Die(){

        transform.position = spawn;
    }
    
    public IEnumerator walkCycle(){
        walking = true;
        while(walking){
            float previousHeight = playerSprite.transform.localPosition.y;
            playerSprite.transform.localPosition = new Vector2(0, Mathf.Abs(Mathf.Sin(stridePointer) * strideHeightPercentage));
            if(playerSprite.transform.localPosition.y < previousHeight && playerSprite.transform.localPosition.y < Mathf.Abs(Mathf.Sin(stridePointer + strideDelta) * strideHeightPercentage)){
                if(rb.velocity == Vector2.zero){
                    /*if(playerSprite.transform.localScale.x == 1){
                        playerSprite.sprite = idleSprites[0];
                    }else{
                        playerSprite.sprite = idleSprites[1];
                    }*/
                    playerSprite.sprite = idleSprites[0];
                    stridePointer = 0;
                    walking = false;
                    yield break;
                }
                if(playerSprite.sprite == walkFrontSprites[1]){
                    playerSprite.sprite = walkFrontSprites[0];
                }else{
                    playerSprite.sprite = walkFrontSprites[1];
                }
            }
            stridePointer += strideDelta;
            yield return null;
        }
    }

    #region
    //Inventory System
    //Inventory system. Can you honestly believe I made that in 11th grade?
    #region
    /*if (itemExists)
    {
        //Item use when pressing Q
        if (Input.GetKeyDown(KeyCode.Q))
        {
            switch (itemInventory[0, arrayCounter])
            {
                case 1: hp.Heal(10);
                    break;
                case 2: StartCoroutine(SEHeal(30, 5));
                    break;
                case 3: dmgMult += 1;
                    StartCoroutine(EndDamageBoost(10));
                    break;
                case 4: speedMult += 1;
                    StartCoroutine(EndSpeedBoost(10));
                    break;
                case 5: immunityTimer += 10f;
                    break;
                case 6: 
                    if (mp.Mana< 75) { break; }
                    StartCoroutine(KillerQueen());
                    break;
                case 7:
                    if (zaWarudo || mp.Mana < 75) { break; }
                    mp.Use(75);
                    StartCoroutine(TimeStop());
                    break;
                case 8: 
                    if (mp.Mana < 75) { break; }
                    StartCoroutine(TuskActIV());
                    break;
            }
            RemoveFromInventory(arrayCounter);
            return;
        }

        //Scroll the item selection wheel up
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (arrayCounter == itemInventory.GetLength(1) - 1)
                arrayCounter = 0;
            else
            {
                arrayCounter++;
            }
        playerhud.GetComponent<HudController>().UpdateShownItem();
        return;
        }

        //Scroll the item selection wheel down
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (arrayCounter == 0)
            {
                arrayCounter = itemInventory.GetLength(1) - 1;
                if (arrayCounter < 0)
                {
                    arrayCounter = 0;
                }
            }
            else
            {
                arrayCounter--;
            }
            playerhud.GetComponent<HudController>().UpdateShownItem();
            return;
        }
    }*/
    #endregion

    /*public void AddToInventory(Items items, int quantity)
    {
        itemExists = true;

        //Check if already on inventory
        for (int i = 0; i < itemInventory.GetLength(1); i++)
        {
            if (itemInventory[0, i] == items.id)
            {
                itemInventory[1, i] += quantity;
                playerhud.GetComponent<HudController>().UpdateShownItem(); //For the occasion in which the obtained item is being shown
                return;
            }
        }

        //If not resize the array to include the new id and the assigned quantity
        itemInventory = ResizeArray(itemInventory, itemInventory.GetLength(1) + 1);
        itemInventory[0, itemInventory.GetLength(1) - 1] = items.id;
        itemInventory[1, itemInventory.GetLength(1) - 1] = items.quantity;
        playerhud.GetComponent<HudController>().UpdateShownItem();
    }

    public void RemoveFromInventory(int n)
    {
        if (Array.IndexOf(playerhud.GetComponent<HudController>().specialItems, itemInventory[0, n]) != -1)
        {
            return;
        }
        itemInventory[1, n]--;
        if (itemInventory[1, n] == 0)
        {
            if (arrayCounter == itemInventory.GetLength(1) - 1)
            {
                itemInventory = ResizeArray(itemInventory, itemInventory.GetLength(1) - 1);
                arrayCounter--;
            }
            else
            {
                for (int i = arrayCounter; i < itemInventory.GetLength(1) - 1; i++)
                {
                    itemInventory[0, i] = itemInventory[0, i + 1];
                    itemInventory[1, i] = itemInventory[1, i + 1];
                }
                itemInventory = ResizeArray(itemInventory, itemInventory.GetLength(1) - 1);
            }
            if (itemInventory.GetLength(1) == 0)
            {
                itemExists = false;
                arrayCounter = 0;
            }
        }
        playerhud.GetComponent<HudController>().UpdateShownItem();
    }

    //Resize the inventory array. A new row number isn't necessary since there will always be 2 rows.
    private int[,] ResizeArray(int[,] original, int newColumnNumber)
    {
        int[,] newArray = new int[2, newColumnNumber];
        if (newColumnNumber < original.GetLength(1))
        {
            for (int i = 0; i < 2; i++)
            {
                for (int i1 = 0; i1 < newColumnNumber; i1++)
                {
                    newArray[i, i1] = original[i, i1];
                }
            }
        }
        else
        {
            for (int i = 0; i < 2; i++)
            {
                for (int i1 = 0; i1 < original.GetLength(1); i1++)
                {
                    newArray[i, i1] = original[i, i1];
                }
            }
        }
        return newArray;
    }

    public static int GetCurrentItemId()
    {
        try
        {
            return itemInventory[0, arrayCounter];
        }
        catch (System.Exception)
        {
            return 0;
        }
    }

    public static int GetCurrentAbilityId()
    {
        try
        {
            return abilities[abilityCounter];
        }
        catch (System.Exception)
        {
            return 0;
        }
    }*/
    #endregion
}
