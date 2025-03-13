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
            public List<string> actionError; // None, Error description
            public List<string> diagnosticError; // None, Error description
            public bool succeeded => actionError == null && diagnosticError == null;

            public StepRecords(int attempt, List<string> actionError, List<string> diagnosticError)
            {
                this.attempt = attempt;
                this.actionError = actionError ;
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
            public TimerData currentSessionTime;
            
            public GlobalData(int nbGames, int nbLevelsCompleted, int nbStepsCompleted, int nbActionErrors, int nbDiagnosticErrors, TimerData gameTime, TimerData currentSessionTime)
            {
                this.nbGames = nbGames;
                this.nbLevelsCompleted = nbLevelsCompleted;
                this.nbStepsCompleted = nbStepsCompleted;
                this.nbActionErrors = nbActionErrors;
                this.nbDiagnosticErrors = nbDiagnosticErrors;
                this.gameTime = gameTime;
                this.currentSessionTime = currentSessionTime;
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
        /// Initialize the record when a game start.
        /// </summary>
        public void InitializeRecords()
        {
            _levelRecords = new Dictionary<int, LevelRecord>();
            _stepRecords = new Dictionary<int, StepRecords>();
        }

        public void SetStepRecords(int stepIndex)
        {
            if (!_stepRecords.ContainsKey(stepIndex)) _stepRecords[stepIndex] = new StepRecords(0, new List<string>(), new List<string>());
        }

        public void RecordsSteps(int stepIndex, string actionError, string diagError)
        {
            if (!_stepRecords.ContainsKey(stepIndex)) return;

            StepRecords stepData = _stepRecords[stepIndex];
            stepData.attempt++;
            if (!string.IsNullOrEmpty(actionError)) stepData.actionError.Add(actionError);
            if (!string.IsNullOrEmpty(diagError)) stepData.diagnosticError.Add(diagError);
            _stepRecords[stepIndex] = stepData;
        }

        public void StetLevelRecords(int levelIndex)
        {
            _levelTimer = new TimerData(Time.time);
            if (!_levelRecords.ContainsKey(levelIndex)) _levelRecords[levelIndex] = new LevelRecord(0, 0, 0, 0, 0, _levelTimer);
        }

        public void RecordsLevels(int levelIndex)
        {
            if (!_levelRecords.ContainsKey(levelIndex)) return;

            LevelRecord levelData = _levelRecords[levelIndex];
            levelData.levelAttempt++;
            
            for (int i = 0; i < _stepRecords.Count; i++)
            {
                StepRecords stepData = _stepRecords[i];
                if (stepData.succeeded) levelData.nbStepSucced++;
                else levelData.nbStepFailed++;
                levelData.totActionError += stepData.actionError.Count;
                levelData.totDiagnosticError += stepData.diagnosticError.Count;
            }


            levelData.levelTime.elapsedTime = Time.time - _levelTimer.startTime;
            _levelRecords[levelIndex] = levelData;
        }

        public void GlobalRecordsOnLevelStart()
        {
            _globalData.nbGames++;
        } 

        public void GlobalRecordsOnLevelEnd()
        {
            _globalData.nbLevelsCompleted++;

            for (int i = 0; i < _stepRecords.Count; i++)
            {
                StepRecords stepData = _stepRecords[i];
                if (stepData.succeeded) _globalData.nbStepsCompleted++;
                else
                {
                    _globalData.nbActionErrors += stepData.actionError.Count;
                    _globalData.nbDiagnosticErrors += stepData.diagnosticError.Count;
                }
            }

            _globalData.gameTime.elapsedTime = Time.time - _globalTimer.startTime + _globalData.gameTime.elapsedTime; // Add the time spend on the game (the global time)
            _globalData.currentSessionTime.elapsedTime = Time.time - _globalTimer.startTime; // Add the time spend on the session
        } 

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _globalTimer = new TimerData(Time.time); // Start the global timer
            // try to get last session time on web request
            _globalData = new GlobalData(0, 0, 0, 0, 0, _globalTimer, _globalTimer);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
