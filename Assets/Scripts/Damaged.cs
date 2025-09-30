using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Damaged : MonoBehaviour
{
    public float maxhealth = 100f; 
    public float currenthealth;
    public Text textHealth;
    void Start()
    {
        currenthealth = maxhealth;
        UpdateHealthText();
    }
    private void Update()
    {
        UpdateHealthText();
    }
    public void PlayerTakeDamage(float damage)
    {
        currenthealth -= damage;
        UpdateHealthText();
        if(currenthealth <= 0)
        {
            Die();
        }
    }
    public void UpdateHealthText()
    {
        textHealth.text = "" + currenthealth.ToString();
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Player hit by enemy");
            PlayerTakeDamage(20f);
        }
    }
    void Die()
    {
        Debug.Log("Player died");
    }
}
