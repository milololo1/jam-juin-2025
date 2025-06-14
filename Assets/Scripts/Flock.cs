using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour
{
    public GameObject boid_prefab;

    public int size = 20;
    public float spawn_radius = 10;

    public float min_speed = 5.0f;
    public float max_speed = 20.0f;
    public float max_steer_force = 3.0f;
    public float sqr_perception_radius = 25.0f;
    public float sqr_avoidance_radius = 4f;
    public float collision_avoidance_distance = 5.0f;
    public float separation_strength = 1.0f;

    public bool follow_target = false;
    public float following_strength = 1.0f;
    public float aiming_speed = 20.0f;
    public float circling_speed = 1.0f;
    public float circling_radius = 10f;
    public float theta = 0;
    private Vector3 target;
    private Vector3 target_velocity;

    public Transform target_transform;


    private List<Boid> boids;

    public void initialize()
    {
        target = Vector3.zero;
        target_velocity = Vector3.zero;
        boids = new List<Boid>();
        for (int i = 0; i < size; i++)
        {
            Vector2 rand_position = Random.insideUnitCircle * spawn_radius;
            Vector2 rand_direction = Random.insideUnitCircle;
            Boid boid = Instantiate(boid_prefab, transform).GetComponent<Boid>();
            boid.initialize(transform.position + new Vector3(rand_position.x, rand_position.y), new Vector3(rand_direction.x, rand_direction.y), this);
            boids.Add(boid);
        }
    }
    private Vector3 perceive_boids(int idx_boid)
    {
        Vector3 center_boids = Vector3.zero, heading_boids = Vector3.zero, separation_heading = Vector3.zero;
        Vector3 acceleration = Vector3.zero;

        if (follow_target)
        {
            acceleration += steer_force(target - boids[idx_boid].position, boids[idx_boid].velocity) * following_strength;
        }

        int n_perceived_boids = 0;
        for (int i = 0; i < size; i++)
        {
            if (i != idx_boid )
            {
                Vector3 offset = boids[i].position - boids[idx_boid].position;
                float sqr_dist = offset.sqrMagnitude;
                if (sqr_dist < sqr_perception_radius)
                {
                    center_boids += offset;
                    heading_boids += boids[i].forward;
                    n_perceived_boids += 1;

                    if (sqr_dist < sqr_avoidance_radius)
                    {
                        separation_heading -= offset / sqr_dist;
                    }
                }
            }
        }

        if (n_perceived_boids > 0)
        {
            center_boids /= n_perceived_boids;
            acceleration += steer_force(center_boids, boids[idx_boid].velocity);                            // cohesion
            acceleration += steer_force(heading_boids, boids[idx_boid].velocity);                           // alignment
            acceleration += steer_force(separation_heading, boids[idx_boid].velocity) * separation_strength;// separation
        }

        return acceleration;
    }

    private Vector3 steer_force(Vector3 direction, Vector3 velocity)
    {
        Vector3 v = direction.normalized * max_speed - velocity;
        return Vector3.ClampMagnitude(v, max_steer_force);
    }

    void Start()
    {
        initialize();
    }

    void Update()
    {
        if (follow_target)
        {
            // update target
            if (target_transform != null) // follow transform
            {
                Vector3 target_acceleration = Vector3.zero;
                target_acceleration += steer_force(target_transform.position - target, target_velocity);
                target_velocity += target_acceleration * Time.deltaTime;
                target_velocity = Vector3.ClampMagnitude(target_velocity, aiming_speed);
                target_velocity.z = 0;
                target += target_velocity * Time.deltaTime;
            }
            else // circle around 
            {
                theta += (circling_speed / circling_radius) * Time.deltaTime;
                target.x = Mathf.Cos(theta) * circling_radius;
                target.y = Mathf.Sin(theta) * circling_radius;
                target.z = 0;
            }
        }

        // update boids
        for (int i = 0; i < size; i++)
        {
            boids[i].update_position(perceive_boids(i));
        }
    }
}
