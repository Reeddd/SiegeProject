using UnityEngine;
using System.Collections;
using System;

public abstract class Player : MonoBehaviour {

	//This is a test. Repeat. This is a test, of the Emergency Warning System
	protected Movement mover;
	protected string color;
	protected Waypoint second;
	protected Waypoint first;
	protected float nextMove;
	protected float pause;
	
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
	//Gets the first and second nodes for movement
	{

  	}

	public void resetTimer()
	{
		nextMove = Time.time + pause;
	}

	public void moveIt()
	{
		if(nodesReady() && first.GetComponent<Waypoint>().hasTroop() && Time.time > nextMove && correctColor(first.GetComponent<Waypoint>()) && first.GetComponent<Waypoint>().checkRadius ())
		{
			moveTroop();
			nextMove = Time.time + pause;
		}
		if(!first.GetComponent<Waypoint>().hasTroop ())
		{
			first.renderer.material.color = Color.white;
		}
	}
	

	public void resetN()
	{
		first = null;
		second = null;
	}
	
	public bool nodesReady()
	{
		if(hasFirst () && hasSecond ()) 
		{return true;}
		
		else  
		{return false;}
	}

	public void addUnused(GameObject troop)
	{
		mover.addUnused (troop);
	}

	public bool hasFirst()    {return first!=null;}
	public bool hasSecond()    {return second!=null;}
	public Waypoint getFirst()    {return first;}
	public Waypoint getSecond()    {return second;}
	
	public void moveTroop()
	{
		if(first.checkPCounter(second)<4 && mover!=null)
		{
			first.plusPCounter(second);
			second.plusPCounter(first);
			mover.moveTroop (moveHelp (), first, second);
		}

	}
	public void setMover(string teamColor)
	{
		mover = (Movement)(GameObject.Find (teamColor).GetComponent("Movement"));
		if (mover.gameObject.name.Equals ("TeamRed"))
			color = "Red";
		else if (mover.gameObject.name.Equals ("TeamBlue"))
			color = "Blue";

	}

	protected bool correctColor(Waypoint wayp)
	{
		if(wayp.occupiedRed && color.Equals ("Red"))
			return true;
		else if(wayp.occupiedBlue && color.Equals ("Blue"))
			return true;
		else
			return false;
	}

	protected bool moveHelp()
	{
		if(color.Equals("Red")){return true;}
		else if(color.Equals("Blue")){return false;}
		else {return false;}
	}

	public abstract int numberBought ();
	
	public abstract int numberLost ();

	public string getMover()
	{
		try
		{
			return mover.gameObject.name;
		}
		catch(NullReferenceException)
		{
			if(GameObject.Find ("Control")!=null)	//Don't ask me why this is here as opposed to in the start method, it just works this way and doesn't work that way
			{
				GameObject control = GameObject.Find("Control");	
				Controller cont = (Controller)(control.GetComponent("Controller"));
				cont.setMovers();
			}
		}
		return null;
	}

}
