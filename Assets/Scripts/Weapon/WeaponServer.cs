using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Unity.VisualScripting;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class WeaponServer : MonoBehaviour
{
    public static WeaponServer Instance { get; private set;}
    [SerializeField] List<WeaponData> levelWeapons; 
    Dictionary<int,WeaponData> weaponsTable;
    

    void RegisterWeapons()
    {
        weaponsTable = new Dictionary<int, WeaponData>();
        foreach (WeaponData weapon in levelWeapons)
        {
            weaponsTable.Add(weapon.id,weapon);
        }
    }

    public WeaponData GetWeapon(int id)
    {
        if(weaponsTable.TryGetValue(id, out WeaponData weapon))
        {
            return weapon;
        }
        return null;
    }


    // NOTE: Do not put objects in DontDestroyOnLoad (DDOL) in Awake.  You can do that in Start instead.
    void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
            RegisterWeapons();
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
    }
}
