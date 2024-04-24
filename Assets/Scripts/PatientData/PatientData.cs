using UnityEngine;

[CreateAssetMenu(fileName = "PatientData", menuName = "PatientData", order = 0)]
public class PatientData : ScriptableObject
{
	public Step0Data step0Data;
	public Step1Data step1Data;
	public Step2Data step2Data;
	public Step3Data step3Data;
	public Step4Data step4Data;

	public void RandomizeData(int stopsAtStep)
	{
		step0Data.RandomizeData(stopsAtStep > 0);
		step1Data.RandomizeData(stopsAtStep > 1);
		step2Data.RandomizeData(stopsAtStep > 2);
		step3Data.RandomizeData(stopsAtStep > 3);
		step4Data.RandomizeData(false); // Step 4 is the last possible steps
	}
}