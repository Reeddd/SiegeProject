using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class Waypoint : MonoBehaviour
{
	/** Overview of Waypoint
	 * Waypoint is going to get a LOT more complicated
	 * but for now, it just controls how fast troops can leave,
	 * counts the number of troops inside
	 * and keeps track of what color it is.
	 * 
	 * This script should be on every waypoint. (Remember that the waypoint prefab already has it done for you.)
	 */
	
	private GameObject control;
	private Controller cont;
    public ArrayList waypoints;
	public int[] pathCounts;
	public GameObject[] paths;
    private int count;
	public int wayNumber;
	public int AtroopCount;
	public int DtroopCount;
	public int StroopCount;
	public bool occupiedBlue;
	public bool occupiedRed;
	public int health;
	GameObject textCount;
	private bool underFire;
	private Waypoint attackedFrom;
	
	public void Start ()
    {
		wayNumber = 0;
		if(GameObject.Find ("Control")!=null)	//Don't ask me why this is here as opposed to in the start method, it just works this way and doesn't work that way
		{
			control = GameObject.Find("Control");	
			cont = (Controller)(control.GetComponent("Controller"));
		}
        count = 0;
		AtroopCount = 0;
		DtroopCount = 0;
		StroopCount = 0;
		waypoints = new ArrayList();
		pathCounts = new int[4];
		paths = new GameObject[4];
		occupiedRed = false;
		occupiedBlue = false;
		underFire = false;
		attackedFrom = null;
		if (this.gameObject.transform.childCount > 0) 
		{
			Transform transCount = this.gameObject.transform.Find ("Counting");
			textCount = transCount.gameObject;
		}
	}

	public void Update ()
    {
		if(textCount != null)
		{
			if((AtroopCount+StroopCount+DtroopCount)==0)
				textCount.GetComponent<TextMesh>().text = "";
			else
				textCount.GetComponent<TextMesh>().text = (AtroopCount+StroopCount+DtroopCount).ToString();
		}
	}
	//Unlike the MoveObject version, this only has an array of waypoints it is connected to by a path

	public void AddWaypoint(Waypoint waypoint)
    {
	    if(count<4)
		{
			waypoints.Add(waypoint);
	        count++;
		}
		else
		{
			print ("Can't add anymore waypoints");
		}
    }
	
	public Waypoint[] getArray()
	{
		Waypoint[] temp = (Waypoint[])waypoints.ToArray (typeof(Waypoint));
		return temp;	
	}
	//Tells whether the waypoint has a troop
	public bool hasTroop()
	{
		if(AtroopCount > 0 || DtroopCount > 0 || StroopCount > 0)
		{
			return true;
		}
		return false;
	}
	//Each time the waypoint is clicked, it figures out if the nodes are ready
	//if this is the first node that was clicked, if it has a troop or troops
	//and has a timer. If all conditions are met, it moves out a troop that matches it's color.
	/*Turns the node red or blue*/
	public void turnRed()
	{
		if(this.gameObject.name!="TeamRed" && this.gameObject.name!="TeamBlue")
		{
			renderer.material.color = Color.red;
		}
		else if(this.gameObject.name.Equals("TeamBlue"))
		{
			//end game
		}
		occupiedRed = true;
		occupiedBlue = false;
	}
	
	public void turnBlue()
	{
		if(this.gameObject.name!="TeamRed" && this.gameObject.name!="TeamBlue")
		{
			renderer.material.color = Color.blue;
		}
		occupiedBlue = true;
		occupiedRed = false;
	}
	
	/*The Red Troops*/
	public void addTroopRedA()
	{
		AtroopCount++; turnRed (); occupiedRed=true; recountHealth (); cont.setRecent('A'); 
	}
	public void addTroopRedD()
	{
		DtroopCount++; turnRed (); occupiedRed=true; recountHealth (); cont.setRecent('D'); 
	}
	public void addTroopRedS()
	{
		StroopCount++; turnRed (); occupiedRed=true; recountHealth (); cont.setRecent('S');
	}
	
	/*The blue troops*/
	public void addTroopBlueA()
	{
		AtroopCount++; occupiedBlue=true; turnBlue (); recountHealth ();
	}
	public void addTroopBlueD()
	{
		DtroopCount++; occupiedBlue=true; turnBlue (); recountHealth ();
	}
	public void addTroopBlueS()
	{
		StroopCount++; occupiedBlue=true; turnBlue (); recountHealth();
	}
	
	public void subtractA()
	{
		AtroopCount--;
		recountHealth();
		checkIt ();
	}
	public void subtractD()
	{
		DtroopCount--;
		recountHealth();
		checkIt ();
	}
	public void subtractS()
	{
		StroopCount--;
		recountHealth ();
		checkIt ();
	}
	
	public void checkIt()
	{
		if(!hasTroop ())
		{
			if(this.gameObject.name!="TeamRed" && this.gameObject.name!="TeamBlue")
			{
				renderer.material.color = Color.white;
			}
			health = 0;
			occupiedBlue=false;
			occupiedRed=false;
		}
	}
	// 1 is attack, 2 is Defense and 3 is Speed
	public void recountHealth()
	{
		int newHealth = 0;
		if(occupiedRed)
		{
			newHealth = getCountA()*cont.getRedStatsA ()[0];
			newHealth+= getCountD()*cont.getRedStatsD()[0];
			newHealth+= getCountS()*cont.getRedStatsS()[0];
		}
		else if(!occupiedRed)
		{
			newHealth = getCountA()*cont.getBlueStatsA ()[0];
			newHealth+= getCountD()*cont.getBlueStatsD()[0];
			newHealth+= getCountS()*cont.getBlueStatsS()[0];
		}
		health = newHealth;
	}

	public void takeDamage(int damage)
	{
		health-=damage;
		if(health<=0)
		{
			health=0;
			AtroopCount=0;
			StroopCount=0;
			DtroopCount=0;
			checkIt ();
		}
		if(occupiedRed&&health>0)
		{	int min = 0;
			min = Math.Min (cont.getRedStatsA ()[0], Math.Min(cont.getRedStatsD ()[0], cont.getRedStatsS ()[0]));
			if(health <= ((AtroopCount*cont.getRedStatsA ()[0])+(DtroopCount*cont.getRedStatsD()[0])+(StroopCount*cont.getRedStatsS()[0])-min))
			{	
				if(AtroopCount!=0)
					AtroopCount--;
				else if(DtroopCount!=0)
					DtroopCount--;
				else if(StroopCount!=0)
					StroopCount--;
			}
			checkIt ();
		}
		//When this needs to be changed, change the getBlueHealth method to take parameters for how many troops of each there are
		if(occupiedBlue&&health>0)
		{
			int min = 0;
			min = Math.Min (cont.getBlueStatsA ()[0], Math.Min(cont.getBlueStatsD ()[0], cont.getBlueStatsS ()[0]));
			if(health <= ((AtroopCount*cont.getBlueStatsA()[0])+(DtroopCount*cont.getBlueStatsD ()[0])+(StroopCount*cont.getBlueStatsS ()[0])-min))
			{

				if(AtroopCount!=0)
					AtroopCount--;
				else if(DtroopCount!=0)
					DtroopCount--;
				else if(StroopCount!=0)
					StroopCount--;
			}
		checkIt();
		}
	}
	
	public int getHealth()
	{
		return health;	
	}

	public int checkPCounter(Waypoint wayp)
	{
		int i = 0;
		int j = 0;
		for(i=0; i<4; i++)
		{
			for(j=0; j<4; j++)
			{
				if(paths[i]!=null && wayp.paths[j]!=null)
				{
					if(paths[i].Equals(wayp.paths[j]))
					{	
						return pathCounts[i];
					}
				}
			}
		}
		return 5;
	}

	public void plusPCounter(Waypoint wayp)
	{
		int i = 0;
		int j = 0;
		for(i=0; i<4; i++)
		{
			for(j=0; j<4; j++)
			{
				if(paths[i]!=null && wayp.paths[j]!=null)
				{
					if(paths[i].Equals(wayp.paths[j]))
					{	
						pathCounts[i]++;
						break;
					}
				}
			}
		}
	}

	public void minusPCounter(Waypoint wayp)
	{
		int i = 0;
		int j = 0;
		for(i=0; i<4; i++)
		{
			for(j=0; j<4; j++)
			{
				if(paths[i]!=null && wayp.paths[j]!=null)
				{
					if(paths[i].Equals(wayp.paths[j]))
					{	
						pathCounts[i]--;
						break;
					}
				}
			}
		}
	}

	public void addPath(GameObject pathy)
	{
		bool goodToGo = true;
		int i = 0;
		for(i=0; i<4; i++)
		{
			if(paths[i]!=null)
			{
				if(paths[i]==pathy)
				{
					goodToGo = false;
				}
			}
			else{
				break;
			}
		}
		if(goodToGo)
		{
			paths[i] = pathy;
			pathCounts[i]=0;
		}
	}

	public bool checkRadius()
	{
		Collider[] inSphere = Physics.OverlapSphere(this.collider.bounds.center, 4.2f);
		foreach (Collider cw in inSphere)
		{
			if(cw.gameObject.CompareTag("Troop"))
			{
				Troop tempTroop = (Troop)cw.gameObject.GetComponent ("Troop");
				if(tempTroop.getFirst() == this && ((tempTroop.getColor()=="red" && this.occupiedRed)||(tempTroop.getColor()=="blue" && this.occupiedBlue)))
					return false;
			}
		}
		return true;
	}

	public int getCountA()	{return AtroopCount;}
	public int getCountD()	{return DtroopCount;}
	public int getCountS()	{return StroopCount;}
	public bool getUnderFire() {return underFire;}
	public void setUnderFire(bool fire) {underFire = fire;}
	public Waypoint getAttackedFrom(){return attackedFrom;}
	public void setAttackedFrom(Waypoint from){attackedFrom = from;}
}