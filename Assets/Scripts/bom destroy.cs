using UnityEngine;

public class bomdestroy : MonoBehaviour
{
    public float lifetime = 2.5f;

    void Start()
    {
        // Hủy object sau 'lifetime' giây
        Destroy(gameObject, lifetime);
    }
}

