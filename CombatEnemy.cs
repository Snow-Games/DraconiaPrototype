using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CombatEnemy : BaseClass
{
    public enum Type { Paladin, Mage, Boss, Minion, Creature, DarkKnight, Undead, Demon, CorruptSpirit }
    public enum Rarity { Common, Uncommon, Rare, SuperRare, UltraRare, XRare, SpritRare }

    public Type EnemyClass;
    public Rarity rarity;
}
