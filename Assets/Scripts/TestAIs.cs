using UnityEngine;
using System.Collections;
using System.IO;
using System.Convert;

public class TestAIs : MonoBehaviour {

	private struct Results
	{
		public int numberFirstAIWins;
		public int numberBaseWins;

		public Results(string[] data)
		{
			numberFirstAIWins = Convert.Int32.Parse(data[0]);
			numberBaseWins = data[1];
		}
	}

	Results results;

	// Use this for initialization
	void Start () 
	{
		readResults ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	private void writeResults()
	{
		//Potential Fields
		//Base AI
		string[] toWrite;
		toWrite[0] = "First AI wins: " + results.numberFirstAIWins;
		toWrite[1] = "Base AI wins: " + results.numberBaseWins;
	}

	private void readResults()
	{
		string path = @".\testResults.txt";

		if(!System.IO.File.Exists (path))
		{
			using(System.IO.FileStream fs = System.IO.File.Create (path))
			{

			}
		}
		else
		{
			string[] lines = System.IO.File.ReadAllLines(path);
			Debug.Log (lines);
		}
	}
}
