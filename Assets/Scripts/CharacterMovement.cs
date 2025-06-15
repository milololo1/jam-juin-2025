using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    private PlayerStats stats;

    private const float playerAccel = 5f;
    private const float maxSpeed = 10f;
    private const float baseSpeed = 5f;

    private float playerSpeed;

    public static Vector2Int room_location;

    private Vector2Int get_room_location()
    {
        Vector3 position = transform.position;

        int x = Mathf.RoundToInt(position.y / RoomStructure.full_room_width);
        int y = Mathf.RoundToInt(position.x / RoomStructure.full_room_length);

        return new Vector2Int(x, y);
    }

    public Direction get_quadrant()
    {
        Vector3 position = transform.position;

        float dx = position.y / RoomStructure.full_room_width - Mathf.RoundToInt(position.y / RoomStructure.full_room_width);
        float dy = position.x / RoomStructure.full_room_length - Mathf.RoundToInt(position.x / RoomStructure.full_room_length);

        bool x_dominant = Mathf.Abs(dx) > Mathf.Abs(dy);

        if (x_dominant) return dx > 0 ? Direction.north : Direction.south;
        else return dy > 0 ? Direction.east : Direction.west;
    }

    void Start()
    {
        this.stats = this.gameObject.GetComponent<PlayerStats>();

        playerSpeed = baseSpeed;
    }

    void Update()
    {
        float time = Time.deltaTime;
        if (Input.anyKey)
        {
            playerSpeed = Mathf.Min(maxSpeed, playerSpeed + playerSpeed * time * playerAccel);

            Vector3 direction = Vector3.zero;

            if (Input.GetKey(KeyCode.W)) direction.y += 1;
            if (Input.GetKey(KeyCode.S)) direction.y -= 1;
            if (Input.GetKey(KeyCode.A)) direction.x -= 1;
            if (Input.GetKey(KeyCode.D)) direction.x += 1;
            direction = direction.normalized;

            transform.position += direction * playerSpeed * time;

            room_location = get_room_location();
            PlayerStats.player_pos = this.gameObject.transform.position;
        }
        else
        {
            playerSpeed = Mathf.Max(baseSpeed, playerSpeed - playerSpeed * time * playerAccel * 5);
        }
    }
}
