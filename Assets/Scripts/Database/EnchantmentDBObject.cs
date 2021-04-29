using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class EnchantmentDBObject : ScriptableObject
{
    [SerializeField] EnchantmentData[] Enchantments;
    public EnchantmentData[] GetEnchantments() { return Enchantments; }
}
