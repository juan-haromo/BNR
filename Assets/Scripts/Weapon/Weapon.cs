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
        if(other.gameObject == owner.gameObject || hitBox.Contains(other)) {return;}
        if(other.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.DealDamage(baseDamage + owner.CurrentDamage, owner.gameObject);
        }
        HitBoxChange(false);
    }
}
