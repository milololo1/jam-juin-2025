using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid : MonoBehaviour
{
    public Vector3 position, forward;
    public Vector3 velocity;

    private Flock flock;

    private static int number_collision_rays = 50;

    private static int collision_mask = 1 << 7;
    private static int boid_mask = 1 << 6;

    public void initialize(Vector3 position, Vector3 forward, Flock flock)
    {
        this.flock = flock;
        this.position = position;
        this.forward = forward;
        transform.position = position;
        transform.forward = forward;
        velocity = transform.forward * (flock.min_speed + flock.max_speed) * 0.5f;
    }

    public void update_position()
    {
        Vector3 acceleration = Vector3.zero;

        if (perceive_boids(out Vector3 center_boids, out Vector3 heading_boids, out Vector3 separation_heading))
        {
            acceleration += steer_force(center_boids - position); // cohesion
            acceleration += steer_force(heading_boids);                     // alignment
            acceleration += steer_force(separation_heading);                // separation
        }

        if (will_collide(out Vector3 avoid_heading))
        {
            acceleration += steer_force(avoid_heading) * 10; // stronger priority
        }

        // update
        velocity += acceleration * Time.deltaTime;
        velocity.z = 0;
        float speed = Mathf.Clamp(velocity.magnitude, flock.min_speed, flock.max_speed);
        Vector3 direction = velocity.normalized;
        velocity = direction * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = direction;

        // cache
        position = transform.position;
        forward = transform.forward;
    }

    public void update_position(Vector3 flock_acceleration)
    {
        if (will_collide(out Vector3 avoid_heading))
        {
            flock_acceleration += steer_force(avoid_heading) * 50; // stronger priority
        }

        // update
        velocity += flock_acceleration * Time.deltaTime;
        velocity.z = 0;
        float speed = Mathf.Clamp(velocity.magnitude, flock.min_speed, flock.max_speed);
        Vector3 direction = velocity.normalized;
        velocity = direction * speed;

        transform.position += velocity * Time.deltaTime;
        transform.forward = direction;

        // cache
        position = transform.position;
        forward = transform.forward;
    }

    private bool perceive_boids(out Vector3 center_boids, out Vector3 heading_boids, out Vector3 separation_heading)
    {
        center_boids = Vector3.zero;
        heading_boids = Vector3.zero;
        separation_heading = Vector3.zero;

        Collider[] hitColliders = Physics.OverlapSphere(position, Mathf.Sqrt(flock.sqr_perception_radius), boid_mask);
        if (hitColliders.Length > 1) // always include itself
        {
            foreach (Collider collider in hitColliders)
            {
                if (collider.TryGetComponent(out Boid boid))
                {
                    if (boid != this)
                    {
                        center_boids += boid.position;
                        heading_boids += boid.forward;
                        Vector3 offset = boid.position - position;
                        float sqr_distance = offset.sqrMagnitude;
                        if (sqr_distance < flock.sqr_avoidance_radius)
                        {
                            separation_heading -= offset / sqr_distance;
                        }
                    }
                }
            }

            center_boids /= hitColliders.Length;
            return true;
        }
        else return false;
    }

    private bool will_collide(out Vector3 direction)
    {
        direction = Vector3.zero;
        if (Physics.Raycast(position, forward, flock.collision_avoidance_distance, collision_mask))
        {
            float dtheta = 180f / number_collision_rays;
            for (int i = 0; i < number_collision_rays; i++)
            {
                float index = i * dtheta * (i % 2 == 0 ? 1 : -1);
                direction.x = Mathf.Cos(index * dtheta);
                direction.y = Mathf.Sin(index * dtheta);
                direction.z = 0;
                if (!Physics.Raycast(position, direction.normalized, flock.collision_avoidance_distance, collision_mask)) return true;
            }
        }
        return false;
    }

    private Vector3 steer_force(Vector3 direction)
    {
        Vector3 v = direction.normalized * flock.max_speed - velocity;
        return Vector3.ClampMagnitude(v, flock.max_steer_force);
    }
}
