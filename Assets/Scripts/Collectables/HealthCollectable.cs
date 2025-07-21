using Mirror;
using UnityEngine;

public class HealthCollectable : ACollectable
{
    [SerializeField] float amount;
    public override void OnCollect(Collider other)
    {
        if(other.gameObject.TryGetComponent<PlayerStats>(out PlayerStats stats)){
            if (stats.isExpactating) { return; }
            stats.Heal(amount);
            NetworkServer.UnSpawn(gameObject);
        }
    }
}
