using Mirror;
using System.Collections;
using UnityEngine;

public class DummyEnemy : NetworkBehaviour, IDamagable
{
    [SerializeField] float maxHp;
    float currentHP;
    [SyncVar(hook = nameof(AliveChange))]
    bool isAlive = true;

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHP = maxHp;
    }

    public void DealDamage(float amount, GameObject dealer)
    {
        currentHP -= Mathf.Abs(amount);
        Debug.Log(dealer + " hit " + gameObject + " with " + amount + " damage");
        if (currentHP <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        StartCoroutine(Revive());
        isAlive = false;
    }

    void AliveChange(bool _oldAlive, bool _newAlive)
    {
        gameObject.SetActive(_newAlive);
    }

    IEnumerator Revive()
    {
        yield return new WaitForSeconds(5.0f);
        isAlive = true;
        currentHP = maxHp;
    }
}
