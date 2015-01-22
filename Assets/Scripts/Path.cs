using UnityEngine;
using System.Collections;

public class Path : MonoBehaviour
{
    /** Overview
     * This just tells the waypoint that it's connected to another waypoint
     * any waypoint that this path touches will be connected to another waypoint
     * a path can only touch 2 waypoints total.
     * 
     * I'm also using this script as a startup script which determines the players
     * and will give them their initial money and troops and tell which nodes are 
     * the starting points (the starting nodes should be named TeamRed and TeamBlue)
     * Right now I have both teams starting out with 5 troops for testing purposes.
     * 
     * put on paths (path prefab)
     */ 
    Waypoint waypoint0;	//The waypoints are the nodes which the paths touch
    Waypoint waypoint1;
	GameObject control;		//This is the control unit that is invisible but does most of the work
	Controller cont;		//The script that is attached to the control unit
	
	public void Start ()	//Declares the waypoints and gets the mvobj script
    {
	    waypoint0 = null;
		waypoint1 = null;
		if(GameObject.Find ("Control")!=null)
		{
			control = GameObject.Find("Control");
			cont = (Controller)(control.GetComponent("Controller"));
		}
	}
	
	public void Update ()
    {
		
	}

    public void OnTriggerEnter(Collider collision)	//This happens when the game loads
    {
        if (waypoint0 == null || waypoint1 == null)
        {
            if (collision.gameObject.tag == "Waypoint" )	//Basically, if the path object is inside one of the waypoint objects
            {											//both waypoints add the waypoint it is connected to to its array of adjacent nodes
                if (waypoint0 == null)					//In other words, it is so each node knows which other nodes it is connected to.
                {
                    waypoint0 = collision.gameObject.GetComponent<Waypoint>();
                }
                else if (waypoint1 == null)
                {
                    waypoint1 = collision.gameObject.GetComponent<Waypoint>();

					AddToArrays ();
                }
            }
        }
    }

    public void AddToArrays()    //This adds the waypoints to the array of waypoints that each waypoint script has which holds a list of adjacent waypoints
    {
        Waypoint waypointPointer0 = waypoint0.GetComponent<Waypoint>();
        Waypoint waypointPointer1 = waypoint1.GetComponent<Waypoint>();

        waypointPointer0.AddWaypoint(waypoint1);
		waypointPointer0.addPath(this.gameObject);
		cont.checkAdd(waypoint1);
        waypointPointer1.AddWaypoint(waypoint0);
		waypointPointer1.addPath(this.gameObject);
		cont.checkAdd(waypoint0);
    }
	
}