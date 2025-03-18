
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using static Assets.Scripts.Managers.GameStateManager;

namespace Assets.Scripts 
{  
    public class GameData : MonoBehaviour
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
                this.actionError = actionError;
                this.diagnosticError = diagnosticError;
            }
        }

        // RECORD OF CURRENT LEVEL - DATA TO SHOW IN LEVEL SELECTOR OR STORE
        private struct LevelRecords
        {
            public int levelAttempt; // Number of attempts for the level
            public int totActionError; // length of actionError
            public int totDiagnosticError; // length of diagnosticError
            public int nbStepSucced; // Number of succeeded (count number of succeeded in StepRecord)
            public int nbStepFailed; // Number of failed (count number of failed in StepRecord)
            public TimerData levelTime; // Time spent on the level
            public int successRate => nbStepSucced / (nbStepSucced + nbStepFailed);
            public int totError => totActionError + totDiagnosticError; // totActionError + totDiagnosticError
            public Dictionary<AlgoState, StepRecords> stepRecords; // StepRecords of the level

            public LevelRecords(int levelAttempt, int totActionError, int totDiagnosticError, int nbStepSucced, int nbStepFailed, TimerData levelTime, Dictionary<AlgoState, StepRecords> stepRecords)
            {
                this.levelAttempt = levelAttempt;
                this.totActionError = totActionError;
                this.totDiagnosticError = totDiagnosticError;
                this.nbStepSucced = nbStepSucced;
                this.nbStepFailed = nbStepFailed;
                this.levelTime = levelTime;
                this.stepRecords = stepRecords;
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
            public Dictionary<LevelState, LevelRecords> levelRecords;
            
            public GlobalData(int nbGames, int nbLevelsCompleted, int nbStepsCompleted, int nbActionErrors, int nbDiagnosticErrors, TimerData gameTime, TimerData currentSessionTime, Dictionary<LevelState, LevelRecords> levelRecords)
            {
                this.nbGames = nbGames;
                this.nbLevelsCompleted = nbLevelsCompleted;
                this.nbStepsCompleted = nbStepsCompleted;
                this.nbActionErrors = nbActionErrors;
                this.nbDiagnosticErrors = nbDiagnosticErrors;
                this.gameTime = gameTime;
                this.currentSessionTime = currentSessionTime;
                this.levelRecords = levelRecords;
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
        private Dictionary<AlgoState, StepRecords> _stepRecords { get; set; }
        private Dictionary<LevelState, LevelRecords> _levelRecords { get; set; }
        private GlobalData _globalData;
        private TimerData _levelTimer;
        private TimerData _globalTimer;

        private readonly string path = "GameData";

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

        public bool PlayerHasAttemptStep(AlgoState algoState)
        {
            return _stepRecords.ContainsKey(algoState) && _stepRecords[algoState].attempt > 0;
        }

        public string StepRecodsToString(int algoState)
        {
           // Ajouter la suite
            return "Attemps: " + _stepRecords[(AlgoState)algoState].attempt + "\nChoix action: ajouter la suite";
        }

        public void SetLevelRecords(LevelState levelState)
        {
            _levelTimer = new TimerData(Time.time);
            if (!_levelRecords.ContainsKey(levelState)) _levelRecords[levelState] = new LevelRecords(0, 0, 0, 0, 0, _levelTimer, _stepRecords);
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
            levelData.stepRecords = _stepRecords;
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
            _globalData.levelRecords = _levelRecords;

            XmlManager.SaveToXml(_globalData, Path.Combine(Application.streamingAssetsPath, path), "GameData");
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _globalTimer = new TimerData(Time.time); // Start the global timer
            // try to get last session time on web request
            _globalData = new GlobalData(0, 0, 0, 0, 0, _globalTimer, _globalTimer, _levelRecords);
        }
    }
}


/*
 * 
 * Format that player data will be saved in the database:
    - UID
    - PlayerData
        |  GlobalData
        |  LevelData
        |  StepData   

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
