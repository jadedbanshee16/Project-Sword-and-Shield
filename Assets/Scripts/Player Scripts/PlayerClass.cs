using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : MonoBehaviour
{
    private Inventory _inv;

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


    // Start is called before the first frame update
    void Start()
    {
        _inv = GameObject.FindGameObjectWithTag("GameManager").GetComponent<Inventory>();
        currentStamina = maximumStamina;
        currentHealth = maximumHealth;
    }

    // Update is called once per frame
    void Update()
    {
        //Now, update the stamina if needed.
        if (currentStamina < maximumStamina)
        {
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

        _inv.updateStamina(currentStamina, maximumStamina);
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
        currentHealth -= rm;

        if(currentHealth <= 0)
        {
            //Get the game manager and reset the level.
            GetComponent<GameManager>().restartGame();

            //Reset the health.
            currentHealth = maximumHealth;
        }
    }
}
