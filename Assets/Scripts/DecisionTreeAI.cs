using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class DecisionTreeAI : Player 
{

	private const int weightOfBases = 15;
	private const int weightOfWaypoint = 1;
	private const int weightOfProximity = 1;
	private const int minimumTreeDepth = 3;
	private const bool predictEnemy = false;

	private Waypoint dtBase;
	private Waypoint enemyBase;

	private bool initialized;

	//array defining which waypoints are adjacent to which others
	bool[,] adjacencies;

	//distances of each corresponding waypoint in waypoints from the home base
	int[] WPDistance;

	int maxWPDistance;

	//controller object
	Controller cont;

	//waypoints in an array
	Waypoint[] waypoints;

	//color of the DT instance. true=red, false=blue
	bool binColor;

	//current cost of a speed troop
	int speedCost;

	//current gold
	int gold;

	//current number of troops
	int troops;

	/**
	 * Used in decisiontree-making. 
	 */
	public class GameState
	{
		//index indicates waypoint the troops are at. 
		public Queue[] tq;

		//keeps track of which waypoints are occupied
		//0 = none, 1 = red, 2 = blue
		public int[] occupied; 

		//children of the state that are possible by making moves
		public LinkedList<GameState> children; 

		//waypoints which together represent an action taken to get from the parent 
		//gamestate to this gamestate
		public int fromWP;
		public int toWP;

		/**
		 * Makes a root based on the state of the game.
		 */
		public GameState(Waypoint[] wps)
		{
			tq = new Queue[wps.Length];
			occupied = new int[wps.Length];
			for(int i = 0; i < wps.Length; i++)
			{
				if(wps[i] == null) 
				{
					break;
				}
				tq[i] = (Queue)wps[i].troopQ.Clone();
				if(wps[i].occupiedRed)
				{
					occupied[i] = 1;
				}
				else if(wps[i].occupiedBlue)
				{
					occupied[i] = 2;
				}
				else // occupied by none
				{
					occupied[i] = 0;
				}
			}
			children = new LinkedList<GameState>();
			fromWP = toWP = -1;
		}

		/**
		 * Compares two gamestates for equality of the state they represent. Does not
		 * take children into account.
		 */
		public bool equals(GameState other)
		{
			for(int i = 0; i < tq.Length; i++)
			{
				if(tq[i] == null) 
				{
					break;
				}
				if(tq[i].Count != other.tq[i].Count || occupied[i] != other.occupied[i])
				{
					return false;
				}
			}
			return true;
		}

		/**
		 * Close to a null constructor.
		 */
		private GameState(int numWaypoints)
		{
			children = new LinkedList<GameState>();
			fromWP = toWP = -1;
			tq = new Queue[numWaypoints]; 
			occupied = new int[numWaypoints];
		}

		/**
		 * Returns a copy of the gamestate but without children
		 */
		public GameState stateOnlyCopy()
		{
			GameState gs = new GameState(tq.Length);
			for(int i = 0; i < tq.Length; i++)
			{
				if(tq[i] == null)
				{
					break;
				}
				gs.tq[i] = (Queue)tq[i].Clone();
				Array.Copy(occupied, gs.occupied, occupied.Length);
			}
			return gs;
		}

		/**
		 * Adds a child to the gamestate
		 */
		public void addChild(GameState child)
		{
			children.AddLast(child);
		}

		/**
		 * Returns true if the gamestate has no children 
		 */
		public bool isLeaf()
		{
			return children.Count == 0;
		}

		/**
		 * Grows the tree one level, which means one player turn and one enemy turn.
		 */
		public void growOneLevel(Waypoint[] wps, bool binColor, bool[,] adjacencies)
		{
			//for every leaf
			if(!isLeaf())
			{
				for(LinkedListNode<GameState> child = children.First; child != null; child = child.Next)
				{
					child.Value.growOneLevel(wps, binColor, adjacencies);
				}
			}
			else //if root is a leaf
			{
				//add based on player's moved
				addAllChildren(true, wps, binColor, adjacencies);

				if(predictEnemy)
				{
					//add based on enemy's response to player's moves
					LinkedListNode<GameState> temp = children.First;
					while(temp != null)
					{
						temp.Value.addAllChildren(false, wps, binColor, adjacencies); 
						temp = temp.Next;
					}
				}
			}
		}

		/**
		 * Adds all possible children to the current node in the tree. That noode must be a leaf.
		 */
		public void addAllChildren(bool isDTturn, Waypoint[] wps, bool binColor, bool[,] adjacencies)
		{
			//currentColor is the color of the player that is moving. 
			bool currentColor = (isDTturn == binColor);
			//for every node
			GameState temp;
			for(int wpIndex = 0; wpIndex < wps.Length; wpIndex++)
			{
				if(wps[wpIndex] == null)
				{
					break;
				}
				if(currentColor ? (occupied[wpIndex] == 1) : (occupied[wpIndex] == 2))
				{
					//for every path from the current node
					for(int pathIndex = 0; pathIndex < wps.Length; pathIndex++)
					{
						if(wps[pathIndex] == null)
						{
							break;
						}
						if(!adjacencies[wpIndex,pathIndex])
						{
							continue;
						}
						//make a state for the troop that would be moved along the path
						temp = stateOnlyCopy();
						//if target waypoint is empty or occupied by current color
						if(currentColor ? temp.occupied[pathIndex] != 2 : temp.occupied[pathIndex] != 1)
						{
							temp.tq[pathIndex].Enqueue(temp.tq[wpIndex].Dequeue());
							temp.occupied[pathIndex] = currentColor ? 1 : 2;
						}
						//else if target waypoint is occupied by opponent
						else
						{
							temp.tq[wpIndex].Dequeue();
							temp.tq[pathIndex].Dequeue();
							if(temp.tq[pathIndex].Count == 0)
							{
								temp.occupied[pathIndex] = 0;
							}
						}
						if(temp.tq[wpIndex].Count == 0)
						{
							temp.occupied[wpIndex] = 0;
						}
						temp.fromWP = wpIndex;
						temp.toWP = pathIndex;
						//add that child to the root's children
						addChild(temp);
					}
				}
			}
		}
	}

	/**
	 * Returns the index of a waypoint in waypoints
	 */
	private int indexOfWaypoint(Waypoint w)
	{
		for(int i = 0; i < waypoints.Length; i++)
		{
			if(w.Equals(waypoints[i]))
			{
				return i;
			}
		}
		return -1;
	}

	/**
	 * Returns the value of a leaf. This is determined by:
	 * -the number of troops at each waypoint.
	 * -the proximity of those waypoints to the enemy's base
	 * -the number of waypoints occupiied
	 */
	public int evaluateLeaf(GameState leaf)
	{
		int sum = 0;
		for(int i = 0; i < leaf.occupied.Length; i++)
		{
			if(waypoints[i] == null) 
			{
				break;
			}
			//if waypoint is occupied by a DT troop
			if(binColor ? leaf.occupied[i] == 1 : leaf.occupied[i] == 2)
			{
				//add to state's value the product of the number of troops 
				//and the distance of the troop from the homebase
				//constant adds value to holding a waypoint
				sum += (leaf.tq[i].Count * WPDistance[i]*weightOfProximity + weightOfWaypoint);
				//if DT occupies enemy base
				if(waypoints[i] == enemyBase)
				{
					sum += weightOfBases;
				}
			}
			else if(leaf.occupied[i] != 0) //occupied by DT's enemy
			{
				sum -= (leaf.tq[i].Count * (maxWPDistance-WPDistance[i])*weightOfProximity + weightOfWaypoint);
				//if enemy occupies dtbase
				if(waypoints[i] == dtBase)
				{
					sum -= weightOfBases;
				}
			}

		}
		return sum;
	}

	/**
	 * Evaluates the state node of the tree.
	 */
	public double evaluate(GameState state)
	{
		if(state.isLeaf())
		{
			return (double)evaluateLeaf(state);
		}
		double sum = 0;
		LinkedListNode<GameState> current = state.children.First;
		while(current != null)
		{
			sum += evaluate(current.Value);
			current = current.Next;
		}
		return sum / state.children.Count;
	}

	/** 
	 * Returns the gamestate that represents the best choice from the root.
	 */
	public GameState getChoice(GameState root)
	{
		double choiceValue = Double.NegativeInfinity;
		double tempValue;
		LinkedListNode<GameState> current = root.children.First;
		GameState choice = current.Value;
		while(current != null)
		{
			tempValue = evaluate(current.Value);
			if(tempValue > choiceValue)
			{
				choice = current.Value;
				choiceValue = tempValue;
			}
			current = current.Next;
		}
		return choice;
	}

	/**
	 * Called upon start of game.
	 */
	void Start()
	{
		initialized = false;
	}

	/**
	 * Initializes everything needed.
	 */
	void init()
	{
		nextMove = 5f;
		pause = 2.3f;
		InvokeRepeating("GimmeMoney", 1.5f, 0.2f);
		troops = 0;
		if(GameObject.Find ("Control")!=null)
		{
			GameObject control = GameObject.Find("Control");
			cont = (Controller)(control.GetComponent("Controller"));
		}
		waypoints = cont.getPoints();
		speedCost = 25;
		gold = 0;
		WPDistance = new int[waypoints.Length];
		adjacencies = new bool[waypoints.Length, waypoints.Length];
		Queue<Waypoint> queue = new Queue<Waypoint> ();
		bool[] added = new bool[waypoints.Length];
		Waypoint wp;
		if(base.mover.gameObject.name.Equals("TeamRed"))
		{
			wp = (Waypoint)(GameObject.Find("TeamRed").GetComponent("Waypoint"));
			binColor = true;
		}
		else
		{
			wp = (Waypoint)(GameObject.Find("TeamBlue").GetComponent("Waypoint"));
			binColor = false;
		}

		dtBase = wp;
		enemyBase = (Waypoint)(GameObject.Find(binColor ? "TeamBlue" : "TeamRed").GetComponent("Waypoint"));
		queue.Enqueue(wp);
		int thisIndex = indexOfWaypoint(wp);
		added[thisIndex] = true;
		WPDistance[thisIndex] = 0;
		Waypoint[] adjacentPoints;
		int thatIndex;
		while(queue.Count > 0)
		{
			wp = queue.Dequeue();
			thisIndex = indexOfWaypoint(wp);
			adjacentPoints = wp.getArray(); 
			for(int i = 0; i < adjacentPoints.Length; i++)
			{
				thatIndex = indexOfWaypoint(adjacentPoints[i]);
				adjacencies[thisIndex, thatIndex] = true;
				if(!added[thatIndex])
				{
					WPDistance[thatIndex] = WPDistance[thisIndex]+1;
					added[thatIndex] = true;
					queue.Enqueue(adjacentPoints[i]);
				}
			}
		}
		maxWPDistance = 0;
		for(int i = 0; i < WPDistance.Length; i++){
			if(WPDistance[i] > maxWPDistance)
			{
				maxWPDistance = WPDistance[i];
			}
		}
	}

	/**
	 * Buys one speed troop, reduces gold accordingly, and increments the speed cost.
	 */
	void buySpeedTroop()
	{
		String baseString = binColor ? "TeamRed" : "TeamBlue";
		if(binColor)
		{
			if(GameObject.Find ("TeamRed")!=null)
			{	
				((Waypoint)(GameObject.Find("TeamRed").GetComponent("Waypoint"))).addTroopRedS();
				gold-=speedCost;
				speedCost++;
			}
		}
		else 
		{
			if(GameObject.Find ("TeamBlue")!=null)
			{	
				((Waypoint)(GameObject.Find("TeamBlue").GetComponent("Waypoint"))).addTroopBlueS();
				gold-=speedCost;
				speedCost++;
			}
		}
		troops++;
	}

	/**
	 * Makes a new decision or considers the current one more deeply.
	 */
	void Update ()
	{
		if(!initialized)
		{
			init();
			initialized = true;
		}
		GameState root = new GameState(waypoints);
		GameState decision = null;
		//if $ buy troop
		if(gold > speedCost)
			buySpeedTroop();

		//if root is different from game state, reset tree to new root.
		GameState current = new GameState(waypoints);
		if(!root.equals(current))
		{
			root = current;
		}
		//grow tree to minimum height/add one layer.
		if (troops > 0) //TODO loss of troops reflected here?
		{
			for(int i = 0; i < minimumTreeDepth; i++)
			{
				root.growOneLevel(waypoints, binColor, adjacencies);
			}
		}
		//evaluate tree, log decision
		if(!root.isLeaf())
		{
			decision = getChoice(root);
			first = waypoints[decision.fromWP];
			second = waypoints[decision.toWP];
		}
		//if time up, execute & reset decision, reset tree to root
		//If the current time is greater than the nextMove value (set to Time.time + added time)
		if(Time.time > nextMove)	
		{
			//Defends a waypoint that's being attacked
			defend();
		
			if(first != null && second != null && first.hasTroop())
			{	
				if(first.checkPCounter(second)<=4) 
				{
					//uses the mover class to move a troop from first to second
					mover.moveTroop(moveHelp(), first, second);
					//Increments the path counter for both waypoints (how many troops are on a path between waypoints)
					first.plusPCounter(second);
					second.plusPCounter (first);
					//first.subtractS();
					//If the waypoint has no more troops, it becomes gray (neutral)
					first.checkIt ();
				}
			}
			nextMove = Time.time + pause;
		}
	}
	/**
	 * Predicts which troop will win. Takes the types of both troops.
	 * Returns 1 if t1 will win, 2 if t2 will win, and 0 if the proc determines the win.
	 * For future features in which troops may not have the same stats across colors, 
	 * the color of the troop is needed.
	 */
	public int predictWinner(char color1, char t1, char color2, char t2)
	{
		if(!verifyIn(color1, new []{'R', 'B'}))
		{
			print("Invalid value passed to predictWinner: color1 = " + color1);
		}
		if(!verifyIn(color2, new []{'R', 'B'}))
		{
			print("Invalid value passed to predictWinner: color2 = " + color2);
		}
		if(!verifyIn(t1, new []{'A', 'D', 'S'}))
		{
			print("Invalid value passed to predictWinner: t1 = " + t1);
		}
		if(!verifyIn(t2, new []{'A', 'D', 'S'}))
		{
			print("Invalid value passed to predictWinner: t2 = " + t2);
		}
		int t1Health, t1Attack, t2Health, t2Attack;
		int[] temp;
		if(t1 == 'A')
		{
			temp = color1 == 'R' ? cont.getRedStatsA() : cont.getBlueStatsA();
		}
	    else if(t1 == 'D')
		{
			temp = color1 == 'R' ? cont.getRedStatsD() : cont.getBlueStatsD();
		}
		else// if(t1 == 'S')
		{
			temp = color1 == 'R' ? cont.getRedStatsS() : cont.getBlueStatsS();
		}
		t1Health = temp[0];
		t1Attack = temp[1];
		if(t2 == 'A')
		{
			temp = color2 == 'R' ? cont.getRedStatsA() : cont.getBlueStatsA();
		}
	    else if(t2 == 'D')
		{
			temp = color2 == 'R' ? cont.getRedStatsD() : cont.getBlueStatsD();
		}
		else// if(t1 == 'S')
		{
			temp = color2 == 'R' ? cont.getRedStatsS() : cont.getBlueStatsS();
		}
		t2Health = temp[0];
		t2Attack = temp[1];
		
		int hitsToKillt2 = (int)Math.Ceiling((t2Health)/(1.0*t1Attack));
		int hitsToKillt1 = (int)Math.Ceiling(t1Health/(1.0*t2Attack));
		if(hitsToKillt1 == hitsToKillt2) 
		{
			return 0;
		}
		else if(hitsToKillt1 < hitsToKillt2)
		{
			return 1;
		}
		else // if(hitsToKillt1 > hitsToKillt2)
		{
			return 2;
		}
	}
	/**
	 * Only used in verifying predictWinner's input
	 */
	private bool verifyIn(char c, char[] possible)
	{
		for(int i = 0; i < possible.Length; i++)
		{
			if(c == possible[i])
			{
				return true;
			}
		}
		return false;
	}

	/**
	 * Returns the number of troops bought.
	 */
	public override int numberBought()
	{
		return speedCost - 25;
	}

	/**
	 * Returns the number of troops lost.
	 */
	public override int numberLost()
	{
		int stock = 0;
		foreach(Waypoint bl in waypoints)
		{
			if(bl != null && (binColor ? bl.occupiedRed : bl.occupiedBlue))
			{
				stock += bl.getCountTotal();
			}
		}
		return numberBought() - stock;
	}

	/**
	 * increments gold
	 */
	public void GimmeMoney()
	{
		gold++;
	}

	/**
	 * Makes troops in waypoints defend against attacks.
	 * same as level3() in first AI
	 */
	public void defend()
	{
		foreach(Waypoint wp in waypoints)
		{
			if(wp == null)
			{
				break;
			}
			if(binColor ? wp.occupiedRed : wp.occupiedBlue)
			{
				if(wp.getUnderFire())
				{
					first = wp;
					second = wp.getAttackedFrom();
				}
			}
		}
	}
}
