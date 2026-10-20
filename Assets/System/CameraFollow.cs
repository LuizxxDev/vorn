using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 newPosition = target.position;
        newPosition.z = transform.position.z; // Mantém a posição Z da câmera
        transform.position = newPosition;
    }
}