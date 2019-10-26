using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BaseClass : MonoBehaviour
{
    public List<BaseAttack> Attacks = new List<BaseAttack>();
    public string name;

    public float baseHP;
    public float baseMP;

    public float curHP;
    public float curMP;

    public float baseAtk;
    public float baseDef;

    public float curAtk;
    public float curDef;

    public float magicDefense;
    public float magicAttack;
}