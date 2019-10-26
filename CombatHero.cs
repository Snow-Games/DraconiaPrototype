using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class CombatHero : BaseClass
{
  public enum Type { Warrior, WhiteMage, Ninja, BlackMage, Dragoon }

  public Type HeroClass;

  public float baseXP;
  public float curXP;

  public int maxLevel;
  public int level;
  public int stamina;
  public int intellect;
  public int speed;
}