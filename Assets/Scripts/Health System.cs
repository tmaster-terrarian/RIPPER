using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Internal;

public class HealthSystem : MonoBehaviour
{
    float hp;
    float armor;
    [SerializeField] float maxHp;
    [SerializeField] float maxArmor;
    [SerializeField] bool invulnerable = false;
    [SerializeField] bool isPlayerUnit;

    Slider healthBar = null;
    Slider armorBar = null;

    // debug
    private WaitForSeconds shotDuration = new WaitForSeconds(0.07f);
    private LineRenderer laserLine;

    void Start()
    {
        // debug
        if(isPlayerUnit)
        {
            laserLine = GetComponent<LineRenderer>();
            healthBar = GameObject.FindGameObjectWithTag("HealthBar").GetComponent<Slider>();
            armorBar = GameObject.FindGameObjectWithTag("ArmorBar").GetComponent<Slider>();
        }
        else
        laserLine = null;
    }

    void Update()
    {
        if(hp <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void TakeDamage(int damage)
    {
        if(invulnerable == false)
        {
            hp -= damage;
            //animator.PlayInFixedTime("Hurt");
        }
    }
}
