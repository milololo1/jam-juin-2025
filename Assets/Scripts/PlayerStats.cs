using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    //Class for player stats logic

    public static float max_hp = 100f;
    public static float current_hp = max_hp;
    public static float attack_power = 1f;
    public static float attack_speed = 1f;
    public static float movement_speed = 1f;

    public static Vector3 player_pos;

    public static float max_invincibility_time = 1f;
    private float invincibility_timer;
    private bool is_invincible;
    private float invincibility_flash_time;

    [SerializeField] AudioSource damageSource;
    [SerializeField] AudioSource upgradeSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("upgrade")) //Le tag n'existe pas encore je crois
        {
            upgradeSource.Play();
        }

        if ((collision.gameObject.CompareTag("boid") || collision.gameObject.CompareTag("enemy")) && !is_invincible)
        {
            take_damage(10);
            is_invincible = true;

            damageSource.Play();
            //Animation � mettre en place pendant l'invincibilit� (du style un clignotement ou un truc du genre)
        }
    }

    public static void take_damage(float damage)
    {

        //A link avec la healtbar du joueur (je sais pas o� c'est afficher et si �a existe d�j�)
        current_hp -= damage;

        if(current_hp < 0)
        {
            //Game over (logique � d�terminer)
        }
    }

    public static void heal_hp(float hp)
    {
        current_hp = Mathf.Max(current_hp + hp, max_hp);
    }

    // Start is called before the first frame update
    void Start()
    {
        invincibility_timer = 0f;
        invincibility_flash_time = 0f;
        is_invincible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_invincible)
        {
            invincibility_timer += Time.deltaTime;
            invincibility_flash_time += Time.deltaTime;
            if(invincibility_flash_time > 0.2)
            {
                this.gameObject.GetComponent<MeshRenderer>().enabled = !this.gameObject.GetComponent<MeshRenderer>().enabled;
                invincibility_flash_time = 0;
            }
            if(invincibility_timer > max_invincibility_time)
            {
                is_invincible = false;
                invincibility_timer = 0f;
                invincibility_flash_time -= 0;
                this.gameObject.GetComponent<MeshRenderer>().enabled = true;
            }
        }
    }
}
