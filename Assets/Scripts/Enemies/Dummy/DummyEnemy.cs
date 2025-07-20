using Mirror;
using System;
using System.Collections;
using UnityEngine;

public class DummyEnemy : NetworkBehaviour, IDamagable
{
    [SerializeField] float maxHp;
    [SyncVar]
    float currentHP;
    [SyncVar(hook = nameof(AliveChange))]
    bool isAlive = true;
    [SerializeField] MeshRenderer enemyMesh;
    [SerializeField] Collider enemyCollider;
    [SerializeField] Rigidbody rb;
    [SerializeField] StatType statType;
    [SerializeField] float statGivenAmount;
    IEnemyPool pool;

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public void DealDamage(float amount, GameObject dealer)
    {
        CmdDealDamage(amount, dealer);
    }

    [Command(requiresAuthority = false)]
    public void CmdDealDamage(float amount, GameObject dealer)
    {

        currentHP -= Mathf.Abs(amount);
        Debug.Log(dealer + " hit " + gameObject + " with " + amount + " damage");
        if (currentHP <= 0)
        {
            Die(dealer);
        }
    }

    [Server]
    void Die(GameObject dealer)
    {
        if (dealer.TryGetComponent<PlayerStats>(out PlayerStats stats))
        {
            stats.IncreaseStat(statType, statGivenAmount);
        }
    }

    void AliveChange(bool _oldAlive, bool _newAlive)
    {
        rb.useGravity = _newAlive;
        rb.linearVelocity = Vector3.zero;
        enemyMesh.enabled = _newAlive;
        enemyCollider.enabled = _newAlive;
        if (!_newAlive)
        {
            EnterPool();
        }
    }

    public void Initialize(IEnemyPool pool, Vector3 startPosition, float spawnRadius)
    {
       this.pool = pool;
    }

    public void EnterPool()
    {
        isAlive = false;
    }    
    public void ExitPool()
    {
        isAlive = true;
    }
}

