using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Step0Data", menuName = "StepsData/Step0Data", order = 0)]
public class Step0Data : StepDataController
{
	private static readonly string[] _possibleDiagnostic = new string[]
	{
		"Rien à signaler",
		"Il y a un problème"
	};

	private static readonly string[] _possibleActions = new string[]
	{
		"Poursuivre le suivi habituel (prochain rendez-vous dans 6 mois)",
		"Faire passer un questionnaire go / no-go"
	};

	public bool[] whisperTestData;

	public Step0Data()
	{
		whisperTestData = new bool[3];
	}

	public override GameObject GetStepDocumentPrefab()
	{
		return _gameData.step0Content;
	}

	public override void UpdateStepDocumentWithData(GameObject stepDocument)
	{
		stepDocument.TryGetComponent(out Step0Content step0Content);
		if (step0Content == null)
		{
			Debug.LogError("The step document prefab does not have the required component");
			return;
		}
		step0Content.SetYesNoTexts(whisperTestData);
	}

	public override string[] GetPossibleDiagnostics()
	{
		return _possibleDiagnostic;
	}

	public override string[] GetPossibleActions()
	{
		return _possibleActions;
	}

	public override bool IsDiagnosticCorrect(int chosenIndex)
	{
		// There is a problem if one of the whisper test answers is wrong, or if the patient declares there is a problem
		return (!whisperTestData[0] || !whisperTestData[1] || whisperTestData[2]) == (chosenIndex == 1);
	}

	public override bool IsActionCorrect(int chosenIndex)
	{
		return IsDiagnosticCorrect(chosenIndex);
	}

	public override void RandomizeData(bool goesToNextStep)
	{
		if (goesToNextStep)
		{
			for (int i = 0; i < whisperTestData.Length; i++)
			{
				whisperTestData[i] = Random.Range(0, 2) == 1;
			}
			if (whisperTestData[0] && whisperTestData[1] && !whisperTestData[2])
			{
				int indexToChange = Random.Range(0, whisperTestData.Length);
				whisperTestData[indexToChange] = !whisperTestData[indexToChange];
			}
		}
		else
		{
			whisperTestData[0] = true;
			whisperTestData[1] = true;
			whisperTestData[2] = false;
		}
	}

	public override bool LeadsToNextStep()
	{
		return IsActionCorrect(1);
	}
}