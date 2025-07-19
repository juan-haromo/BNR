using System.Collections.Generic;
using UnityEngine;
using Mirror;

/*
	Documentation: https://mirror-networking.gitbook.io/docs/guides/networkbehaviour
	API Reference: https://mirror-networking.com/docs/api/Mirror.NetworkBehaviour.html
*/

public class NetworkWeapon : NetworkBehaviour
{
    Dictionary<int, Weapon> loadedWeapons;
    [SerializeField] PlayerStats owner;
    [SerializeField] Transform weaponSocket;
    public Weapon CurrentWeapon { get; private set; }
    [SyncVar(hook = nameof(WeaponChanged))]
    int currentWeaponId = int.MinValue;
    public void ChangeWeapon(int weaponID)
    {
        CmdChangeWeapon(weaponID);
    }

    [Command(requiresAuthority = false)]
    void CmdChangeWeapon(int weaponID)
    {

        currentWeaponId = weaponID;
    }

    void WeaponChanged(int oldWeapon, int newWeapon)
    {
        //Turn off old active weapon
        if (loadedWeapons.ContainsKey(oldWeapon))
        {
            loadedWeapons[oldWeapon].gameObject.SetActive(false);
        }
        //Turn on loaded weapon if exists
        if (loadedWeapons.ContainsKey(newWeapon))
        {
            loadedWeapons[newWeapon].gameObject.SetActive(true);
            CurrentWeapon = loadedWeapons[newWeapon];
            return;
        }
        //Load weapon if not already loaded
        WeaponData weaponData = WeaponServer.Instance.GetWeapon(newWeapon);
        if (weaponData == null) { Debug.LogError("Weapon with ID " + newWeapon + " is not valid"); return; }
        GameObject weaponInstance = Instantiate(weaponData.weapon.gameObject, weaponSocket);
        Weapon weapon = weaponInstance.GetComponent<Weapon>();
        weapon.InitializeWeapon(owner);
        loadedWeapons.Add(weaponData.id, weaponInstance.GetComponent<Weapon>());
        CurrentWeapon = loadedWeapons[newWeapon];
        if (isServer) { NetworkServer.Spawn(weaponInstance,gameObject); }
    }



    [SyncVar(hook = nameof(HitBoxChange))]
    bool isHitBoxEnable;
    public List<Collider> hitBox;


    public void ChangeHitbox(bool isActive)
    {
        CommandChangeHitbox(isActive);
    }

    [Command(requiresAuthority = false)]
    void CommandChangeHitbox(bool isActive)
    {
        isHitBoxEnable = isActive;
    }
    void HitBoxChange(bool oldHitBox, bool newHitBox)
    {
        CurrentWeapon.HitBoxChange(newHitBox);
    }
   

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
    public override void OnStartClient()
    {
        loadedWeapons = new Dictionary<int, Weapon>();
        ChangeWeapon(2);
    }

    /// <summary>
    /// This is invoked on clients when the server has caused this object to be destroyed.
    /// <para>This can be used as a hook to invoke effects or do client specific cleanup.</para>
    /// </summary>
    public override void OnStopClient() { }

    /// <summary>
    /// Called when the local player object has been set up.
    /// <para>This happens after OnStartClient(), as it is triggered by an ownership message from the server. This is an appropriate place to activate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStartLocalPlayer()
    {

    }

    /// <summary>
    /// Called when the local player object is being stopped.
    /// <para>This happens before OnStopClient(), as it may be triggered by an ownership message from the server, or because the player object is being destroyed. This is an appropriate place to deactivate components or functionality that should only be active for the local player, such as cameras and input.</para>
    /// </summary>
    public override void OnStopLocalPlayer() {}

    /// <summary>
    /// This is invoked on behaviours that have authority, based on context and <see cref="NetworkIdentity.hasAuthority">NetworkIdentity.hasAuthority</see>.
    /// <para>This is called after <see cref="OnStartServer">OnStartServer</see> and before <see cref="OnStartClient">OnStartClient.</see></para>
    /// <para>When <see cref="NetworkIdentity.AssignClientAuthority">AssignClientAuthority</see> is called on the server, this will be called on the client that owns the object. When an object is spawned with <see cref="NetworkServer.Spawn">NetworkServer.Spawn</see> with a NetworkConnectionToClient parameter included, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStartAuthority()
    {

    }

    /// <summary>
    /// This is invoked on behaviours when authority is removed.
    /// <para>When NetworkIdentity.RemoveClientAuthority is called on the server, this will be called on the client that owns the object.</para>
    /// </summary>
    public override void OnStopAuthority() { }

    #endregion
}
