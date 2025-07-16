using UnityEngine;

public class Weapon : MonoBehaviour
{
    [SerializeField] Animation ligthAttack;
    [SerializeField] Animation heavyAttack;
    [SerializeField] float bonusDamage;
    public float BonusDamage { get => bonusDamage; private set { bonusDamage = value; } }

    public void Attack(Animator animator)
    {

    }
}
