using UnityEngine;

[CreateAssetMenu(fileName = "WeaponData", menuName = "Scriptable Objects/WeaponData")]
public class WeaponData : ScriptableObject
{
    public int id;

    public AnimatorOverrideController controller;

    public Weapon weapon;

    public override int GetHashCode()
    {
        return id;
    }
}
