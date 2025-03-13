using UnityEngine;

public enum AudiometryResult
{
	NormalAudiogram,
	PerceptionSymmetry,
	Asymmetry,
	Transmission,
	InvertedSymmetry
}

[CreateAssetMenu(fileName = "Step4Data", menuName = "StepsData/Step4Data", order = 4)]
public class Step4Data : StepDataController
{
	private static readonly string[] _possibleDiagnostics = {
		"Rien à signaler",
		"Pathologie simple",
		"Pathologie complexe"
	};

	private static readonly string[] _possibleActions = {
		"Continuer la surveillance ICOPE",
		"Prescrire une prothèse auditive",
		"Renvoyer le patient vers un ORL"
	};

	public AudiometryResult audiometryResult;

	public Step4Data()
	{
		audiometryResult = AudiometryResult.NormalAudiogram;
	}

	public override GameObject GetStepDocumentPrefab()
	{
		return _gameData.step4Content;
	}

	public override void UpdateStepDocumentWithData(GameObject stepDocument)
	{
		stepDocument.TryGetComponent(out Step4Content step4Content);
		if (step4Content == null)
		{
			Debug.LogError("The step document prefab does not have the required component");
			return;
		}
		step4Content.DisplayAudiogramImage(audiometryResult);
	}

	public override string[] GetPossibleDiagnostics()
	{
		return _possibleDiagnostics;
	}

	public override string[] GetPossibleActions()
	{
		return _possibleActions;
	}

	public override bool IsDiagnosticCorrect(int chosenIndex)
	{
		return IsActionCorrect(chosenIndex);
	}

	public override bool IsActionCorrect(int chosenIndex)
	{
		return chosenIndex switch
		{
			0 => audiometryResult == AudiometryResult.NormalAudiogram, 
			1 => audiometryResult == AudiometryResult.PerceptionSymmetry,
			2 => audiometryResult != AudiometryResult.Asymmetry || audiometryResult != AudiometryResult.Transmission || audiometryResult != AudiometryResult.InvertedSymmetry,
			_ => false
		};
	}

	public override void RandomizeData(bool goesToNextStep)
	{
		// Ignore goesToNextStep as it is the last step
		audiometryResult = (AudiometryResult)Random.Range(0, 5);
	}

	public override bool LeadsToNextStep()
	{
		return false; // Step is the last one
	}
}