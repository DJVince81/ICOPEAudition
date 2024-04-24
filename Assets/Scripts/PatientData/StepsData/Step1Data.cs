using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Step1Data", menuName = "StepsData/Step1Data", order = 1)]
public class Step1Data : StepData
{
	private static readonly string[] _possibleDiagnostic = new string[]
	{
		"Pathologie simple",
		"Pathologie complexe"
	};

	private static readonly string[] _possibleActions = new string[]
	{
		"Faire passer une vidéo-otoscopie",
		"Renvoyer le patient vers un ORL"
	};

	public bool[] goNoGoAnswers;

	public Step1Data()
	{
		goNoGoAnswers = new bool[6];
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
		// If the patient has a complex pathology (at least one answer is Yes), the chosen diagnostic should be the one of index 1
		return goNoGoAnswers.Contains(true) == (chosenIndex == 1);
	}

	public override bool IsActionCorrect(int chosenIndex)
	{
		// If the patient has a complex pathology (at least one answer is Yes), the chosen action should be the one of index 1
		return goNoGoAnswers.Contains(true) == (chosenIndex == 1);
	}

	public override void RandomizeData(bool goesToNextStep)
	{
		if (goesToNextStep)
		{
			for (int i = 0; i < goNoGoAnswers.Length; i++)
			{
				goNoGoAnswers[i] = false;
			}
		}
		else
		{
			for (int i = 0; i < goNoGoAnswers.Length; i++)
			{
				goNoGoAnswers[i] = Random.Range(0, 2) == 1;
			}

		}
	}

	public override bool LeadsToNextStep()
	{
		return IsActionCorrect(0);
	}
}