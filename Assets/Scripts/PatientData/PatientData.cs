using UnityEngine;

[CreateAssetMenu(fileName = "PatientData", menuName = "PatientData", order = 0)]
public class PatientData : ScriptableObject
{
	public Step0Data step0Data;
	public Step1Data step1Data;
	public Step2Data step2Data;
	public Step3Data step3Data;
	public Step4Data step4Data;

	/// <summary>
	/// Generates random data for all steps the patient can go through in order for them to reach step "stopsAtStep".
	/// </summary>
	/// <param name="stopsAtStep">The step the data is generated for the patient to reach.</param>
	public void RandomizeData(int stopsAtStep)
	{
		step0Data = CreateInstance<Step0Data>();
		step1Data = CreateInstance<Step1Data>();
		step2Data = CreateInstance<Step2Data>();
		step3Data = CreateInstance<Step3Data>();
		step4Data = CreateInstance<Step4Data>();

		step0Data.RandomizeData(stopsAtStep > 0);
		step1Data.RandomizeData(stopsAtStep > 1);
		step2Data.RandomizeData(stopsAtStep > 2);
		step3Data.RandomizeData(stopsAtStep > 3);
		step4Data.RandomizeData(false); // Step 4 is the last possible steps
	}

	/// <summary>
	/// Gets all different StepDatas as the abstract type StepData.
	/// </summary>
	/// <returns></returns>
	public StepData[] GetStepDatas()
	{
		return new StepData[] {
			step0Data,
			step1Data,
			step2Data,
			step3Data,
			step4Data
		};
	}
}