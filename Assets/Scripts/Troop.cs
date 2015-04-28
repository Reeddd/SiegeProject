using UnityEngine;
using System.Collections;

public abstract class Troop : MonoBehaviour 
{
	/**
	 * Overview of TroopR
	 * Interacts with the Move Objects script
	 * It makes sure that a red troop comes out of a red waypoint
	 * and that when a red troop enters a neutral waypoint it becomes a red waypoint
	 * it helps the troop find the waypoint it's going towards and stops it when it gets
	 * there and controls making the troop active or inactive.
	 * It also sends a message to moveTroops when it should be added to the array of unused.
	 * 
	 * This should be placed on every red troop, except really there should only be a prefab, as the game will be instantiating it's own troop objects
	 */ 
	private GameObject control;
	public Controller cont;
	private string color;
	private bool startMove;
	public bool attacking;
	public bool waiting;
	public bool sieging;
	public bool battling;
	private Waypoint first;
	private Waypoint second;
	public Troop otherTroop;
	public Troop potentialTroop;
	string ident = "";
	private bool trackBattle;

	public double timer;
	
	
	public Troop(){}
	
	public Troop( string c)
	{
		color = c;
		if(GameObject.Find ("Control")!=null)	//Don't ask me why this is here as opposed to in the start method, it just works this way and doesn't work that way
		{
			control = GameObject.Find("Control");	
			cont = (Controller)(control.GetComponent("Controller"));
		}
		
		timer = Time.time;
		waiting = false;
		attacking = false;
	}
	
	void Start () 
	{
		if(GameObject.Find ("Control")!=null)	//Don't ask me why this is here as opposed to in the start method, it just works this way and doesn't work that way
		{
			control = GameObject.Find("Control");	
			cont = (Controller)(control.GetComponent("Controller"));
		}

		timer = Time.time;
	}

	public Waypoint getFirst()
	{
		if(first!=null)
			return first;
		else
			return null;
	}
	public Waypoint getSecond()
	{
		if(second!=null)
			return second;
		else
			return null;
	}
	
