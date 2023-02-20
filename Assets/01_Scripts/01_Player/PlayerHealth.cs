using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    // The maximum health of the player
    public int maxHealth = 100;

    // The current health of the player
    public int currentHealth;

    // The amount of damage taken when hit by an enemy attack
    public int damageTaken = 10;

    // A reference to the player's animator component
    private Animator animator;

    // A reference to the player's audio source
    private AudioSource audioSource;

    // A reference to the game over screen
    public GameObject gameOverScreen;

    // The amount of health to restore per second
    public int healingRate = 5;

    // The time in seconds between each healing tick
    public float healingInterval = 1.0f;

    // A flag indicating whether the player is currently being damaged
    private bool isDamaged = false;

    public Image damageScreen;
    Color Color;

    public HealthBar healthBar;

    // declare audiomanager
    public AudioManager audioManager;

    public float heartbeatVolume = 0.5f;

    private void Start()
    {
        // Set the current health to the maximum health
        currentHealth = maxHealth;

        // Get a reference to the player's animator component
        animator = GetComponent<Animator>();

        // Get a reference to the audio manager
        audioManager = FindObjectOfType<AudioManager>();

        //play Heartbeat sound
        audioManager.Play("Heartbeat");

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            // remove 10 points of damage from the player's health
            TakeDamage(10);
            isDamaged = true;
        }
        if (currentHealth == maxHealth)
        {
            StopCoroutine(RestoreHealth());
        }

        //play heartbeat and show bloodoverlay based on health
        ImageOverlay();
        HeartbeatSound();
    }

    public void TakeDamage(int damage)
    {
        // Decrease the current health by the amount of damage taken
        currentHealth -= damage;


        // Check if the player is still alive
        if (currentHealth > 0)
        {

            // Set the damaged flag to true
            isDamaged = true;

            // Start a coroutine to restore the player's health over time
            StartCoroutine(DelayHealing());

            // Setting healthbar to current health
            healthBar.SetHealth(currentHealth);
        }
        else
        {
            // Trigger the death animation
            animator.SetTrigger("Die");

            // Play the death sound

            // Show the game over screen
            // gameOverScreen.SetActive(true);
        }


    }

    IEnumerator DelayHealing()
    {
        // Wait 5 seconds before restoring health
        yield return new WaitForSeconds(5.0f);

        // Set the damaged flag to false
        isDamaged = false;

        if (!isDamaged)
        {
            StopCoroutine(DelayHealing());
            StartCoroutine(RestoreHealth());
        }

    }

    IEnumerator RestoreHealth()
    {
        // Wait for the specified interval before restoring health
        yield return new WaitForSeconds(healingInterval);


        // Restore health as long as the player is not being damaged
        while (!isDamaged)
        {
            // Increase the current health by the healing rate
            currentHealth = Mathf.Min(currentHealth + healingRate, maxHealth);

            // Wait for the next healing interval
            yield return new WaitForSeconds(healingInterval);

            // Setting healthbar to current health
            healthBar.SetHealth(currentHealth);
        }

        if (isDamaged)
        {
            StopCoroutine(RestoreHealth());
        }
    }

    void ImageOverlay()
    {
        // Calculate the opacity of the blood overlay based on the health value
        // The higher the health value, the less opaque the overlay should be
        float alpha = 1.0f - (float)currentHealth / (float)maxHealth;

        // Set the alpha value of the damage screen image
        Color.a = alpha;
        damageScreen.color = Color;
    }

    void HeartbeatSound()
    {
        float volume = 0.5f - (float)currentHealth / (float)maxHealth;

        audioManager.Volume("Heartbeat", volume);
    }
}
