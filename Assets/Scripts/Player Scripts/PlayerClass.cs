using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    private Inventory _inv;
    private PlayerMovement _control;

    private float currentStamina;
    public float currentHealth;

    [Header("Stamina variables")]
    //Speed of the stamina regen.
    [SerializeField]
    private float staminaRegenPace;
    private float staminaTimer;
    //The amount of stamina per regen tick.
    [SerializeField]
    private float staminaRegen;
    //Maximum stamina the player can have.
    [SerializeField]
    private float maximumStamina;

    [Header("Health variable")]
    [SerializeField]
    private float maximumHealth;
    [SerializeField]
    private float invincibilityLength;

    private bool invincible;
    private float invincibleTimer;



    // Start is called before the first frame update
    void Start()
    {
        _inv = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Inventory>();
        currentStamina = maximumStamina;
        currentHealth = maximumHealth;
        invincibleTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //Now, update the stamina if needed.
        if (currentStamina < maximumStamina)
        {
            //If stamina is lower than max, reginerate stamina.
            if (staminaTimer < staminaRegenPace)
            {
                staminaTimer += Time.deltaTime;
            }
            else
            {
                currentStamina += staminaRegen;
                staminaTimer = 0;
            }
        } else
        {
            currentStamina = maximumStamina;
        }

        if(invincible && invincibleTimer > 0)
        {
            invincibleTimer -= Time.deltaTime;
        } else
        {
            invincible = false;
        }

        _inv.updateStamina(currentStamina, maximumStamina);
        _inv.updateHealth(currentHealth, maximumHealth);
    }

    public float getStamina()
    {
        return currentStamina;
    }

    public float getHealth()
    {
        return currentHealth;
    }

    //Remove the stamina down to zero.
    public void deductStamina(float rm)
    {
        currentStamina -= rm;

        if (currentStamina < 0)
        {
            currentStamina = 0;
        }
    }

    public void deductHealth(float rm)
    {
        if (!invincible)
        {
            currentHealth -= rm;

            invincible = true;
            invincibleTimer = invincibilityLength;
        }

        if(currentHealth <= 0)
        {
            //Get the game manager and reset the level.
            //GetComponent<GameManager>().restartGame();
            if(_control == null)
            {
                _control = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
            }
            _control.die();
            //Reset the health.
            currentHealth = maximumHealth;
        }
    }
}
