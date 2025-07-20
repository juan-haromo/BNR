using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.UI.GridLayoutGroup;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class EnemyStats : NetworkBehaviour, IDamagable, IPoolableEnemy
{
    #region Unity Callbacks

    /// <summary>
    /// Add your validation code here after the base.OnValidate(); call.
    /// </summary>
    protected override void OnValidate()
    {
        base.OnValidate();
    }



    // NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.
    void Awake()
    {
    }

    void Start()
    {
    }

    #endregion

    #region Start & Stop Callbacks

    /// <summary>
    /// This is invoked for NetworkBehaviour objects when they become active on the server.
    /// <para>This could be triggered by NetworkServer.Listen() for objects in the scene, or by NetworkServer.Spawn() for objects that are dynamically created.</para>
    /// <para>This will be called for objects on a "host" as well as for object on a dedicated server.</para>
    /// </summary>
    public override void OnStartServer() { }

    /// <summary>
    /// Invoked on the server when the object is unspawned
    /// <para>Useful for saving object data in persistent storage</para>
    /// </summary>
    public override void OnStopServer() { }

    /// <summary>
    /// Called on every NetworkBehaviour when it is activated on a client.
    /// <para>Objects on the host have this function called, as there is a local client on the host. The values of SyncVars on object are guaranteed to be initialized correctly with the latest state from the server when this function is called on the client.</para>
    /// </summary>
    public override void OnStartClient() { }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() { }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer() { }

    /// <summary>
    /// Called when the local player object is being stopped.
    /// <para>This happens before OnStopClient(), as it may be triggered by an ownership message from the server, or because the player object is being destroyed. This is an appropriate place to deactivate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStopLocalPlayer() { }

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnectionToClient parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority() { }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }
    #endregion

    #region Health
    [SerializeField] float maxHp;
    [SyncVar]
    float currentHP;
    [SyncVar(hook = nameof(AliveChange))]
    bool isAlive = true;
    public bool IsAlive => isAlive;
    [SerializeField] List<MeshRenderer> enemyMesh;
    [SerializeField] GameObject brain;
    [SerializeField] Collider enemyCollider;
    [SerializeField] StatType statType;
    [SerializeField] float statGivenAmount;
    [SerializeField] Vector3 purgatorio;

    public void DealDamage(float amount, GameObject dealer)
    {
        Debug.Log(gameObject.name + " was damaged by " + dealer.name);
        if (dealer == gameObject) { return; }
        CmdTakeDamage(amount, dealer);
    }

    [Server]
    void CmdTakeDamage(float amount, GameObject dealer)
    {
        currentHP -= Mathf.Abs(amount);
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
        pool.AddToPool(this);
    }


    void AliveChange(bool _oldAlive, bool _newAlive)
    {
        foreach (MeshRenderer mesh in enemyMesh)
        {
            mesh.enabled = _newAlive;
        }
        brain.SetActive(_newAlive);
        enemyCollider.enabled = _newAlive;
        transform.position = _newAlive ? RandomSpawnPos() : purgatorio;
        if (!_newAlive)
        {
            EnterPool();
        }
    }


    #endregion

    #region EnemyPool
    IEnemyPool pool;
    [SyncVar]
    Vector3 startPosition;
    public Vector3 StartPosition => startPosition;
    [SyncVar]
    float spawnRadius;
    public float SpawnRadius => spawnRadius;
    public void Initialize(IEnemyPool pool, Vector3 startPosition, float spawnRadius, Vector3 purgatorio)
    {
        this.pool = pool;
        this.startPosition = startPosition;
        this.spawnRadius = spawnRadius;
    }

    public void ExitPool()
    {
        CmdExitPool();
    }

    [Server]
    void CmdExitPool()
    {
        currentHP = maxHp;
        isAlive = true;
    }

    Vector3 RandomSpawnPos()
    {
        Vector2 offset = (UnityEngine.Random.insideUnitCircle * spawnRadius);
        return new Vector3(startPosition.x + offset.x, startPosition.y, startPosition.z + offset.y);
    }

    public void EnterPool()
    {
        CmdEnterPool();
    }

    [Server]
    void CmdEnterPool()
    {
        isAlive = false;
    }
    
    #endregion
}

[Serializable]
public enum StatType
{
    HP,
    Stamina,
    Damage
}
