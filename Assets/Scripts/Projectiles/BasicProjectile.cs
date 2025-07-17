using Mirror;
using Unity.VisualScripting;
using UnityEngine;

public class BasicProjectile : NetworkBehaviour
{
    [SerializeField] float lifeTime;
    [SerializeField] float speed;
    [SerializeField] float damage;
    float deathTime;
    [SyncVar]
    GameObject owner;
    [SyncVar]
    Vector3 moveDirection = Vector3.zero;
    public void Initialize(GameObject owner, Vector3 moveDirection)
    {
        ServerInitialize(owner, moveDirection);
    }

    [Server]
    void ServerInitialize(GameObject owner, Vector3 moveDirection)
    {
        this.owner = owner;
        this.moveDirection = moveDirection.normalized;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        deathTime = Time.time + lifeTime;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > deathTime) { Destroy(gameObject); }
        transform.Translate(speed * Time.deltaTime * moveDirection);
    }

    private void OnTriggerEnter(Collider other)
    {  
        if (other.gameObject.TryGetComponent<IDamagable>(out IDamagable damagable))
        {
            damagable.DealDamage(damage, owner);
        }
        Destroy(gameObject);
    }
}
