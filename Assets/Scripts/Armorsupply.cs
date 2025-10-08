using UnityEngine;

public class Armorsupply : MonoBehaviour
{
    public int armorAmount = 10; // số giáp hồi khi nhặt
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Damaged player = other.GetComponent<Damaged>();
            if (player != null)
            {
                player.AddArmor(armorAmount);
                Debug.Log("Nhặt được " + armorAmount + " giáp!");
            }
            Destroy(gameObject);
        }
        if (other.CompareTag("bullet"))
        {
            Destroy(gameObject);
        }
    }
}
