using UnityEngine;

public class Amorsupply : MonoBehaviour
{
    public int ammoAmount = 10; // số viên đạn hồi khi nhặt

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Weapon playerShoot = other.GetComponentInChildren<Weapon>();
            if (playerShoot != null)
            {
                playerShoot.AddAmmo(ammoAmount);
                Debug.Log("Nhặt được " + ammoAmount + " viên đạn!");
            }
            Destroy(gameObject);
        }
    }
}
