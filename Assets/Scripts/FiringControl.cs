using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BulletMode
{
    standard,
    high_density,
    aligned,
    slow
}

public struct BulletSetting
{
    public BulletSetting(float firing_delay, float speed, float life, float noise, bool use_alignement=false)
    {
        this.firing_delay = firing_delay;
        this.speed = speed;
        this.life = life;
        this.noise = noise;
        this.use_alignement = use_alignement;
    }


    public float firing_delay;
    public float speed;
    public float life;
    public float noise;
    public bool use_alignement;
}

public class FiringControl : MonoBehaviour
{
    public BulletPool pool;
    private float timer;

    public float spawn_distance = 2.5f;

    private BulletMode current_mode;
    private BulletSetting current_setting;

    public static BulletSetting standard, high_density, aligned, slow;

    void Start()
    {
        standard = new BulletSetting(0.2f, 8f, 3f, 0.1f);
        high_density = new BulletSetting(0.05f, 10f, 2f, 0.5f);
        aligned = new BulletSetting(0.3f, 6f, 4f, 0.8f, true);
        slow = new BulletSetting(0.1f, 5f, 5f, 0.05f);

        set_mode(BulletMode.aligned);

        timer = 0;
    }

    void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            Vector3 forward = get_forward();

            float theta_noise = (2 * Random.value - 1) * current_setting.noise;
            Vector3 ortho_forward = new Vector3(forward.y, forward.x, 0);

            //Debug.DrawRay(transform.position, (forward + ortho_forward * theta_noise).normalized * 50, Color.green);

            if (current_setting.use_alignement) pool.fire(transform.position + forward * spawn_distance, (forward + ortho_forward * theta_noise).normalized * current_setting.speed, current_setting.speed, forward, current_setting.life);
            else pool.fire(transform.position + forward * spawn_distance, (forward + ortho_forward * theta_noise).normalized * current_setting.speed, current_setting.speed, current_setting.life);
            timer = current_setting.firing_delay;
        }
    }

    private void set_mode(BulletMode bullet_mode)
    {
        current_mode = bullet_mode;
        switch (bullet_mode)
        {
            case BulletMode.high_density:
                current_setting = high_density;
                break;
            case BulletMode.aligned:
                current_setting = aligned;
                break;
            case BulletMode.slow:
                current_setting = slow;
                break;
            case BulletMode.standard:
            default:
                current_setting = standard;
                break;
        }
    }

    private Vector3 get_forward()
    {
        Vector3 v = Input.mousePosition;
        v.z = 50;
        Vector3 mouse_position = Camera.main.ScreenToWorldPoint(v);
        mouse_position.z = 0;
        return (mouse_position - transform.position).normalized;
    }
}
