using Assets.Scripts.PatientData.AlgoData;
using UnityEngine;

namespace Assets.Scripts.PatientData
{
    public class AlgoStepRunner : MonoBehaviour
    {
        public AlgoStep step;

        public void ShowContext()
        {
            Debug.Log("Contexte: " + step.contextDescription);
        }

        public void ShowDiagnosticQuestion()
        {
            Debug.Log("Diagnotic: " + step.diagnosticPhase.questionText);
        }

        public void OnDiagnoticAnswer(int index)
        {
            if (!step.hasDiagnosticPhase)
            {
                Debug.LogWarning("Pas de phase de diagnotics sur cette étape");
                return;
            }
            
            if (step.diagnosticPhase.InAnswerCorrect(index))
                Debug.Log("Bon diagnotic");
            else
                Debug.Log("Erreur: " + step.diagnosticPhase.GetCorrection(index));
        }

        public void ShowActionQuestion()
        {
            Debug.Log("Action :" + step.actionPhase.questionText);
        }

        public void OnActionAnwer(int index)
        {
            if (!step.hasActionPhase)
            {
                Debug.LogWarning("Pas de phase de choix d'action sur cette étape");
                return;
            }
            
            if (step.actionPhase.InAnswerCorrect(index))
                Debug.Log("Bonne reponse");
            else
                Debug.Log("Erreur: " + step.actionPhase.GetCorrection(index));
        }
    }

}

