using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CurrentWeapon
{
    public static CurrentWeapon Instance;

    public CurrentWeapon(int id, string imgeUrl, string hitUrl, string effectUrl, bool isMelee)
    {
        this.id = id;
        this.imgeUrl = imgeUrl;
        this.hitUrl = hitUrl;
        this.effectUrl = effectUrl;
        this.isMelee = isMelee;
        Instance = this;
    }

    public CurrentWeapon(int id, string name, string imgeUrl, string hitUrl, string effectUrl, bool isMelee)
    {
        this.id = id;
        this.name = name;
        this.imgeUrl = imgeUrl;
        this.hitUrl = hitUrl;
        this.effectUrl = effectUrl;
        this.isMelee = isMelee;
        Instance = this;
    }

    public int id;
    public string name;
    public string imgeUrl;
    public string hitUrl;
    public string effectUrl;
    public bool isMelee;
}

public class WeaponObject
{
    public static WeaponObject Instance;

    public int id;
    public Sprite weaponSprite;
    public Sprite hitSprite;
    public Sprite[] effectSprites;
    public bool isMelee;
}

