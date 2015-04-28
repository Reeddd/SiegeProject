using UnityEngine;
using System.Collections;

public class Speed : Troop
{
	
	public int health;
	public float attackRate{ get; set;}
	public int damage{ get; set;}
	public int speed{ get; set;}
	public char type{get; set;}

	public void Start()
	{
		type = 'S';
		health = 300;
		attackRate = .8f;
		damage = 30;
		speed = 2;
		timer = Time.time;
		if(base.getColor().Equals("red"))
			setSpecs (base.cont.getRedStatsS());
		else
			setSpecs (base.cont.getBlueStatsS());
	}
	
	public void setSpecs(int[] specs)
	{
		health = specs [0];
		damage = specs [1];
		speed = specs [2];
		type = 'S';
	}
	
	
	public override void siege()
	{
		getSecond().takeDamage (damage);
		timer=Time.time + attackRate;
		
	}
	
	public override void attackTroop(Troop troop)
	{
		if(troop != null)
		{
			if(timer < Time.time)
			{
				if(troop.getHealth () == this.getHealth())
				{
					setTracking(true);				
			    }
				troop.takeDamage (damage);
				timer=Time.time + attackRate;
			}
		}
		else
		{
			setAll (false, false, false);
			restart ();
		}
	}
	
	public override void takeDamage(int dmg)
	{
		health -= dmg;
		if(health <= 0)
		{
			base.getFirst().minusPCounter(base.getSecond ());
			base.getSecond().minusPCounter(base.getFirst ());
			Destroy(gameObject);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag=="Troop")
		{
			Troop troop = other.gameObject.GetComponent<Troop>();
			if((troop.getSecond() == base.getFirst() && troop.getFirst () == base.getSecond ()) || (troop.getSecond () == base.getSecond () && troop.getFirst() == base.getFirst ()))
			{
				base.battle(other.gameObject);
			}
		}
	}

	public override int getHealth(){return health;}
	public override void setHealth(int h){health = h;}
	public override float getAttackRate(){return attackRate;}
	public override void setAttackRate(int ar){attackRate = ar;}
	public override int getDamage(){return damage;}
	public override void setDamage(int d){damage = d;}
	public override int getSpeed(){return speed;}
	public override void setSpeed(int s){speed = s;}
	
}
