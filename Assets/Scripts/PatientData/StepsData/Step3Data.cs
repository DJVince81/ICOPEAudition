using UnityEngine;

public enum HHIESAnswer : int
{
	No,
	Sometimes,
	Yes
}

public enum WeberTestResult : int
{
	Center,
	RightEar,
	LeftEar
}

public enum Ear : int
{
	RightEar,
	LeftEar
}

[CreateAssetMenu(fileName = "Step3Data", menuName = "StepsData/Step3Data", order = 3)]
public class Step3Data : StepData
{
	private static readonly string[] _possibleDiagnostic = new string[]
	{
		"Oreilles saines",
		"Surdité de transmission",
		"Surdité de perception"
	};

	private static readonly string[] _possibleActions = new string[]
	{
		"Faire passer une audiométrie",
		"Renvoyer le patient vers un ORL",
		"Prescrire une prothèse"
	};

	public HHIESAnswer[] hhiesAnswers;
	public WeberTestResult weberTestResult;
	public Ear affectedEar;

	public Step3Data()
	{
		hhiesAnswers = new HHIESAnswer[10];
	}

	public int GetHHIESScore()
	{
		int score = 0;
		for (int i = 0; i < hhiesAnswers.Length; i++)
		{
			score += (int)hhiesAnswers[i] * 2;
		}
		return score;
	}

	public override string[] GetPossibleDiagnostics()
	{
		return _possibleDiagnostic;
	}

	public override string[] GetPossibleActions()
	{
		return _possibleActions;
	}

	// TODO: Correct once we have the actual conditions
	public override bool IsDiagnosticCorrect(int chosenIndex)
	{
		if (weberTestResult == WeberTestResult.Center) return chosenIndex == 0;

		// The patient has a transmission deafness if the Weber test result is the same as the affected ear
		bool isTransmissionDeafness = weberTestResult == WeberTestResult.RightEar && affectedEar == Ear.RightEar ||
			weberTestResult == WeberTestResult.LeftEar && affectedEar == Ear.LeftEar;
		return chosenIndex == (isTransmissionDeafness ? 1 : 2);
	}

	// TODO: Correct once we have the actual conditions
	public override bool IsActionCorrect(int chosenIndex)
	{
		if (weberTestResult == WeberTestResult.Center) return chosenIndex == 0;

		// The patient has a transmission deafness if the Weber test result is the same as the affected ear
		bool isTransmissionDeafness = weberTestResult == WeberTestResult.RightEar && affectedEar == Ear.RightEar ||
			weberTestResult == WeberTestResult.LeftEar && affectedEar == Ear.LeftEar;
		return chosenIndex == (isTransmissionDeafness ? 1 : 2);
	}

	// TODO: Correct once we have the actual conditions
	public override void RandomizeData(bool goesToNextStep)
	{
		for (int i = 0; i < hhiesAnswers.Length; i++)
		{
			hhiesAnswers[i] = (HHIESAnswer)Random.Range(0, 3);
		}
		affectedEar = (Ear)Random.Range(0, 2);
		if (goesToNextStep)
		{
			weberTestResult = WeberTestResult.Center;
		}
		else
		{
			weberTestResult = (WeberTestResult)Random.Range(1, 3);
		}
	}

	public override bool LeadsToNextStep()
	{
		return IsActionCorrect(0);
	}
}