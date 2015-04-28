using UnityEngine;
using System.Collections;
using System;

public class BaseAI : Player {

	//controller object
	Controller cont;
	//Array of waypoints known to be blue (set by findBlues())
	public ArrayList blues;
	//An arraylist that holds subclasses that keep track of a nodes priority (level two uses this)
	public ArrayList priorities;
	//number of blue nodes controlled
	public int bCount;
	//how much money you have to buy troops
	private int gold;
	//Cost of a speed troop
	private int speedCost;
	//placeholder?
	private Waypoint homeBase;
	public bool runOnce;
	//boolean to see if the priority of nodes have changed
	public bool priorityChanged;
	
	void Start ()
	{
		//This finds the controller object which holds information about the game, statistics for troops, and the human and AI classes
		if(GameObject.Find ("Control")!=null)
		{
			GameObject control = GameObject.Find("Control");
			cont = (Controller)(control.GetComponent("Controller"));
		}
		//Initial values
		first = null;
		second = null;
		nextMove = 5f;
		pause = 2.3f;
		//mover = null;
		blues = new ArrayList();
		bCount=0;
		gold = 0;
		speedCost = 25;
		//Repeats the method GimmeMoney which increments the gold variable
		InvokeRepeating("GimmeMoney", 1.5f, 0.2f);
		homeBase = null;
		runOnce = true;
		priorityChanged = true;
	}
	
	void Update ()
	{
		//only runs once to get the priorities once everything is initialized
		if (runOnce)
		{
			runOnce = false;
		}
		//If the current time is greater than the nextMove value (set to Time.time + added time)
		if(Time.time > nextMove)	
		{
			//Find all waypoints that are blue
			findBlues();
			//Picks the closest blue and moves to it
			levelOne();
			if(first!=null && second!=null && first.hasTroop ())
			{	
				if(first.checkPCounter(second)<=4)
				{
					//uses the mover class to move a troop from first to second
					moveIt ();
					//first.subtractS();
					//If the waypoint has no more troops, it becomes gray (neutral)
					first.checkIt ();
				}
			}
			reset();
			nextMove = Time.time + pause;
		}
		if(gold > speedCost)
		{	
			if(base.mover.gameObject.name.Equals ("TeamRed"))
			{
				if(GameObject.Find ("TeamRed")!=null)
				{	
					homeBase = (Waypoint)(GameObject.Find("TeamRed").GetComponent("Waypoint"));
					homeBase.addTroopRedS ();
					gold-=speedCost;
					speedCost = speedCost+1;
				}
			}
			else if(base.mover.gameObject.name.Equals ("TeamBlue"))
			{
				if(GameObject.Find ("TeamBlue")!=null)
				{	
					homeBase = (Waypoint)(GameObject.Find("TeamBlue").GetComponent("Waypoint"));
					homeBase.addTroopBlueS ();
					gold-=speedCost;
					speedCost = speedCost+1;
				}
			}
		}
		
	}
	
	public void levelOne()
	{
		ArrayList potentialTwo = new ArrayList();
		if(blues.Count>0)
		{
			first = returnRandom (blues);
			foreach(Waypoint attached in first.getArray ())
			{
				if(attached != null)
				{
					if(!correctColor (attached))
					{
						potentialTwo.Add(attached);
					}
				}
			}
			second = returnRandom (potentialTwo);
		}
	}
	
	public void findBlues()
	{
		ArrayList temp = new ArrayList();
		int i=0;
		foreach(object n in cont.getPoints())
		{
			if(n!=null)
			{
				if(correctColor(((Waypoint) n)))
				{
					temp.Add((Waypoint) n);
					i++;
				}
			}
		}
		blues = temp;
		bCount = i;
	}

	public override int numberBought()
	{
		return speedCost - 25;
	}
	
	public override int numberLost()
	{
		findBlues ();
		int stock = 0;
		foreach(Waypoint bl in blues)
		{
			stock += bl.getCountTotal();
		}
		return numberBought() - stock;
	}

	public Waypoint returnRandom(ArrayList waypoints)
	{
		System.Random rnd = new System.Random ();
		int whichOne = 0;
		if(waypoints.Count > 0)
		{
			whichOne = rnd.Next (waypoints.Count-1);
			return (Waypoint)waypoints[whichOne];
		}
		return null;

	}

	public void GimmeMoney()
	{
		gold++;
	}
	
	public void reset()
	{
		first=null;
		second=null;
	}

}
