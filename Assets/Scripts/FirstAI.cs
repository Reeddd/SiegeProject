using UnityEngine;
using System.Collections;
using System;

public class FirstAI : MonoBehaviour {

	public class Priority : IComparable 
	{
		public Waypoint wayp;
		public int priority;
	
		public Priority()
		{
			wayp = null;
			priority = 0;
		}
	
		public int CompareTo(object other)
		{
			Priority that = other as Priority;
			return this.priority.CompareTo (that.priority);
		}
	}
	//controller object
	Controller cont;
	//Waypoint you are moving a troop to
	public Waypoint second;
	//Waypoint you are moving a troop from
	public Waypoint first;
	//The current timer, when the time is greater than this, you can move
	private float timer;
	//After a troop moves, you must wait this long before moving again
	private float pause;
	//Array of waypoints known to be blue (set by findBlues())
	public Waypoint[] blues;
	//An arraylist that holds subclasses that keep track of a nodes priority (level two uses this)
	public ArrayList priorities;
	//number of blue nodes controlled
	public int bCount;
	//Mover object
	private Movement mover;
	//how much money you have to buy troops
	private int gold;
	//Cost of a speed troop
	private int speedCost;
	//placeholder?
	private Waypoint blue;
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
		timer = 5f;
		pause = 2.3f;
		mover = (Movement)GameObject.Find ("TeamBlue").GetComponent("Movement");
		blues = new Waypoint[15];
		bCount=0;
		gold = 0;
		speedCost = 25;
		//Repeats the method GimmeMoney which increments the gold variable
		InvokeRepeating("GimmeMoney", 1.5f, 0.2f);
		if(GameObject.Find ("TeamBlue")!=null)
		{	
			blue = (Waypoint)(GameObject.Find("TeamBlue").GetComponent("Waypoint"));
		}
		runOnce = true;
		priorityChanged = true;
	}
	
	void Update ()
	{
		//only runs once to get the priorities once everything is initialized
		if (runOnce)
		{
			findPriorities();
			runOnce = false;
		}
		//If the current time is greater than the timer value (set to Time.time + added time)
		if(Time.time > timer)	
		{
			//Find all waypoints that are blue
			findBlues();
			//Picks the closest blue and moves to it
			levelOne();
			//Picks a blue waypoint to move to based on the priority struct
			levelTwo();
			//Defends a waypoint that's being attacked
			levelThree();

			if(first!=null && second!=null && first.hasTroop ())
			{	
				if(first.checkPCounter(second)<=4)
				{
					//uses the mover class to move a troop from first to second
					mover.moveTroop (false, first, second);
					//Increments the path counter for both waypoints (how many troops are on a path between waypoints)
					first.plusPCounter(second);
					second.plusPCounter (first);
					//first.subtractS();
					//If the waypoint has no more troops, it becomes gray (neutral)
					first.checkIt ();
				}
			}
			reset();
			timer = Time.time + pause;
		}
		if(gold > speedCost)
		{	
			if(GameObject.Find ("TeamBlue")!=null)
			{	
				blue = (Waypoint)(GameObject.Find("TeamBlue").GetComponent("Waypoint"));
				blue.addTroopBlueS ();
				gold-=speedCost;
				speedCost = speedCost+1;
			}
		}
		
	}
	
	public void levelOne()
	{
		bool done = false;
		foreach(Waypoint way in blues)
		{
			if(way != null)
			{
				foreach(Waypoint l in way.getArray ())
				{
					if(l != null)
					{
						if(!l.occupiedBlue)
						{
							first = way;
							second = l;
							done=true;
							break;
						}
					}
				}
			}
			if(done)
				break;
		}
	}

	public void levelTwo()
	{
		if(priorityChanged)
		{
			priorities.Sort();
			priorities.Reverse();
			priorityChanged = false;
		}
		foreach (Priority pri in priorities)
		{
			foreach(Waypoint wayp in pri.wayp.getArray ())
			{
				if(wayp.occupiedBlue)
				{
					first = wayp;
					second = pri.wayp;
					return;
				}
			}
		}
		return;
	}

	public void levelThree()
	{
		//Checking to see if waypoints are being attacked
		foreach(Waypoint way in blues)
		{
			if(way!=null)
			{
				if(way.getUnderFire())
				{
					first = way;
					second = way.getAttackedFrom();
				}
			}
		}
	}



	public void addUnused(GameObject troop)
	{
		mover.addUnused (troop);
	}
	
	public bool hasFirst()
	{
		return first!=null;
	}
	
	public bool hasSecond()
	{
		return second!=null;
	}
	
	public Waypoint getFirst()    {return first;}
	public Waypoint getSecond()    {return second;}
	
	public void findBlues()
	{
		Waypoint[] temp = new Waypoint[20];
		int i=0;
		foreach(object n in cont.getPoints())
		{
			if(n!=null)
			{
				if(((Waypoint) n).occupiedBlue)
				{
					temp[i] = (Waypoint) n;
					i++;
				}
			}
		}
		blues = temp;
		bCount = i;
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
	
	private void findPriorities()
	{
		priorities = new ArrayList();
		Waypoint root = null;
		foreach (Waypoint w in cont.getPoints())
		{
			if(w == null){}
			else if(w.gameObject.name == "TeamBlue")
			{
				root = w;
				break;
			}
		}
		int prior = 1;
		Queue toGo = new Queue();
		toGo.Enqueue (root);
		Priority p = new Priority();
		p.priority = 0;
		p.wayp = root;
		priorities.Add(p);
		bool go;
		while (toGo.Count != 0)
		{
			root = (Waypoint)toGo.Dequeue();
			foreach(Waypoint w in root.getArray ())
			{
				go = true;
				foreach(Priority pr in priorities)
				{
					if(pr.wayp == root)
						prior = pr.priority+1;
					if(pr.wayp == w)
					{
						go = false;
						break;
					}
				}
				if(go)
				{

					toGo.Enqueue(w);
					Priority newP = new Priority();
					newP.priority = prior;
					newP.wayp = w;
					priorities.Add(newP);
				}
			}
		}

	}
}
