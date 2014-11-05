using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class ChatWindow : MonoBehaviour {

	public static ChatWindow instance;
	public static string FOCUS_OUT_UID = "FOCUS_OUT_FIELD_ENDCHAT";

	public int maxLength = 1100;
	public int maxLogMessages = 200;
	public bool visible = true;
	public bool showTimestamp = true;

	public string stringToEdit = "";
	public bool selectTextfield = false;
	public bool isScrolling = false;

	public float windowHeight = 150;
	public float windowWidth = 400;
	public float entryHeight = 20;
	public float entrySpacing = 0;

	public GUIStyle printGUIStyle;

	private List<LogMessage> log = new List<LogMessage>();

	private bool returnPressed = false;
	private bool changeFocus = false;
	private bool focused = false;

	private string focusName = "ChatWindow";

	private Vector2 scrollPos = Vector2.zero;
	private int lastLogLen = 0;
	private List<float> logBoxHeights = new List<float>();

	private string dateFormat_12 = "[hh:mm:ss tt]";
	private string dateFormat_24 = "[HH:mm:ss]";

	private string username = "LaughingLeader";

	void Awake() {
		instance = this;
		
	}
	// Use this for initialization
	void Start () {
		//Input.eatKeyPressOnTextFieldFocus = false;
		printGUIStyle.normal.textColor = Color.white;
		printGUIStyle.wordWrap = true;
		printGUIStyle.stretchWidth = false;
		print("Alpha Build 1.0", false, "", "#00ffffff");
	}

	void print(string m, bool timeStamp = true, string u = "", string MessageColor = "")
	{
		if (m.Length > maxLength) m = m.Substring(0, maxLength);
		//m = m.Replace(System.Environment.NewLine, " ");

		string ts = timeStamp ? DateTime.Now.ToString(dateFormat_12) : "";

		log.Add(new LogMessage(m, u, ts, MessageColor));
	
		if(log.Count > maxLogMessages){
			log.RemoveAt(0);
		}
	}

	void OnGUI()
	{
		Rect entryRect = new Rect(0, Screen.height - 50, windowWidth, entryHeight);

		if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return){
			returnPressed = true;
		}

		float topY = Screen.height - 150 - 50;
		Rect logBox = new Rect(0, topY, windowWidth, windowHeight + entryHeight);
		if (Event.current.type == EventType.MouseDown)
		{
			selectTextfield = logBox.Contains(Event.current.mousePosition);
		}

		isScrolling = Event.current.type == EventType.ScrollWheel && logBox.Contains(Event.current.mousePosition);
		
		if(visible){
			GUI.SetNextControlName(focusName);
			stringToEdit = GUI.TextField(entryRect, stringToEdit, maxLength);
		}

		if (selectTextfield){
			GUI.FocusControl(focusName);
			Input.ResetInputAxes();
			focused = true;
		} else {
			GUI.SetNextControlName(FOCUS_OUT_UID);
			GUI.Label(new Rect(-100, -100, 1, 1), "");
			GUI.FocusControl(FOCUS_OUT_UID);
			focused = false;
		}

		
		//if (changeFocus)
		//{
		//	GUI.SetNextControlName(focusName);
		//	if (GUI.GetNameOfFocusedControl() != focusName)
		//	{
		//		GUI.FocusControl(focusName);
		//	}
		//	else
		//	{
		//		GUI.FocusControl("");
		//	}
		//	Debug.Log("changeFocus:" + changeFocus + " GUIFocus:" + GUI.GetNameOfFocusedControl());
		//	changeFocus = false;
		//}

		float logBoxWidth = windowWidth;
		float logBoxHeight = 0.0f;
		logBoxHeights.Clear();
		float totalHeight = 0.0f;
		
		int i = 0;

		for (int k = 0; k < log.Count;k++){
			LogMessage lm = log[k];
			logBoxHeight = printGUIStyle.CalcHeight(new GUIContent(lm.toString()), logBoxWidth);
			logBoxHeights.Add(logBoxHeight);
			totalHeight += logBoxHeight + entrySpacing;
			i++;
		}

		float innerScrollHeight = totalHeight;
		// if there's a new message, automatically scroll to bottom
		if (lastLogLen != log.Count){
			scrollPos.Set(0.0f, innerScrollHeight);
			lastLogLen = log.Count;
		}

		scrollPos = GUI.BeginScrollView(new Rect(0, Screen.height - 150 - 50, windowWidth, windowHeight), scrollPos, new Rect(0, 0, (windowWidth * 0.80f), innerScrollHeight));
		//GUILayout.BeginArea(new Rect(0, Screen.height - 150 - 50, windowWidth, windowHeight));
		//scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(windowWidth), GUILayout.Height(innerScrollHeight));
			
		float currY = 0.0f;
		i = 0;

		for (int k = 0; k < log.Count; k++)
		{
			LogMessage lm = log[k];
			logBoxHeight = logBoxHeights[i++];
			GUI.Label(new Rect(0, currY, logBoxWidth, logBoxHeight), lm.toString(), printGUIStyle);
			//GUILayout.Label(message, printGUIStyle);
			currY += logBoxHeight + entrySpacing;
		}

		GUI.EndScrollView();
		//GUILayout.EndScrollView();
		//GUILayout.EndArea();

	}
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return) || returnPressed)
		{
			if (stringToEdit != "") {
				print(stringToEdit, showTimestamp, username);
				stringToEdit = "";
			}
			selectTextfield = !selectTextfield;
			returnPressed = false;
		}

		//Debug.Log("selectTextfield: " + selectTextfield + " changeFocus:" + changeFocus + " GUIFocus:" + GUI.GetNameOfFocusedControl());
	}
}

class LogMessage
{
	public string message = "";
	public string user = "";
	public string timestamp = "";

	public string tsColor = "#ffa500ff";
	public string userColor = "#00ff00ff";
	public string messageColor = "";

	public int length()
	{
		return toString().Length;
	}

	public string toString()
	{
		string m = "";
		if (timestamp != "") {
			m = tsColor != "" ? "<color=" + tsColor + ">" + timestamp + "</color>" : timestamp;
		}
		if (user != "") {
			m = string.Concat(m, userColor != "" ? " <color=" + userColor + ">" +user+":</color> " : user+" ");
		}
		if (message != "") {
			m = string.Concat(m, messageColor != "" ? "<color=" + messageColor + ">" + message + "</color>" : message);
		}
		return m;
	}

	public LogMessage(string m, string u="", string ts="", string MessageColor="")
	{
		message = m;
		user = u;
		timestamp = ts;
		messageColor = MessageColor;
	}
}
