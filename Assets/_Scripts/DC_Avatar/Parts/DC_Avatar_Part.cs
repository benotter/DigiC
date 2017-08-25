using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Part : MonoBehaviour 
{
    public DC_Avatar avatar;
    public bool broken = false;

    public int health = 50;

    public virtual DC_Avatar.BodyParts GetBodyPart()
    {
        return DC_Avatar.BodyParts.None;
    }

    public virtual bool BoltStrike(DC_Bolt bolt) 
    {
        avatar.BoltStrike(this, bolt);
		return TakeDamage(bolt.damage);
    }

    public virtual void OnServerUpdate() {}

    public virtual void OnDamage() {}
    public virtual void OnHeal() {}

    public virtual void OnBreak() {}
    public virtual void OnFix () {}

    public virtual bool CheckHealth() 
    {
        if(health < 0)
        {
            broken = true;
            this.OnBreak();

            return false;
        }
        else
        {
            broken = false;
            this.OnFix();
            return true;
        }
    }

    public bool TakeDamage(int damage)
    {
        health -= damage;
        CheckHealth();

        this.OnDamage();
        return false;
    }

    public bool Heal(int negDamage)
    {
        health += negDamage;
        CheckHealth();

        this.OnHeal();
        return false;
    }
}
