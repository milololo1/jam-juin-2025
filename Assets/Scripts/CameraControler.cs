using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public float transition_time = 0.5f;
    private float timer;

    private Vector2Int start, end;
    private Vector2 direction;

    private Vector3 getPosition(Vector2 coord)
    {
        return new Vector3(RoomStructure.full_room_length * coord.y, RoomStructure.full_room_width * coord.x, -16);
    }

    private float ease_out_cubic(float t)
    {
        return 1 - Mathf.Pow(1 - t, 3);
    }

    public void transition(Vector2Int start, Vector2Int end, bool skip_transition=false)
    {
        this.start = start;
        this.end = end;
        direction = end - start;

        if (skip_transition)
        {
            timer = 0;
            transform.position = getPosition(end);
        }
        else
        {
            timer = transition_time;
        }
    }

    void Start()
    {
        timer = 0;    
    }

    void Update()
    {
        if (timer >  0)
        {
            timer -= Time.deltaTime;

            float t = ease_out_cubic(1-Mathf.Max(timer / transition_time, 0));
            transform.position = getPosition(start + direction * t);
        }
    }
}
