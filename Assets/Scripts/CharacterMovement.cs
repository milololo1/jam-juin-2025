using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public GameObject playerCamera;

    private const float playerAccel = 1.3f;
    private const float CameraAccel = 1f;
    private const float maxSpeed = 5f;
    private const float baseSpeed = 2f;

    private float playerSpeed;
    private float cameraSpeed;

    // Start is called before the first frame update
    void Start()
    {
        playerSpeed = baseSpeed;
        cameraSpeed = baseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float time = Time.deltaTime;
        if (Input.anyKey)
        {
            playerSpeed = Mathf.Min(maxSpeed, playerSpeed + playerSpeed * time * playerAccel);
            cameraSpeed = Mathf.Min(maxSpeed, cameraSpeed + cameraSpeed * time * CameraAccel);

        }
        else
        {
            playerSpeed = Mathf.Max(baseSpeed, playerSpeed - playerSpeed * time * playerAccel * 5);
            cameraSpeed = Mathf.Max(baseSpeed, cameraSpeed - cameraSpeed * time * CameraAccel * 5);
        }

        var diagonalMove = (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S)) && (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.A));
        var adjustedPlayerSpeed = diagonalMove ? playerSpeed / Mathf.Sqrt(2) : playerSpeed;

        var characterPos = this.gameObject.transform.position;
        var cameraPos = playerCamera.transform.position;

        if (Input.GetKey(KeyCode.W))
        {
            this.gameObject.transform.position = new Vector3(characterPos.x, 0, characterPos.z + adjustedPlayerSpeed * time);
            characterPos = this.gameObject.transform.position;
        }
        if (Input.GetKey(KeyCode.A))
        {
            this.gameObject.transform.position = new Vector3(characterPos.x - adjustedPlayerSpeed * time, 0, characterPos.z);
            characterPos = this.gameObject.transform.position;
        }
        if (Input.GetKey(KeyCode.S))
        {
            this.gameObject.transform.position = new Vector3(characterPos.x, 0, characterPos.z - adjustedPlayerSpeed * time);
            characterPos = this.gameObject.transform.position;
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.gameObject.transform.position = new Vector3(characterPos.x + adjustedPlayerSpeed * time, 0, characterPos.z);
        }

        var diffVector = this.gameObject.transform.position - playerCamera.transform.position;
        diffVector = new Vector3(diffVector.x, 0, diffVector.z);
        diffVector = Vector3.ClampMagnitude(diffVector, 1);
        //diffVector.Normalize();

        //var xzMagnitude = Mathf.Abs(diffVector.x) + Mathf.Abs(diffVector.z);
        //var xRatio = xzMagnitude < 0.05 ? 0 : diffVector.x / xzMagnitude;
        //var zRatio = xzMagnitude < 0.05 ? 0 : diffVector.z / xzMagnitude;

        playerCamera.transform.position = new Vector3(cameraPos.x + cameraSpeed * time * diffVector.x, 5, cameraPos.z + cameraSpeed * time * diffVector.z);
    }
}
