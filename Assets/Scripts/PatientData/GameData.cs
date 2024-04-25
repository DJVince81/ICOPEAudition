using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameData", order = 1)]
public class GameData : ScriptableObject
{
	[Header("Documents to display per step")]
	public GameObject step0Content;
	public GameObject step1Content;
	public GameObject step2Content;
	public GameObject step3Content;
	public GameObject step4Content;
}