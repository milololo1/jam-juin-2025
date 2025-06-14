using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private float life;
    private float speed;
    private bool use_alignement;
    private Vector3 alignement;
    private Vector3 velocity;

    private BulletPool pool;

    public void fire(Vector3 position, Vector3 velocity, float speed, BulletPool pool, float life=2f)
    {
        transform.position = position;
        transform.forward = velocity.normalized;
        this.velocity = velocity;
        this.speed = speed;
        this.life = life;
        this.pool = pool;
        use_alignement = false;
        gameObject.SetActive(true);
    }
    public void fire(Vector3 position, Vector3 velocity, float speed, Vector3 alignement, BulletPool pool, float life=2f)
    {
        transform.position = position;
        transform.forward = velocity.normalized;
        this.velocity = velocity;
        this.speed = speed;
        this.life = life;
        this.pool = pool;
        use_alignement = true;
        this.alignement = alignement;
        gameObject.SetActive(true);
    }

    public void retire()
    {
        gameObject.SetActive(false);
        pool.recover(this);
    }

    void Update()
    {
        if (use_alignement)
        {
            Vector3 acceleration = steer_force(alignement);
            velocity += acceleration * Time.deltaTime;
            velocity = Vector3.ClampMagnitude(velocity, speed);
            velocity.z = 0;

            transform.position += velocity * Time.deltaTime;
            transform.forward = velocity.normalized;
        }
        else
        {
            transform.position += velocity * Time.deltaTime;
        }

        life -= Time.deltaTime;
        if (life <= 0)
        {
            retire();
        }
    }

    private Vector3 steer_force(Vector3 direction)
    {
        Vector3 v = direction.normalized - velocity.normalized;
        return Vector3.ClampMagnitude(v*speed, speed);
    }
}
