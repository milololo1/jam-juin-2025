using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    public float transition_time = 0.5f;
    private float timer;

    private Vector2Int start_coord, end_coord;

    private Vector3 getPosition(Vector2Int coord)
    {
        return Vector3.zero;
    }

    void Start()
    {
        timer = 0;    
    }

    void Update()
    {
        if (timer >  0)
        {

        }
    }
}
