using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DC_Avatar_Part : MonoBehaviour 
{
    public Color playerColor = Color.blue;
	public Color brokenColor = Color.red * new Color(1f,1f,1f,0.5f);

    [Range(0f,1f)] public float fadeOut = 0f;

    [Space(10)]

    public DC_Avatar avatar;
    public GameObject avatarO;

    private bool _broken = false;
    public bool broken { get { return (health <= 0); } }

    public int health = 50;

    private MeshRenderer rend;

    void Start () 
	{
        avatarO = gameObject;

		rend = GetComponent<MeshRenderer>();
		
		if(!rend)
			rend = GetComponentInChildren<MeshRenderer>();

		UpdateColor();
	}

    public void UpdateColor()
	{
		Color c;

		if(!broken)
			c = playerColor;
		else
			c = brokenColor;

		Color curC = rend.material.color;

        if(fadeOut > 0f)
            c.a = fadeOut;

		if(curC != c)
			rend.material.color = c;
	}

    public void SetHealth(int h)
    {
        health = h;
        CheckHealth();
    }

    public bool CheckHealth() 
    {
        if(!_broken && health <= 0)
            this.OnBreak();
        else if(_broken && health > 0)
            this.OnFix();

        _broken = (health <= 0);

        return (health >= 0);
    }

    public virtual DC_Avatar.BodyParts GetBodyPart()
    {
        return DC_Avatar.BodyParts.None;
    }

    public virtual bool BoltStrike(DC_Bolt bolt) 
    {
        var ret = TakeDamage(bolt.damage);
        avatar.BoltStrike(this, bolt);
        
		return ret;
    }

    public virtual bool ClientBoltStrike(DC_LocalBolt lBolt)
    {
        return TakeDamage(lBolt.damage);
    }

    public virtual void OnServerUpdate() 
    {
		UpdateColor();
    }

    public virtual void OnDamage() 
    {
        UpdateColor();
    }
    public virtual void OnHeal() 
    {
        UpdateColor();
    }

    public virtual void OnBreak() 
    {
        UpdateColor();
    }

    public virtual void OnFix () 
    {
        UpdateColor();
    }

    public bool TakeDamage(int damage)
    {
        health -= damage;
        var res = CheckHealth();
        
        this.OnDamage();
        return res;
    }

    public bool Heal(int negDamage)
    {
        health += negDamage;
        var res = CheckHealth();

        this.OnHeal();
        return res;
    }
}
