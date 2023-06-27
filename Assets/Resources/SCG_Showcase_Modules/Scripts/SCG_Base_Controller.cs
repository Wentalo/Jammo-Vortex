using UnityEngine;
using System.Collections;
using UnityEditor;
using UnityEngine.UI;

[ExecuteInEditMode]
public class SCG_Base_Controller : MonoBehaviour 
{
	
	[Header("     Text setup")]
	public Color textColor = new Color(1f,1f,1f,1f);
	public string baseText;
	Text textEditor;

	void Start ()
	{
	//Initialize components
		textEditor = this.GetComponentInChildren<Text>();
	//Do something 
		textEditor.text = baseText;
		textEditor.color = textColor;
	}

}
