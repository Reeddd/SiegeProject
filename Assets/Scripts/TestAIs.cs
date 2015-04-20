using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TestAIs : MonoBehaviour {

	private struct Results
	{
		public int numberFirstAIWins;
		public int numberBaseWins;

		public Results(string[] data)
		{
			numberFirstAIWins = Int32.Parse(data[0]);
			numberBaseWins = Int32.Parse (data[1]);
		}
	}

	Results results;
	float timer;
	string path = @".\testResults.txt";
	// Use this for initialization
	void Start () 
	{
		//results = readResults ();
		//Debug.Log (results.numberBaseWins +" "+results.numberFirstAIWins);
		timer = Time.time;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	private void writeResults(string toWrite)
	{
		//Potential Fields
		//Base AI
		using(StreamWriter sw = File.AppendText(path))
		{
			sw.WriteLine(toWrite);
		}
	}

	private Results readResults()
	{
		string path = @".\testResults.txt";

		if(!System.IO.File.Exists (path))
		{
			using(System.IO.FileStream fs = System.IO.File.Create (path))
			{
				return new Results();
			}
		}
		else
		{
			string[] lines = System.IO.File.ReadAllLines(path);
			for(int i=0; i < lines.Length; i++)
			{
				string[] temp = lines[i].Split (new Char[] {':'});
				lines[i] =temp[1];
			}
			foreach(string l in lines)
				Debug.Log (l);
			return new Results(lines);	
		}
	}

	public void addWin(string winAI, string lossAI, int winBought, int winDead, int lossBought, int lossDead)
	{
		Debug.Log (winAI);
		string[] temp = winAI.Split (new Char[] {'('});
		temp = temp[1].Split(new Char[] {')'});
		string winFinal = temp [0];
		temp = lossAI.Split (new Char[] {'('});
		temp = temp[1].Split(new Char[] {')'});
		string lossFinal = temp [0];
		string toPrint = "W: " + winFinal + " L: " + lossFinal + " Time: " + (Time.time - timer) + "Winner Bought: " + winBought + "Winner Deaths: " + winDead + "Loser Bought: " + lossBought + "Loser Deaths: " + lossDead;
		writeResults (toPrint);
		Application.LoadLevel ("Map2");
	}
}
