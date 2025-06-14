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

    private List<Boid> boids;

    public void initialize()
    {
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
            acceleration += steer_force(center_boids, boids[idx_boid].velocity);                        // cohesion
            acceleration += steer_force(heading_boids, boids[idx_boid].velocity);                       // alignment
            acceleration += steer_force(separation_heading, boids[idx_boid].velocity);                  // separation
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
        /*foreach (Boid boid in boids)
        {
            boid.update_position();
        }*/
        for (int i = 0; i < size; i++)
        {
            boids[i].update_position(perceive_boids(i));
        }
    }
}
