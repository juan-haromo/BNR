using System;
using UnityEngine;

public interface IPoolableEnemy
{
    void Initialize(IEnemyPool pool);
    public void ExitPool();
    public void EnterPool();
}

public interface IEnemyPool
{
    void AddToPool(IPoolableEnemy poolableEnemy);
    void RemoveFromPool(IPoolableEnemy poolableEnemy);
}

[Serializable]
public struct PoolableEnemy
{
    public GameObject poolableEnemy;
    public int amount;
}
