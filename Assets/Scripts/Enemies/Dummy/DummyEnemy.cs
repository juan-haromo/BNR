using Mirror;
using System;
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
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

  
    void Die(GameObject dealer)
    {
        
        if (dealer.TryGetComponent<PlayerStats>(out PlayerStats stats))
        {
            stats.IncreaseStat(statType, statGivenAmount);
        }
        
        StartCoroutine(Revive());
        isAlive = false;
    }

    void AliveChange(bool _oldAlive, bool _newAlive)
    {
        rb.useGravity = _newAlive;
        enemyMesh.enabled = _newAlive;
        enemyCollider.enabled = _newAlive;
    }

    IEnumerator Revive()
    {
        yield return new WaitForSeconds(5.0f);
        isAlive = true;
        currentHP = maxHp;
    }
}

[Serializable]
public enum StatType
{
    HP,
    Stamina,
    Damage
}