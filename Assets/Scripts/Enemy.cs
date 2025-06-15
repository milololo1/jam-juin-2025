using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyStrategy
{
    nothing,
    attack_player,
    move_to_player,
    move_to_player_and_attack
}

public class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] GameObject healthBarPrefab;
    private HealthBar healthBar;

    public bool is_dead;
    public EnemyStrategy strategy;
    public float enemy_speed = 2f;

    private void Start()
    {
        currentHealth = maxHealth;

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        GameObject healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, canvas.transform);
        healthBar = healthBarInstance.GetComponent<HealthBar>();
        healthBar.SetTarget(transform);
        is_dead = false;
    }

    public void TakeDamage(float amout)
    {
        currentHealth -= amout;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        //Debug.Log(currentHealth);
        healthBar.SetHealth(currentHealth, maxHealth);

        if(currentHealth == 0f)
        {
            this.gameObject.SetActive(false);
            this.healthBar.gameObject.SetActive(false);
            is_dead = true;
        }
    }

    public void destroy_enemy()
    {
        Destroy(this.healthBar.gameObject);
        Destroy(this.gameObject);
    }

    private void Update()
    {
        switch (strategy)
        {
            case EnemyStrategy.nothing:
                //Nothing
                break;
            case EnemyStrategy.attack_player:
                break;
            case EnemyStrategy.move_to_player:
                var direction = PlayerStats.player_pos - this.gameObject.transform.position;
                direction.Normalize();
                this.gameObject.transform.position += direction * enemy_speed * Time.deltaTime;
                break;
            case EnemyStrategy.move_to_player_and_attack:
                break;
        }
    }
}
