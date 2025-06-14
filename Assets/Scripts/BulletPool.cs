using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    private List<Bullet> available_bullets;
    private List<Bullet> fired_bullets;

    void Start()
    {
        available_bullets = new List<Bullet>(GetComponentsInChildren<Bullet>(true));
        fired_bullets = new List<Bullet>();
    }

    public void fire(Vector3 position, Vector3 velocity, float speed, Vector3 alignement, float life = 2f)
    {
        if (available_bullets.Count == 0)
        {
            Bullet b_fired = fired_bullets[0];
            b_fired.retire();
        }

        Bullet b = available_bullets[0];
        available_bullets.RemoveAt(0);
        fired_bullets.Add(b);
        b.fire(position, velocity, speed, alignement, this, life);
    }

    public void fire(Vector3 position, Vector3 velocity, float speed, float life = 2f)
    {
        if (available_bullets.Count == 0)
        {
            Bullet b_fired = fired_bullets[0];
            b_fired.retire();
        }

        Bullet b = available_bullets[0];
        available_bullets.RemoveAt(0);
        fired_bullets.Add(b);
        b.fire(position, velocity, speed, this, life);
    }

    public void recover(Bullet bullet)
    {
        fired_bullets.Remove(bullet);
        available_bullets.Add(bullet);
    }
}
