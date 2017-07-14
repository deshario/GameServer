using UnityEngine;
using System.Collections;
using SocketIO;
using MyScript;
using System.Collections.Generic;

public class Chat : MonoBehaviour 
{

	private SocketIOComponent socket;
	private ectScript myscript;

	public List<string> chatHistory = new List<string>();

	private string currentMessage = string.Empty;
	private Vector2 scrollPos =  new Vector2(0, 0);
	private int lastLogLen = 0;
	public int MaxLogMessage = 200;

	public GUIStyle printGUIStyle;

	public bool selectTextfield = false;
	public bool visible = true;
	float maxLogLabelHeight = 100.0f;
	public bool enterPressed;
	void Start () 
	{
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		myscript = go.GetComponent<ectScript>();
		socket.On ("talk", ChatMessage);
	}

	void OnGUI()
	{
		enterPressed = false;

		Event e = Event.current;

		if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl () == "chatWindow") {

			enterPressed = true;

		}
		if (visible) {
			GUI.SetNextControlName ("chatWindow");
			currentMessage = GUI.TextField (new Rect (0.0f, Screen.height - 50, 200, 20), currentMessage, 25);
			if(GUI.Button(new Rect(210,Screen.height - 50,50,20),"Send")||enterPressed)
			{
				Debug.Log(currentMessage);
				if(selectTextfield) {
					selectTextfield = true;
				}
				if(currentMessage != "") {
					
					Dictionary<string, string> data = new Dictionary<string, string> ();
					data ["message"] = currentMessage;
					socket.Emit("talk", new JSONObject(data));
					currentMessage = "";
				}
			}

			if (!selectTextfield) {
				GUI.FocusControl  ("chatWindow");
			}

			float logBoxWidth = 180.0f;
			float[] logBoxHeights = new float[chatHistory.Count];
			float totalHeight = 0.0f;
			int i = 0;
			float logBoxHeight ;
			foreach (string c in chatHistory) {
				logBoxHeight = Mathf.Min (maxLogLabelHeight, printGUIStyle.CalcHeight (new GUIContent(c), logBoxWidth));
				logBoxHeights [i++] = logBoxHeight;
				totalHeight += logBoxHeight + 10.0f;
			}
//		GUILayout.BeginHorizontal(GUILayout.Width(250));
//		currentMessage = GUILayout.TextField (currentMessage);
//
//		if(GUILayout.Button("Send"))
//		{
//			Dictionary<string, string> data = new Dictionary<string, string> ();
//			data ["message"] = currentMessage;
//			socket.Emit("talk", new JSONObject(data));
//		}
//
//		GUILayout.EndHorizontal ();
			float innerScrollHeight = totalHeight;

			if (lastLogLen != chatHistory.Count) {
				scrollPos = new Vector2 (0.0f, innerScrollHeight);
				lastLogLen = chatHistory.Count;
			}

			scrollPos = GUI.BeginScrollView (new Rect (0.0f, Screen.height - 150.0f - 50.0f, 200, 150), scrollPos, new Rect (0.0f, 0.0f, 180.0f, innerScrollHeight));
			float currY = 0.0f;
			i = 0;

			foreach (string c in chatHistory) {
				logBoxHeight = logBoxHeights[i++];
				GUI.Label(new Rect(10.0f, currY, logBoxWidth, logBoxHeight), c, printGUIStyle);
				currY += logBoxHeight+10.0f;
			}

			GUI.EndScrollView ();
		}
	}

	public void ChatMessage(SocketIOEvent e)
	{
		string newString = myscript.jsontoString( e.data [0].ToString (),"\"") + " : " + myscript.jsontoString( e.data [1].ToString (),"\"");

		chatHistory.Add (newString);
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Return)) 
		{
			Debug.Log("mkmkmk");

			//Dictionary<string, string> data = new Dictionary<string, string> ();
			//data ["message"] = currentMessage;
			//socket.Emit("talk", new JSONObject(data));
		}
	}
}
