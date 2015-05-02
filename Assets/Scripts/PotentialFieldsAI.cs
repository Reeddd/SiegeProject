using UnityEngine;
using System.Collections;
using System;

public class PotentialFieldsAI : Player {
	
	public class Priority : IComparable 
	{
		public Waypoint wayp;
		public int priority;
		public int originalP;
		
		public Priority()
		{
			wayp = null;
			priority = 0;
			originalP = 0;
		}
		
		public int CompareTo(object other)
		{
			Priority that = other as Priority;
			return this.priority.CompareTo (that.priority);
		}
	}
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
		mover = null;
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
			findPriorities();
			runOnce = false;
		}
		//If the current time is greater than the nextMove value (set to Time.time + added time)
		if(Time.time > nextMove)	
		{
			//Find all waypoints that are blue
			findBlues();

			levelFour ();
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
					moveIt ();
					//first.subtractS();
					//If the waypoint has no more troops, it becomes gray (neutral)
					first.checkIt ();
				}
			}
			reset();
			resetTimer ();
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
	
	public void levelTwo()
	{
		int highest = -1;
		if(priorityChanged)
		{
			priorities.Sort();
			priorities.Reverse();
			priorityChanged = false;
		}
		ArrayList choices = new ArrayList();
		ArrayList choicesTwo = new ArrayList ();
		foreach (Priority pri in priorities)
		{
			int current = pri.priority;
			if(highest <= current)
			{
				try
				{
					foreach(Waypoint wayp in pri.wayp.getArray ())
					{
						if(correctColor(wayp)&&wayp.checkRadius()) 
						{
							highest = current;
							choicesTwo.Add(pri.wayp);
							break;
						}
					}
				}
				
				catch(NullReferenceException)
				{
					print("Priority had no wayp");
				}
			}
		}
		second = returnRandom (choicesTwo);
		try
		{
			foreach(Waypoint w in second.getArray ())
			{
				if(correctColor (w))
				{
					choices.Add (w);
				}
			}
		}
		catch(NullReferenceException)
		{}
		first = returnRandom (choices);
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
	
	public void levelFour()
	{
		//This level prepares the AI for the next run through by changing priorities if necessary
		ArrayList priorUp = new ArrayList();
		ArrayList priorDown = new ArrayList();
		foreach(Priority pri in priorities)
		{
			pri.priority=pri.originalP;
		}
		foreach(Priority pri in priorities)
		{
			if(pri.wayp.getCountTotal()>=4)
			{
				foreach(Waypoint w in pri.wayp.getArray())
				{
					foreach(Priority p in priorities)
					{
						if(p.wayp == w && p.priority>pri.priority)
							priorUp.Add(w);
					}
				}
			}
			if((pri.wayp.occupiedRed&&this.color.Equals("Red")||pri.wayp.occupiedBlue&&this.color.Equals("Blue"))&&pri.wayp.getCountTotal()==1)
			{
				priorUp.Add (pri.wayp);
				priorUp.Add (pri.wayp);
			}
			if((pri.wayp.name.Equals ("TeamRed")&&this.color.Equals("Red"))||(pri.wayp.name.Equals ("TeamBlue")&&this.color.Equals ("Blue")))
			{
				priorDown.Add (pri.wayp);
				priorDown.Add (pri.wayp);
				priorDown.Add (pri.wayp);
			}
		}

		if(priorUp.Count > 0)
		{
			foreach(Priority pri in priorities)
			{
				for(int i=0; i < priorUp.Count; i++)
				{
					if(pri.wayp.Equals (priorUp[i]))
					{
						pri.priority+=1;
					}
				}
			}
		}

		if(priorDown.Count>0)
		{
			foreach(Priority pri in priorities)
			{
				for(int i=0; i < priorDown.Count; i++)
				{
					if(pri.wayp.Equals (priorDown[i]))
					{
						pri.priority-=1;
					}
				}
			}
		}
		priorityChanged = true;
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
	
	public void GimmeMoney()
	{
		gold++;
	}
	
	public Waypoint returnRandom(ArrayList waypoints)
	{
		System.Random rnd = new System.Random ();
		int whichOne = 0;
		if(waypoints.Count > 0)
		{
			whichOne = rnd.Next (waypoints.Count);
			return (Waypoint)waypoints[whichOne];
		}
		return null;
		
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
		try
		{
			Movement thisMover = (Movement)(GameObject.Find (getMover ()).GetComponent("Movement"));
		}
		catch(NullReferenceException)
		{}
			foreach (Waypoint w in cont.getPoints())
		{
			if(w!=null && base.mover != null)
			{
				if(w.gameObject.name.Equals ( base.mover.gameObject.name))
				{
					root = w;
					break;
				}
			}
		}
		int prior = 1;
		Queue toGo = new Queue();
		toGo.Enqueue (root);
		Priority p = new Priority();
		p.priority = 0;
		p.originalP = 0;
		p.wayp = root;
		priorities.Add(p);
		bool go;
		while (toGo.Count != 0)
		{
			root = (Waypoint)toGo.Dequeue();
			if(root != null)
			{
				foreach(Waypoint w in root.getArray ())
				{
					go = true;
					foreach(Priority pr in priorities)
					{
						if(pr.wayp == root)
						{
							prior = pr.priority+1;
						}
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
						newP.originalP = prior;
						newP.wayp = w;
						priorities.Add(newP);
					}
				}
			}
		}
	}
}
