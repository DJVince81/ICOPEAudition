
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using static Assets.Scripts.Managers.GameStateManager;

namespace Assets.Scripts 
{  
    public class PlayerData : MonoBehaviour
    {
        // RECORDS OF CURRENT STEPS OF ALGO - DATA TO SHOW IN STEP SELECTOR OR STORE
        public struct StepRecords
        {
            public int attempt;
            [XmlArray("actionError"), XmlArrayItem("Error")]
            public List<string> actionError; // None, Error description
            [XmlArray("diagnosticError"), XmlArrayItem("Error")]
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
        public  struct LevelRecords
        {
            public int levelAttempt; // Number of attempts for the level
            public int totActionError; // length of actionError
            public int totDiagnosticError; // length of diagnosticError
            public int nbStepSucced; // Number of succeeded (count number of succeeded in StepRecord)
            public int nbStepFailed; // Number of failed (count number of failed in StepRecord)
            public TimerData levelTime; // Time spent on the level
            public int successRate => nbStepSucced / (nbStepSucced + nbStepFailed);
            public int totError => totActionError + totDiagnosticError; // totActionError + totDiagnosticError

            public LevelRecords(int levelAttempt, int totActionError, int totDiagnosticError, int nbSucced, int nbFailed, TimerData time)
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
        public struct GlobalData
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
        public  struct TimerData
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
        private Dictionary<AlgoState, StepRecords> _stepRecords { get; set; }
        private Dictionary<LevelState, LevelRecords> _levelRecords { get; set; }
        private GlobalData _globalData;
        private TimerData _levelTimer;
        private TimerData _globalTimer;

        private SaveData _saveData;

        /// <summary>
        /// Initialize the record when a game start.
        /// </summary>
        public void InitializeRecords()
        {
            _levelRecords = new Dictionary<LevelState, LevelRecords>();
            _stepRecords = new Dictionary<AlgoState, StepRecords>();
        }

        public void SetStepRecords(AlgoState algoStep)
        {
            if (!_stepRecords.ContainsKey(algoStep)) _stepRecords[algoStep] = new StepRecords(0, new List<string>(), new List<string>());
        }

        public void RecordsSteps(AlgoState algoStep, string actionError, string diagError)
        {
            if (!_stepRecords.ContainsKey(algoStep)) return;

            StepRecords stepData = _stepRecords[algoStep];
            stepData.attempt++;
            if (!string.IsNullOrEmpty(actionError)) stepData.actionError.Add(actionError);
            if (!string.IsNullOrEmpty(diagError)) stepData.diagnosticError.Add(diagError);
            _stepRecords[algoStep] = stepData;
        }

        public void StetLevelRecords(LevelState levelState)
        {
            _levelTimer = new TimerData(Time.time);
            if (!_levelRecords.ContainsKey(levelState)) _levelRecords[levelState] = new LevelRecords(0, 0, 0, 0, 0, _levelTimer);
        }

        public void RecordsLevels(LevelState levelState)
        {
            if (!_levelRecords.ContainsKey(levelState)) return;

            LevelRecords levelData = _levelRecords[levelState];
            levelData.levelAttempt++;
            
            foreach(var step in _stepRecords)
            {
                StepRecords stepData = step.Value; // Get the step data
                if (stepData.succeeded) levelData.nbStepSucced++;
                else levelData.nbStepFailed++;
                levelData.totActionError += stepData.actionError.Count;
                levelData.totDiagnosticError += stepData.diagnosticError.Count;
            }
            
            levelData.levelTime.elapsedTime = Time.time - _levelTimer.startTime;
            _levelRecords[levelState] = levelData;
        }

        public void GlobalRecordsOnLevelStart()
        {
            _globalData.nbGames++;
        } 

        public void GlobalRecordsOnLevelEnd()
        {
            _globalData.nbLevelsCompleted++;

            foreach(var level in _levelRecords)
            {
                LevelRecords levelData = level.Value;
                _globalData.nbStepsCompleted += levelData.nbStepSucced;            
                _globalData.nbActionErrors += levelData.totActionError;
                _globalData.nbDiagnosticErrors += levelData.totDiagnosticError;
            }

            _globalData.gameTime.elapsedTime = Time.time - _globalTimer.startTime + _globalData.gameTime.elapsedTime; // Add the time spend on the game (the global time)
            _globalData.currentSessionTime.elapsedTime = Time.time - _globalTimer.startTime; // Add the time spend on the session
        }

        /*
        Format that player data will be saved in the database:
            - UID
            - PlayerData
                |  GlobalData
                |  LevelData
                |  StepData    
        
        Save data form:
            UID : UID_Player
            Section : GlobalData
             | Body : - nbGames, 
             |   |    - nbLevelsCompleted, 
             |   |    - nbStepsCompleted, 
             |   |    - nbActionErrors, 
             |   |    - nbDiagnosticErrors, 
             |   |    - gameTime, 
             |   |    - currentSessionTime,
            Section : LevelData
             | Body : - levelAttempt,
             |   |    - totActionError, 
             |   |    - totDiagnosticError, 
             |   |    - nbStepSucced, 
             |   |    - nbStepFailed, 
             |   |    - levelTime,
            Section : StepData
             | Body : - attempt, 
             |   |    - actionError, 
             |   |    - diagnosticError
         */

        public void OnStepCompleted(AlgoState currentAlgoState, StepRecord records)
        {
            StepEntry entry = _saveData.Data.StepEntries.Find(e => e.Step == currentAlgoState);
            if (entry == null)
            {
                entry = new StepEntry { Step = currentAlgoState, Records = records };
                _saveData.Data.StepEntries.Add(entry);
            }
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


/*
 XML FORMAT:
    | UID - string
    | PlayerData
    |   | GlobalData - Struct
    |   |   | nbGames - int
    |   |   | nbLevelsCompleted - int
    |   |   | nbStepsCompleted - int
    |   |   | nbActionErrors - int
    |   |   | nbDiagnosticErrors - int
    |   |   | gameTime - TimerData
    |   |   | currentSessionTime - TimerData 
    |   | LevelData - Struct
    |   |   | Level : Level 0 - enum
    |   |   |   | levelAttempt - int
    |   |   |   | totActionError - int
    |   |   |   | totDiagnosticError - int
    |   |   |   | nbStepSucced - int
    |   |   |   | nbStepFailed - int
    |   |   |   | levelTime - TimerData
    |   |   | Level : Level 1 - enum
    |   |   |   | levelAttempt - int
    |   |   |   | totActionError - int
    |   |   |   | totDiagnosticError - int
    |   |   |   | nbStepSucced - int
    |   |   |   | nbStepFailed - int
    |   |   |   | levelTime - TimerData
    |   | StepData - Struct
    |   |   | Step : Questionary - enum
    |   |   |   | attempt - int
    |   |   |   | actionError - List<string>
    |   |   |   | diagnosticError - List<string>
    |   |   | Step : Diagnostic - enum
    |   |   |   | attempt - int
    |   |   |   | actionError - List>string>
    |   |   |   | diagnosticError - List<string>
 
LevelData : Dictonary<LevelState, LevelRecord>
StepData : Dictonary<AlgoState, StepRecords>

 */
