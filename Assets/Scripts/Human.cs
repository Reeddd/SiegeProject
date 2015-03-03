using UnityEngine;
using System.Collections;

public class Human : Player {
	
	//This is a test. Repeat. This is a test, of the Emergency Warning System
	
	public Camera cam;
	// Use this for initialization
	void Start () 
	{
		first = null;
		second = null;
		nextMove = Time.time;
		pause = 0.7f;
	}
	
	// Update is called once per frame
	void Update () 
		//Gets the first and second nodes for movement
	{
		if(Input.GetMouseButtonDown(0))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;
			if( Physics.Raycast( ray, out hit, 5000 ) && hit.transform.gameObject.CompareTag("Waypoint"))
			{
				if(first==null && correctColor(hit.transform.gameObject.GetComponent<Waypoint>()))
				{
					first = hit.transform.gameObject.GetComponent<Waypoint>();
				}
			}
		}
		if(!hasSecond () && hasFirst () && Input.GetMouseButtonUp(0))
		{
			Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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

	
	public void setCamerap(Camera cam)
	{
		this.cam = cam;
	}
	public void PaddUnused(GameObject troop)
	{
		base.mover.addUnused (troop);
	}

	
}
