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
	Controller cont;
	public Waypoint second;
	public Waypoint first;
	private float timer;
	private float pause;
	public Waypoint[] blues;
	public ArrayList priorities;
	public int bCount;
	private Movement mover;
	private int gold;
	private int speedCost;
	private Waypoint blue;
	public bool runOnce;
	public bool priorityChanged;

	void Start ()
	{
		if(GameObject.Find ("Control")!=null)
		{
			GameObject control = GameObject.Find("Control");
			cont = (Controller)(control.GetComponent("Controller"));
		}
		first = null;
		second = null;
		timer = 5f;
		pause = 2.3f;
		mover = (Movement)GameObject.Find ("TeamBlue").GetComponent("Movement");
		blues = new Waypoint[15];
		bCount=0;
		gold = 0;
		speedCost = 25;
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
		if (runOnce)
		{
			findPriorities();
			runOnce = false;
		}
		if(Time.time > timer)	
		{
			findBlues();
			levelOne();
			levelTwo();
			levelThree();
			if(first!=null && second!=null && first.hasTroop ())
			{	
				if(first.checkPCounter(second)<=4)
				{
					mover.moveTroop (false, first, second);
					first.plusPCounter(second);
					second.plusPCounter (first);
					//first.subtractS();
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