	void Update () 
	{
		if(attacking)
		{
			if(potentialTroop==null  || otherTroop==null)
			{
				setAll (false, false, false);
				if(getTracking())
				{
					GameObject tester = GameObject.Find("Tester");
					TestAIs aiTest = (TestAIs)tester.GetComponent("TestAIs");
					aiTest.troopWin(getColor ());
					setTracking(false);
				}
			}
			else
				this.attackTroop(otherTroop);
		}
		else if(waiting)
		{
			if(potentialTroop==null  || otherTroop==null)
				setAll (false, false, false);
			else if(!otherTroop.battling && !otherTroop.sieging && !otherTroop.waiting)
				setAll (false, false, false);
		}
		else if(startMove)	//Start move is called below in the startM() method
		{
			if(first != null && second != null)
			{	
				if(this.gameObject.GetComponent<Speed>() as Speed !=null)
					ident = "S";
				else if(this.gameObject.GetComponent<Attack>() as Attack !=null)
					ident = "A";
				else if(this.gameObject.GetComponent<Defense>() as Defense !=null)
					ident = "D";
				float dist = Vector3.Distance(second.collider.bounds.center, transform.position);
				if(dist > (second.gameObject.renderer.bounds.size[0]/2)+(this.gameObject.renderer.bounds.size[0]*1.4f))
				{		
					//Makes the speed of movement equal the speed of movement in the stats arrays - RED
					if(color.Equals ("red"))
					{
						transform.LookAt(second.collider.bounds.center);	//Find the node we are moving to
						if(ident == "S")
							transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getRedStatsS()[2]*(.01f));	//Move towards that node
						else if(ident == "A")
							transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getRedStatsA()[2]*(.01f));
						else if(ident == "D")
							transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getRedStatsD()[2]*(.01f));
					}
					//Makes the speed of movement equal the speed of movement in the stats arrays - BLUE
					else
					{
						transform.LookAt(second.collider.bounds.center);	//Find the node we are moving to
						if(ident == "S")
							transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getBlueStatsS()[2]*(.01f));	//Move towards that node
						else if(ident == "A")
							transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getBlueStatsA()[2]*(.01f));
						else if(ident == "D")
							transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getBlueStatsD()[2]*(.01f));
					}
				}
				else
				{
					attackWaypoint ();
				}
			}
		}
	}
	
	//This will be code to make battles possible
	
	public abstract void takeDamage(int dmg);
	
	public void endMove()
	{
		startMove = false;
	}
	
	public void restart()
	{
		startMove = true;
	}
	
	public void deactivate()
	{
		//turn invisible
		this.gameObject.SetActive (false);
		renderer.enabled = false;
		collider.enabled = false;
	}

	public void reactivate()
	{
		renderer.enabled = true;
		collider.enabled = true;
		this.gameObject.SetActive (true);
	}

	public void startM(bool red)
	{
		if(GameObject.Find ("Control")!=null)	//Don't ask me why this is here as opposed to in the start method, it just works this way and doesn't work that way
		{
			control = GameObject.Find("Control");	
			cont = (Controller)(control.GetComponent("Controller"));
		}
		first = null;
		second = null;
		//This figures out which node the troop should be moving to or from and then makes it visible and starts it moving
		if(red)
		{
				first = cont.RGetFirst();
				second = cont.RGetSecond();
				reactivate();
				renderer.material.SetColor("_Color", Color.red);
				color = "red";
				startMove = true;
		}
		else
		{
				first = cont.BGetFirst();
				second = cont.BGetSecond();
				reactivate ();
				renderer.material.SetColor("_Color", Color.blue);
				color = "blue";
				startMove = true;
		}
	}
	
	public void attackWaypoint()
	{
		if((color.Equals ("red")&&second.GetComponent<Waypoint>().occupiedBlue)||color.Equals ("blue")&&second.GetComponent<Waypoint>().occupiedRed)
		{
			if(!sieging)
				setAll (false, false, true);
			otherTroop = null;
			if(Time.time > timer)
			{	
				siege ();
				second.GetComponent<Waypoint>().setUnderFire(true);
				second.GetComponent<Waypoint>().setAttackedFrom(first);
			}
		}
		else
		{
			second.GetComponent<Waypoint>().setUnderFire(false);
			second.GetComponent<Waypoint>().setAttackedFrom(null);
			//restart ();
			setAll (false, false, false);
			float dist = Vector3.Distance(second.collider.bounds.center, transform.position);
			transform.LookAt(second.collider.bounds.center);	//Find the node we are moving to
	   		//Makes the speed of movement equal the speed of movement in the stats arrays - RED
			if(color.Equals ("red"))
			{
				transform.LookAt(second.collider.bounds.center);	//Find the node we are moving to
				if(ident == "S")
					transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getRedStatsS()[2]*(.01f));	//Move towards that node
				else if(ident == "A")
					transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getRedStatsA()[2]*(.01f));
				else if(ident == "D")
					transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getRedStatsD()[2]*(.01f));
			}
			//Makes the speed of movement equal the speed of movement in the stats arrays - BLUE
			else
			{
				transform.LookAt(second.collider.bounds.center);	//Find the node we are moving to
				if(ident == "S")
					transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getBlueStatsS()[2]*(.01f));	//Move towards that node
				else if(ident == "A")	
					transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getBlueStatsA()[2]*(.01f));
				else if(ident == "D")
					transform.position= Vector3.MoveTowards(transform.position, second.collider.bounds.center, cont.getBlueStatsD()[2]*(.01f));
			}
			
			if(dist < (second.gameObject.renderer.bounds.size[0] / 1.8f))		//When we get to that node
			{	
				deactivate ();
				setAll (false, false, false);
				endMove ();//Make it invisible and turn off the collider so it doesn't run into other troops
				getFirst().minusPCounter(getSecond ());
				getSecond().minusPCounter(getFirst ());
				if(color.Equals ("red"))
				{
					if(ident == "S")
					{
						second.GetComponent<Waypoint>().addTroopRedS();
						setHealth (cont.getRedStatsS()[0]);
					}
					else if(ident == "A")
					{
						second.GetComponent<Waypoint>().addTroopRedA();
							setHealth (cont.getRedStatsA()[0]);
					}
					else if(ident == "D")
					{
						second.GetComponent<Waypoint>().addTroopRedD();
						setHealth (cont.getRedStatsD()[0]);
					}
					//second.GetComponent<Waypoint>().turnRed();							
					cont.RAddUnused(this.gameObject);	//Put the object in the unused array
				}
				else if(color.Equals("blue"))
				{
					if(ident == "S")
					{
						second.GetComponent<Waypoint>().addTroopBlueS();
						setHealth (cont.getBlueStatsS()[0]);
					}
					else if(ident == "A")
					{
						second.GetComponent<Waypoint>().addTroopBlueA();
						setHealth (cont.getBlueStatsA()[0]);
					}
					else if(ident == "D")
					{
						second.GetComponent<Waypoint>().addTroopBlueD();
						setHealth (cont.getBlueStatsD()[0]);
					}
					//second.GetComponent<Waypoint>().turnBlue();							
					cont.BAddUnused(this.gameObject);
				}
			}
		}	
	}
	
	public void battle(GameObject troop)
	{
		potentialTroop = troop.GetComponent<Troop>();
		warLogic(potentialTroop);
		if(attacking || waiting)
		{
			battling = true;
		}
	}
	
	public void setAll(bool wait, bool attack, bool siege)
	{
		waiting = wait;
		attacking = attack;
		sieging = siege;
		if(wait || attack)
		{
			if((Vector3.Distance(first.collider.bounds.center, transform.position) - 
				Vector3.Distance(second.collider.bounds.center, transform.position))>=0)
			{
		    	transform.position = Vector3.MoveTowards(transform.position, first.collider.bounds.center, .5f);
			}
			else
				transform.position = Vector3.MoveTowards(transform.position, first.collider.bounds.center, .2f);
		}
		if(siege)
		{}
		if(!wait && !attack && !siege)
		{
			battling=false;
			restart();
		}
	}
	
	public void warLogic(Troop troop)
	{
		//Runs into enemy
		if(troop.getColor() != this.getColor())
		{
			if(attacking)
			{}
			else if(waiting)
			{
				//turn around
				otherTroop = troop;
				if(otherTroop!=null)
				{
					setAll (false, true, false);
					endMove ();
				}
				else
				{
					setAll (false, false, false);
				}
			}
			else if(sieging)
			{
				//if one way
					//turn around
				//if the other way
				otherTroop = troop;
				if(otherTroop!=null)
				{
					setAll (false, true, false);
					endMove ();
				}
				else
				{
					setAll (false, false, false);
				}
			}
			else
			{
				otherTroop = troop;
				if(otherTroop!=null)
				{
					setAll (false, true, false);
					endMove ();
				}
				else
				{
					setAll (false, false, false);
				}	
			}
		}
		//Runs into ally
		else
		{
			if(attacking || waiting || sieging)
			{}
			else
			{
				otherTroop = troop;
				if(otherTroop!=null && (otherTroop.getAttacking() || otherTroop.getSieging() || otherTroop.getWaiting()))
				{
					setAll (true, false, false);
					endMove ();
				}
				else
				{
					//restart ();
					setAll (false, false, false);
				}
			}
		}
	}
	public abstract void siege();
	public abstract void attackTroop(Troop troop);
	public abstract int getHealth();
	public abstract void setHealth(int h);
	public abstract float getAttackRate();
	public abstract void setAttackRate(int ar);
	public abstract int getDamage();
	public abstract void setDamage(int d);
	public abstract int getSpeed();
	public abstract void setSpeed(int s);
	public void setColor(string c){color = c;}
	public string getColor(){return color;}
	public bool getWaiting(){return (waiting);}
	public bool getSieging(){return sieging;}
	public bool getAttacking(){return attacking;}
	public bool getTracking(){return trackBattle;}
	public void setTracking(bool track){trackBattle = track;}
}