using UnityEngine;
using System.Collections;

public class Human : MonoBehaviour {
	
	//This is a test. Repeat. This is a test, of the Emergency Warning System
	public Waypoint second;
	public Waypoint first;
	private Movement mover;
	private float nextMove;
	private float pause;
	public Camera camera;
	// Use this for initialization
	void Start () 
	{

	}
	
	// Update is called once per frame
	void Update () 
		//Gets the first and second nodes for movement
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, 1000 ) && hit.transform.gameObject.CompareTag("Waypoint"))
			{
				if(first==null && hit.transform.gameObject.GetComponent<Waypoint>().occupiedRed)
				{
					first = hit.transform.gameObject.GetComponent<Waypoint>();
				}
			}
		}
		if(!hasSecond () && hasFirst () && Input.GetMouseButtonUp(0))
		{
			Ray ray = camera.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, 500 ) && hit.transform.gameObject.CompareTag("Waypoint") )
			{
				Waypoint[] temp = first.GetComponent<Waypoint>().getArray();
				foreach(Waypoint w in temp)
				{
					if(w == hit.transform.gameObject.GetComponent <Waypoint>())	
					{
						second = hit.transform.gameObject.GetComponent<Waypoint>();
						//GameObject.Find("Main Camera").GetComponent<ButtonDone>().enabled = true;
						moveIt();
						first = null;
						second = null;
					}
				}
				first=null;
				second=null;
			}
			else
			{
				first=null;
				second=null;
			}
			
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
	public void setCamerap(Camera camera)
	{
		this.camera = camera;
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
