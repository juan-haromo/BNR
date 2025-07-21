using Mirror;
using UnityEngine;

public class WeaponCollectable : ACollectable
{
    [SerializeField] WeaponData weaponData;

    public override void OnCollect(Collider other)
    {
        if(other.gameObject.TryGetComponent<NetworkWeapon>(out NetworkWeapon holder))
        {
            if (other.gameObject.GetComponent<PlayerStats>().isExpactating) { return; }
            holder.ChangeWeapon(weaponData.id);
            NetworkServer.UnSpawn(gameObject);
        }
    }
}
