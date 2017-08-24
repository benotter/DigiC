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
        avatar.ServerBoltStrike(bolt, GetBodyPart());
		return TakeDamage(bolt.damage);
    }

    public virtual void OnDamage() {}
    public virtual void OnHeal() {}

    public virtual void OnBreak() {}
    public virtual void OnFix () {}

    public bool TakeDamage(int damage)
    {
        if((health -= damage) < 0)
        {
            broken = true;
            this.OnBreak();

            return true;
        }

        this.OnDamage();
        
        return false;
    }

    public bool Heal(int negDamage)
    {
        if((health += negDamage) > 0)
        {
            broken = false;
            this.OnFix();

            return true;
        }

        this.OnHeal();

        return false;
    }
}
