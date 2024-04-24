using UnityEngine;

public abstract class StepData : ScriptableObject
{
	/// <summary>
	/// Returns the possible diagnostics for the step
	/// </summary>
	/// <returns>An array of strings representing the possible diagnostics</returns>
	public abstract string[] GetPossibleDiagnostics();
	/// <summary>
	/// Returns the possible actions for the step
	/// </summary>
	/// <returns>An array of strings representing the possible actions</returns>
	public abstract string[] GetPossibleActions();

	/// <summary>
	/// Checks if the diagnostic made by the player corresponds to the data
	/// </summary>
	/// <param name="chosenIndex">The player's chosen diagnostic's index</param>
	/// <returns>True if the diagnostic is correct, false otherwise</returns>
	public abstract bool IsDiagnosticCorrect(int chosenIndex);
	/// <summary>
	/// Checks if the action chosen by the player corresponds to the data
	/// </summary>
	/// <param name="chosenIndex">The player's chosen action's index</param>
	/// <returns>True if the action is correct, false otherwise</returns>
	public abstract bool IsActionCorrect(int chosenIndex);
	/// <summary>
	/// Randomizes the data for the step
	/// </summary>
	/// <param name="goesToNextStep">Whether the data generated allows the patient to go to the next step</param>
	public abstract void RandomizeData(bool goesToNextStep);
	/// <summary>
	/// Checks if the step leads to the next step
	/// </summary>
	/// <returns>True if the step leads to the next step, false otherwise</returns>
	public abstract bool LeadsToNextStep();
}