using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;

[RequireComponent(typeof(Entity))]
public class PlayerController : MonoBehaviour
{

    Entity entity;
    public bool strafe;
    bool attackBool;
    bool isInteracting;
    public float deadZone = 0.2f;
    public bool dead;

    int _ActiveItemSlot;

    void Start()
    {
        entity = GetComponent<Entity>();
    }

	// Update is called once per frame
	void Update ()
    {
        if (PauseManager.gamePaused)
            return;

        CheckForInteractable();

        SetMoveVector();

        SetLookDir();

        CheckForAttacks();


        if (entity.health <= 0 && !dead)
        {
            dead = true;
            Invoke("PlayerDead", 2f);
        }
    }

    void PlayerDead()
    {
        GameManager.GM.PlayerDeath();
    }

    void SetMoveVector()
    {
        Vector2 input = new Vector2(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"));

        if (input.magnitude < deadZone) { input = Vector2.zero; }

        if (entity.MaxSpeed > 0)
        {
            entity.moveVector = input * entity.MaxSpeed;
        }
        
    }

    void SetLookDir()
    {
        if (strafe == true || entity.stunLocked)
        {
            return;
        }

        Vector2 input = new Vector2(CnInputManager.GetAxis("Horizontal"), CnInputManager.GetAxis("Vertical"));

        if (input.magnitude < deadZone) { input = Vector2.zero; }


        if (input.magnitude > 0)
        {
            Vector2 lookInput = input;
            entity.lookDir = lookInput.normalized;
        }
    }

    void CheckForInteractable()
    {
        if(entity.targetInteractable != null)
        {
            bool fire2Down = CnInputManager.GetButtonDown("Fire2");
            bool fire2Up = CnInputManager.GetButtonUp("Fire2");

            bool fire3Down = CnInputManager.GetButtonDown("Fire3");
            bool fire3Up = CnInputManager.GetButtonUp("Fire3");

            if (fire2Down || fire3Down)
            {
                isInteracting = true;

                if(fire2Down)
                {
                    _ActiveItemSlot = 0;
                }
                else if (fire3Down)
                {
                    _ActiveItemSlot = 1;
                }
            }

            if ((fire2Up || fire3Up) && isInteracting)
            {
                isInteracting = false;

                if (entity.targetInteractable is ItemPickup)
                {
                    ((ItemPickup)entity.targetInteractable).activeItemSlot = _ActiveItemSlot;
                }

                entity.Interact();
            }
            
        }
    }

    void CheckForAttacks()
    {
        if (entity.attackDelay > 0 || isInteracting)
            return;

        bool onDown = CnInputManager.GetButtonDown("Fire2");
        bool onHeld = CnInputManager.GetButton("Fire2");
        bool onUp = CnInputManager.GetButtonUp("Fire2");
        ActiveItem weapon = entity.weapons[0];
        if (onDown && attackBool == false && weapon != null)
        {
            weapon.OnAttackTriggered(entity);
            attackBool = true;
        }
        if (onDown == false && onHeld && attackBool && weapon != null)
        {
            weapon.OnAttackHeld(entity);
        }
        if (onUp && attackBool && weapon != null)
        {
            weapon.OnAttackEnd(entity);
            attackBool = false;
        }

        onDown = CnInputManager.GetButtonDown("Fire3");
        onHeld = CnInputManager.GetButton("Fire3");
        onUp = CnInputManager.GetButtonUp("Fire3");
        weapon = entity.weapons[1];
        if (onDown && attackBool == false && weapon != null)
        {
            weapon.OnAttackTriggered(entity);
            attackBool = true;
        }
        if (onDown == false && onHeld && attackBool && weapon != null)
        {
            weapon.OnAttackHeld(entity);
        }
        if (onUp && attackBool && weapon != null)
        {
            weapon.OnAttackEnd(entity);
            attackBool = false;
        }

    }
}
