using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 1;
    private HealthManager playerHealthScript;
    private GameObject player;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealthScript = player.GetComponent<HealthManager>();
    }
    private void OnEnable()
    {
        // Deal damage to the enemy
        playerHealthScript.TakeDamage(damage);
        this.enabled = false;
    }
}
