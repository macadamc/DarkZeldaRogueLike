using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : BaseInteractable
{

    // we have use a new rng for each chest because they can go to them in any order.
    // thus making the content in the chest diffrent for diffrent players.
    // using a seed and a new rng generated when we create the lvl we can sidestep this problem.
    public int seed;
    DefaultRNG rng;

    public Sprite openSprite;
    public Sprite closedSprite;
    SpriteRenderer rend;

    public static ItemsMetaData allItems;

    public bool trapped;
    public bool forceItemSpawn;

    [Range(0f,1f)]
    public float itemChance = 0.15f;

    public AudioClip chestSpawnSFX;

    // used to toggle the sprite of the chest to open/closed.
    bool isOpen;
    // set to true the first time this chest is opened.
    bool triggered;
    // this is used to stop the opening/closing of the chest while its creating the items.
    bool running;


    public override void Awake()
    {
        base.Awake();
        isOpen = false;
        triggered = false;
        running = false;

        rend = gameObject.transform.parent.gameObject.GetComponent<SpriteRenderer>();
        rend.sprite = closedSprite;
        allItems = GameObject.Find("Manager").GetComponent<GameManager>().itemsMetaData;
    }

    public override void Interact(Entity other)
    {
        if (running == false)
        {
            ToggleOpenState();
        }

        // spawn contents of the chest.
        if (triggered == false)
        {
            StartCoroutine("CreateContents");
            triggered = true;
            PlaySFX(interactSFX);

            //other.targetInteractable = null;
            base.OnTriggerExit2D(other.GetComponent<Collider2D>());
            //trigger.enabled = false;
        }

    }

    void ToggleOpenState()
    {
        isOpen = !isOpen;

        // play chest opened/closed animation.
        if (isOpen)
        {
            rend.sprite = openSprite;
            //Debug.Log("Chest Opened!!");
        }
        else
        {
            rend.sprite = closedSprite;
            //Debug.Log("Chest Closed!!");
        }
    }

    IEnumerator CreateContents()
    {
        // THE IDEA

        // get a list of items that can spawn in this lvl.
        // randomly create the quality of this chest. used to generate better items.

        // generate number of coins to spawn;
        // generate Each coins amount based on lvl and quality. (set animation to diffrent coins based on amount)
        // generate any pickups using the quality and list of items that can be spwaned in this lvl. (still unsure how i should handle this. and weapons with "effects" in general..)

        // give all the objects we spawned some sort of distance var and some sort of deceleration value.
        // maybe make a reusable monobehavionr for this..
        // maybe add some sort of shadow system to make it look more 3d.

        running = true;

        rng = new DefaultRNG(seed);

        float chestQuality = rng.NextFloat();

        bool itemGenerated = false;

        yield return new WaitForSeconds(.08f);

        if (chestQuality <= itemChance || forceItemSpawn)
        {
            Object prefab;
            if (rng.NextDouble() <= .5f)
            {
                prefab = allItems.activeItems[rng.Next(0, allItems.activeItems.Count)].onGroundPrefab;
            }
            else
            {
                prefab = allItems.passiveItems[rng.Next(0, allItems.passiveItems.Count)].onGroundPrefab;
            }
             

            if (!source.isPlaying)
                PlaySFX(chestSpawnSFX);

            InGameObjectManager.CreatePickup(prefab, transform.position, 30, 15);

            itemGenerated = true;
        }

        float maxCoins = 25;

        if (itemGenerated)
        {
            maxCoins = maxCoins / 2;
            chestQuality = chestQuality / 2f;
        }

        int numberOfCoins = Mathf.CeilToInt(maxCoins * chestQuality);

        // Coin Explosion!! Just a test..
        // need to make it so they cant get picked up until they stop moving..
        // the player gets shotgun blased with coins....
        for (int i = 0; i < numberOfCoins;)
        {
            if (!source.isPlaying)
                PlaySFX(chestSpawnSFX);

            GameObject coin = InGameObjectManager.CreatePickup("Coin", transform.position, 30, 15);

            int amount;

            if (i + 5 < numberOfCoins && rng.NextDouble() <= .25f)
            {
                amount = 5;
            }
            else
            {
                amount = 1;
            }
            coin.GetComponent<Coin>().amount = amount;
            i += amount;
            yield return new WaitForSeconds(.08f);
        }

        running = false;

    }

}