using UnityEngine;

public class WeaponCollectable : ACollectable
{
    [SerializeField] WeaponData weaponData;

    public override void OnCollect(Collider other)
    {
        if(other.gameObject.TryGetComponent<NetworkWeapon>(out NetworkWeapon holder))
        {
            holder.ChangeWeapon(weaponData.id);
            ChangeActive(false);
        }
    }
}
