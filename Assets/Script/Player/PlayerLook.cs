using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    public float lookSpeed = 3.0f;
    public void ProcessLook(Vector2 input)
    {
        float rotationY = input.x * lookSpeed;

        transform.Rotate(Vector3.up, rotationY);
    }
}
