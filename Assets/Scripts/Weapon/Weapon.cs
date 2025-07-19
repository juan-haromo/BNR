using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : NetworkBehaviour 
{
    public List<Collider> hitBox;
    [SerializeField] float baseDamage;
    [SerializeField] PlayerStats owner;
    public void InitializeWeapon(PlayerStats owner)
    {
        this.owner = owner;
    }

    public void HitBoxChange(bool newHitBox)
    {
        foreach (Collider c in hitBox)
        {
            c.enabled = newHitBox;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided");
        if(other.gameObject == owner.gameObject || hitBox.Contains(other)) {return;}
        Debug.Log("Trying to damage");
        if(other.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            Debug.Log("Damage");
            damagable.DealDamage(baseDamage + owner.CurrentDamage, owner.gameObject);
        }
        HitBoxChange(false);
    }
}
