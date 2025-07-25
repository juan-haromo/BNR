using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Collections;
using Telepathy;
using Unity.Mathematics;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class Spawner : NetworkBehaviour, IEnemyPool
{
    public List<PoolableEnemy> enemies;
    List<IPoolableEnemy> poolableEnemies;
    public float spawnRadius = 1;
    public float spawnDelay = 2;
    [SerializeField] int maxSpawnEnemies = 3;
    [SyncVar]
    int spawnCount = 0;
    [SerializeField] Transform purgatrorio;
    #region Unity Callbacks

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }

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
    public override void OnStartServer()
    {
        InstantiateEnemies();
    }


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

    #region EnemySpawn
    private void InstantiateEnemies()
    {
        if (!isServer) { return; }
        poolableEnemies = new List<IPoolableEnemy>();
        foreach (PoolableEnemy enemy in enemies)
        {
            if (enemy.poolableEnemy.TryGetComponent<IPoolableEnemy>(out IPoolableEnemy poolableEnemy))
            {
                GameObject enemyInstance = enemy.poolableEnemy;
                for (int i = 0; i < enemy.amount; i++)
                {
                    ServerInstantiateEnemy(enemyInstance);
                }
            }
        }
        StartCoroutine(SpawnDelay());
    }

    [Server]
    void ServerInstantiateEnemy(GameObject enemy)
    {
        GameObject instance = Instantiate(enemy, transform.position, enemy.transform.rotation);
        IPoolableEnemy poolableEnemy = instance.GetComponent<IPoolableEnemy>();
        poolableEnemies.Add(poolableEnemy);
        poolableEnemy.Initialize(this, transform.position,spawnRadius, purgatrorio.position);
        NetworkServer.Spawn(instance);
        poolableEnemy.EnterPool();
    }

    [Server]
    void SpawnEnemy()
    {
        if (!isServer || poolableEnemies.Count <= 0) { return; }
        int i = UnityEngine.Random.Range(0, poolableEnemies.Count);
        RemoveFromPool(poolableEnemies[i]);
    }

    [Server]
    public void AddToPool(IPoolableEnemy poolableEnemy)
    {
        poolableEnemies.Add(poolableEnemy);
        poolableEnemy.EnterPool();
        spawnCount--;
    }

    [Server]
    public void RemoveFromPool(IPoolableEnemy poolableEnemy)
    {
        if (poolableEnemies.Remove(poolableEnemy))
        {
            poolableEnemy.ExitPool();
            spawnCount++;
        }
    }

    IEnumerator SpawnDelay()
    {
        while (true)
        {
            if ((spawnCount < maxSpawnEnemies) && poolableEnemies.Count > 0)
            {
                SpawnEnemy();
            }
            yield return new WaitForSeconds(spawnDelay);
        }
    }
    #endregion

}