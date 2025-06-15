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

    public static float max_invincibility_time = 1f;
    private float invincibility_timer;
    private bool is_invincible;

    [SerializeField] AudioSource damageSource;
    [SerializeField] AudioSource upgradeSource;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("upgrade")) //Le tag n'existe pas encore je crois
        {
            Debug.Log("Touch upgrade");

            upgradeSource.Play();
            //Appeller ici une fonction qui modifie player stat en fonction d'un certain type d'upgrade
            //ex.: abstract class upgrade qui est héritée par upgradeHp, ..., la classe abstract est contenue dans un monobehaviour sur l'object upgrade
        }

        if ((collision.gameObject.CompareTag("boid") || collision.gameObject.CompareTag("enemy")) && !is_invincible)
        {
            take_damage(10);
            is_invincible = true;

            damageSource.Play();
            //Animation à mettre en place pendant l'invincibilité (du style un clignotement ou un truc du genre)
        }
    }

    public static void take_damage(float damage)
    {

        //A link avec la healtbar du joueur (je sais pas où c'est afficher et si ça existe déjà)
        current_hp -= damage;

        if(current_hp < 0)
        {
            //Game over (logique à déterminer)
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
        is_invincible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (is_invincible)
        {
            invincibility_timer += Time.deltaTime;
            if(invincibility_timer > max_invincibility_time)
            {
                is_invincible = false;
                invincibility_timer = 0f;
            }
        }
    }
}
