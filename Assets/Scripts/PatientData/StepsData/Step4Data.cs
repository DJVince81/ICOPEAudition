using UnityEditor;
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
public class Step4Data : StepData
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

	public Step4Data() : base()
	{
		audiometryResult = AudiometryResult.NormalAudiogram;
    }

	public Sprite GetAudiometrySprite()
	{
		return audiometryResult switch
		{
			AudiometryResult.NormalAudiogram => gameData.normalAudiogramImage,
			AudiometryResult.PerceptionSymmetry => gameData.perceptionSymmetryImage,
			AudiometryResult.Asymmetry => gameData.asymmetryImage,
			AudiometryResult.Transmission => gameData.transmissionImage,
			AudiometryResult.InvertedSymmetry => gameData.invertedSymmetryImage,
			_ => null
		};
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
		return chosenIndex switch
		{
			0 => audiometryResult == AudiometryResult.NormalAudiogram,
			1 => audiometryResult == AudiometryResult.PerceptionSymmetry,
			2 => audiometryResult != AudiometryResult.NormalAudiogram && audiometryResult != AudiometryResult.PerceptionSymmetry,
			_ => false
		};
	}

	public override bool IsActionCorrect(int chosenIndex)
	{
		return chosenIndex switch
		{
			0 => audiometryResult == AudiometryResult.NormalAudiogram,
			1 => audiometryResult == AudiometryResult.PerceptionSymmetry,
			2 => audiometryResult != AudiometryResult.NormalAudiogram && audiometryResult != AudiometryResult.PerceptionSymmetry,
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