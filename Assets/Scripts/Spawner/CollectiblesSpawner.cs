using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Collections;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class CollectiblesSpawner : NetworkBehaviour
{
    [SerializeField] List<ACollectable> m_Collectables;
    [SerializeField] float spawnRadius;
    [SerializeField] float spawnDelay;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
    public override void OnStartServer()
    {
        base.OnStartServer();

    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        StartCoroutine(SpawnDelay());
    }

    [Server]
    void SpawnItem()
    {
        ACollectable item = m_Collectables[Random.Range(0,m_Collectables.Count)];
        Vector3 spawnPos = transform.position;
        Vector2 offset = Random.insideUnitSphere * spawnRadius;
        spawnPos += new Vector3(offset.x, 0, offset.y);
        GameObject instance = (GameObject)Instantiate(item.gameObject,spawnPos,item.gameObject.transform.rotation);
        NetworkServer.Spawn(instance);
    }

    IEnumerator SpawnDelay()
    {
        while (true)
        {
            SpawnItem();
            yield return new WaitForSeconds(spawnDelay);
        }

    }

}
