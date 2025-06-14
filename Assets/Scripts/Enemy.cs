using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] float maxHealth = 100f;
    private float currentHealth;

    [SerializeField] GameObject healthBarPrefab;
    private HealthBar healthBar;

    private void Start()
    {
        currentHealth = maxHealth;

        GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        GameObject healthBarInstance = Instantiate(healthBarPrefab, transform.position, Quaternion.identity, canvas.transform);
        healthBar = healthBarInstance.GetComponent<HealthBar>();
        healthBar.SetTarget(transform);
    }

    public void TakeDamage(float amout)
    {
        currentHealth -= amout;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        healthBar.SetHealth(currentHealth, maxHealth);
    }
}
