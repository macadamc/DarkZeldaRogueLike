using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Destructable : ZOrderObject {

    public float health = 0f;
    public Stats stats;
    public StatModifiers statMods;

    public bool skipDamageFlash;
    bool flashing;

    public AudioClip hurtSfx;
    AudioSource audioSource;

    delegate void OnObjDeath(string name);
    OnObjDeath onObjDeath;

    public System.Random random = new System.Random();

    public void ModifyHealth(float changeInHealth)
    {
        if (flashing)
            return;

        health += changeInHealth;

        if (changeInHealth < 0)
        {
            PlayHurtSFX();
            StartCoroutine(DamageFlash(2));
        }

    }

    public void CheckForDeath()
    {
        if (health <= 0)
        {
            if (onObjDeath != null)
                onObjDeath(stats.objName);

            DestroyObject();
        }
    }

    public void DestroyObject()
    {
        Debug.Log("obj destroyed");
        Spawn(stats.spawnOnDeath);
        gameObject.SetActive(false);
        //Destroy(gameObject);
    }

    public override void Awake()
    {
        base.Awake();
        health = stats.health;
        Spawn(stats.spawnOnCreate);
        audioSource = GetComponent<AudioSource>();

        statMods = new StatModifiers();

        onObjDeath += AchievementManager.am.OnObjDeath;
    }
    void OnDisable()
    {
        onObjDeath -= AchievementManager.am.OnObjDeath;
    }

    void OnDestroy()
    {
        onObjDeath -= AchievementManager.am.OnObjDeath;
    }

    public void PlayHurtSFX()
    {
        audioSource.Stop();
        audioSource.clip = hurtSfx;
        audioSource.Play();
    }

    public void PlaySfx(AudioClip sfx)
    {
        audioSource.Stop();
        audioSource.clip = sfx;
        audioSource.Play();
    }

    public void Spawn(SpawnObject[] spawnObjects)
    {
        for (int i = 0; i < spawnObjects.Length; i++)
        {
            Vector2 randomVector = Random.insideUnitCircle * spawnObjects[i].positionRandomness;
            Vector3 spawnPos = (Vector3)(randomVector + spawnObjects[i].positionOffset) + transform.position;

            GameObject obj = Instantiate(spawnObjects[i].objectToSpawn, spawnPos, transform.rotation) as GameObject;
            obj.transform.parent = GameManager.GM.InGameObjectManager.GetContainer("ParticleEffects").transform;
        }
    }

    public IEnumerator DamageFlash(int flashes)
    {
        if(!skipDamageFlash)
        {
            flashing = true;
            for (int i = 0; i < flashes; i++)
            {
                rend.enabled = true;
                yield return new WaitForSeconds(0.1f);
                rend.enabled = false;
                yield return new WaitForSeconds(0.1f);
            }
        }
        flashing = false;
        rend.enabled = true;
        CheckForDeath();
        yield return null;
    }
}
