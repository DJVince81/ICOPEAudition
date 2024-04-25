using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "GameData", order = 1)]
public class GameData : ScriptableObject
{
	[Header("Video Otoscopy Images")]
	public Sprite normalEardrumImage;
	public Sprite cerumenImpactionImage;
	public Sprite tympanicPathologyImage;
	public Sprite ductPathologyImage;
	[Header("Audiometry Images")]
	public Sprite normalAudiogramImage;
	public Sprite perceptionSymmetryImage;
	public Sprite asymmetryImage;
	public Sprite transmissionImage;
	public Sprite invertedSymmetryImage;
}