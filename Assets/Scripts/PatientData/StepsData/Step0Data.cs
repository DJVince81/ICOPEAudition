using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Step0Data", menuName = "StepsData/Step0Data", order = 0)]
public class Step0Data : StepData
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
		// If the patient has a problem (at least one answer is Yes), the chosen diagnostic should be the one of index 1
		return whisperTestData.Contains(true) == (chosenIndex == 1);
	}

	public override bool IsActionCorrect(int chosenIndex)
	{
		// If the patient has a problem (at least one answer is Yes), the chosen action should be the one of index 1
		return whisperTestData.Contains(true) == (chosenIndex == 1);
	}

	public override void RandomizeData(bool goesToNextStep)
	{
		if (goesToNextStep)
		{
			for (int i = 0; i < whisperTestData.Length; i++)
			{
				whisperTestData[i] = Random.Range(0, 2) == 1;
			}
			if (!whisperTestData.Contains(true)) // If all answers are No, we need to have at least one Yes
			{
				whisperTestData[Random.Range(0, whisperTestData.Length)] = true;
			}
		}
		else
		{
			for (int i = 0; i < whisperTestData.Length; i++)
			{
				whisperTestData[i] = false;
			}
		}
	}

	public override bool LeadsToNextStep()
	{
		return IsActionCorrect(1);
	}
}