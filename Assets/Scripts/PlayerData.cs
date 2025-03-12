using Assets.Scripts.Managers;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts 
{
    public class PlayerData : MonoBehaviour
    {
        // RECORDS OF CURRENT STEPS OF ALGO - DATA TO SHOW IN STEP SELECTOR OR STORE
        private struct StepRecords
        {
            public int attempt;
            public string[] actionError; // None, Error description
            public string[] diagnosticError; // None, Error description
            public bool succeeded => actionError == null && diagnosticError == null;

            public StepRecords(int attempt, string[] actionError = null, string[] diagnosticError = null)
            {
                this.attempt = attempt;
                this.actionError = actionError;
                this.diagnosticError = diagnosticError;
            }
        }

        // RECORD OF CURRENT LEVEL - DATA TO SHOW IN LEVEL SELECTOR OR STORE
        private struct LevelRecord
        {
            public int levelAttempt; // Number of attempts for the level
            public int totActionError; // length of actionError
            public int totDiagnosticError; // length of diagnosticError
            public int nbStepSucced; // Number of succeeded (count number of succeeded in StepRecord)
            public int nbStepFailed; // Number of failed (count number of failed in StepRecord)
            public TimerData levelTime; // Time spent on the level
            public int successRate => nbStepSucced / (nbStepSucced + nbStepFailed);
            public int totError => totActionError + totDiagnosticError; // totActionError + totDiagnosticError

            public LevelRecord(int levelAttempt, int totActionError, int totDiagnosticError, int nbSucced, int nbFailed, TimerData time)
            {
                this.levelAttempt = levelAttempt;
                this.totActionError = totActionError;
                this.totDiagnosticError = totDiagnosticError;
                this.nbStepSucced = nbSucced;
                this.nbStepFailed = nbFailed;
                this.levelTime = time;
            }
        }

        // GLOBAL RECORDS
        private struct GlobalData
        {
            public int nbGames; // Number of games played
            public int nbLevelsCompleted; // Number of levels completed
            public int nbStepsCompleted; // Number of steps completed
            public int nbActionErrors; // Number of action errors
            public int nbDiagnosticErrors; // Number of diagnostic errors
            public TimerData gameTime;
            public TimerData lastSessionTime;
            
            public GlobalData(int nbGames, int nbLevelsCompleted, int nbStepsCompleted, int nbActionErrors, int nbDiagnosticErrors, TimerData gameTime, TimerData lastSessionTime)
            {
                this.nbGames = nbGames;
                this.nbLevelsCompleted = nbLevelsCompleted;
                this.nbStepsCompleted = nbStepsCompleted;
                this.nbActionErrors = nbActionErrors;
                this.nbDiagnosticErrors = nbDiagnosticErrors;
                this.gameTime = gameTime;
                this.lastSessionTime = lastSessionTime;
            }
        }

        // TIMER DATA
        private struct TimerData
        {
            public float startTime;
            public float elapsedTime;

            public TimerData(float startTime)
            {
                this.startTime = startTime;
                this.elapsedTime = 0f;
            }
        }

        // PRIVATE VARIABLES
        private Dictionary<int, StepRecords> _stepRecords { get; set; }
        private Dictionary<int, LevelRecord> _levelRecords { get; set; }
        private GlobalData _globalData;

        private TimerData _levelTimer;
        private TimerData _globalTimer;


        /// <summary>
        /// Initialize the record when a game start
        /// </summary>
        public void InitializeRecord()
        {
            _levelRecords = new Dictionary<int, LevelRecord>();
            _levelTimer = new TimerData(Time.time);
            //_stepRecords = new Dictionary<int, StepRecords>();
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _globalTimer = new TimerData(Time.time); // Start the global timer
            // try to get last session time on web request
            _globalData = new GlobalData(0, 0, 0, 0, 0, new TimerData(0), new TimerData(0));
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
