using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public enum VideoOtoscopyResult
{
	NormalEardrum,
	CerumenImpaction,
	TympanicPathology,
	DuctPathology
}

[CreateAssetMenu(fileName = "Step2Data", menuName = "StepsData/Step2Data", order = 2)]
public class Step2Data : StepData
{
    private static readonly string[] _possibleDiagnostics = {
		"Tympans normaux",
		"Bouchon de cérumen",
		"Pathologie du tympan",
		"Pathologie du conduit"
	};

	private static readonly string[] _possibleActions = {
		"Faire passer un questionnaire HHIE-S et un test de Weber",
		"Effectuer une ablation du bouchon de cérumen, par soi-même si qualifié ou par un professionnel de santé",
		"Renvoyer le patient vers un ORL"
	};

	public VideoOtoscopyResult videoOtoscopyResult;

    public Step2Data()
    {
		videoOtoscopyResult = VideoOtoscopyResult.NormalEardrum;
    }

	public override string[] GetPossibleDiagnostics()
	{
		return _possibleDiagnostics;
	}

	public override string[] GetPossibleActions()
	{
		return _possibleActions;
	}

	public Sprite GetVideoOtoscopySprite()
	{
		return videoOtoscopyResult switch
		{
			VideoOtoscopyResult.NormalEardrum => gameData.normalEardrumImage,
			VideoOtoscopyResult.CerumenImpaction => gameData.cerumenImpactionImage,
			VideoOtoscopyResult.TympanicPathology => gameData.tympanicPathologyImage,
			VideoOtoscopyResult.DuctPathology => gameData.ductPathologyImage,
			_ => null
		};
	}

	public override bool IsDiagnosticCorrect(int chosenIndex)
	{
		return chosenIndex == (int)videoOtoscopyResult;
	}

	public override bool IsActionCorrect(int chosenIndex)
	{
		return chosenIndex switch
		{
			0 => videoOtoscopyResult == VideoOtoscopyResult.NormalEardrum,
			1 => videoOtoscopyResult == VideoOtoscopyResult.CerumenImpaction,
			2 => videoOtoscopyResult == VideoOtoscopyResult.TympanicPathology || videoOtoscopyResult == VideoOtoscopyResult.DuctPathology,
			_ => false,
		};
	}

	public override void RandomizeData(bool goesToNextStep)
	{
		if (goesToNextStep)
		{
			videoOtoscopyResult = VideoOtoscopyResult.NormalEardrum;
		}
		else
		{
			videoOtoscopyResult = (VideoOtoscopyResult)Random.Range(1, 4);
		}
	}

	public override bool LeadsToNextStep()
	{
		return IsActionCorrect(0);
	}
}