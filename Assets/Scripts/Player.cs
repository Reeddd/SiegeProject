using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	//This is a test. Repeat. This is a test, of the Emergency Warning System
	private Movement mover;
	public Camera camera;
	public Waypoint second;
	public Waypoint first;
	private float nextMove;
	private float pause;
	// Use this for initialization
	void Start () 
	{
		mover = null;
	}
	
	// Update is called once per frame
	void Update () 
	//Gets the first and second nodes for movement
	{
		if (mover == null) 
		{
				
		}
  	}
	

	public void moveIt()
	{
		if(nodesReady() && first.GetComponent<Waypoint>().hasTroop() && Time.time > nextMove && first.GetComponent<Waypoint>().occupiedRed && first.GetComponent<Waypoint>().checkRadius ())
		{
			moveTroop();
			nextMove = Time.time + pause;
		}
		if(!first.GetComponent<Waypoint>().hasTroop ())
		{
			renderer.material.color = Color.white;
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
		if(first.checkPCounter(second)<4)
		{
			first.plusPCounter(second);
			second.plusPCounter(first);
			mover.moveTroop (true, first, second);
		}
	}
	
}
