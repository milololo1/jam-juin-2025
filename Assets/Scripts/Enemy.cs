using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] GameObject healthBarPrefab;
    private HealthBar healthBar;

    public bool is_dead;

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
        Debug.Log(currentHealth);
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
}
