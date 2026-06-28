using UnityEngine;

public class Bullet : MonoBehaviour
{
    void OnCollisionEnter()
    {
        Destroy(gameObject, 0.1f);
    }
}
