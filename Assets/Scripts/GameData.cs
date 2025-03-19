
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using static Assets.Scripts.Managers.GameStateManager;

namespace Assets.Scripts 
{  
    public class GameData : MonoBehaviour
    {
        // RECORDS OF CURRENT STEPS OF ALGO - DATA TO SHOW IN STEP SELECTOR OR STORE
        public struct StepRecords
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
        public struct LevelRecords
        {
            public int levelAttempt; // Number of attempts for the level
            public int totActionError; // length of actionError
            public int totDiagnosticError; // length of diagnosticError
            public int nbStepSucced; // Number of succeeded (count number of succeeded in StepRecord)
            public int nbStepFailed; // Number of failed (count number of failed in StepRecord)
            public TimerData timeSpentInLevel; // Time spent on the level
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
                this.timeSpentInLevel = levelTime;
                this.stepRecords = stepRecords;
            }
        }

        // GLOBAL RECORDS
        public struct GlobalData
        {
            public int nbGames; // Number of games played
            public int nbLevelsCompleted; // Number of levels completed
            public int nbStepsCompleted; // Number of steps completed
            public int globalActionErrors; // Number of action errors
            public int globalDiagnosticErrors; // Number of diagnostic errors
            public TimerData gameTime;
            public TimerData currentSessionTime;
            public Dictionary<LevelState, LevelRecords> levelRecords;
            
            public GlobalData(int nbGames, int nbLevelsCompleted, int nbStepsCompleted, int nbActionErrors, int nbDiagnosticErrors, TimerData gameTime, TimerData currentSessionTime, Dictionary<LevelState, LevelRecords> levelRecords)
            {
                this.nbGames = nbGames;
                this.nbLevelsCompleted = nbLevelsCompleted;
                this.nbStepsCompleted = nbStepsCompleted;
                this.globalActionErrors = nbActionErrors;
                this.globalDiagnosticErrors = nbDiagnosticErrors;
                this.gameTime = gameTime;
                this.currentSessionTime = currentSessionTime;
                this.levelRecords = levelRecords;
            }
        }

        // TIMER DATA
        public struct TimerData
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

        [SerializeField] private string path = "GameData";

        /// <summary>
        /// Initialize Dictionarys (_levelRecords<LevelState, LevelRecords> & _stepRecords<AlgoState, StepRecords>) when a game start (click on the GrandMa/GrandPa).
        /// </summary>
        public void InitializeRecords()
        {
            if (_levelRecords == null)
            {
                _levelRecords = new Dictionary<LevelState, LevelRecords>();
            }
            _stepRecords = new Dictionary<AlgoState, StepRecords>();
        }

        /// <summary>
        /// Set the dictionary<AlgoState, StepRecords> _stepRecords as a key an AlgoState (input parameter: currentAlgoState) and a value a new StepRecords.
        /// </summary>
        /// <param name="algoStep"></param>
        public void SetStepRecords(AlgoState algoStep)
        {
            if (!_stepRecords.ContainsKey(algoStep)) _stepRecords[algoStep] = new StepRecords(0, new List<string>(), new List<string>());
        }

        /// <summary>
        /// Modify/records the data form the player (attempt, actionError, diagnoticError) of the current step (AlgoState).
        /// Take as inputs AlgoState, string of action error and a string of diagnostic error. 
        /// </summary>
        /// <param name="algoStep"></param>
        /// <param name="actionError"></param>
        /// <param name="diagError"></param>
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

        /// <summary>
        /// Set the dictionary<LevelState, LevelRecords> _levelRecords as a key a LevelState (input parameter: currentLevelState) and value a new LevelRecords.
        /// </summary>
        /// <param name="levelState"></param>
        public void SetLevelRecords(LevelState levelState)
        {
            _levelTimer = new TimerData(Time.time);
            if (!_levelRecords.ContainsKey(levelState)) _levelRecords[levelState] = new LevelRecords(0, 0, 0, 0, 0, _levelTimer, _stepRecords);
        }

        /// <summary>
        /// Modify/records the data from the player (attempt, nbStepSucced, nbStepFailed, totActionError, totDiagnostics, timeSpentInLevel, stepRecords) of the current level (LevelState).
        /// </summary>
        /// <param name="levelState"></param>
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
            
            levelData.timeSpentInLevel.elapsedTime = Time.time - _levelTimer.startTime;
            levelData.stepRecords = _stepRecords;
            _levelRecords[levelState] = levelData;
        }

        /// <summary>
        /// Change the number of game when the game start.
        /// </summary>
        public void GlobalRecordsOnLevelStart()
        {
            _globalData.nbGames++;
        }

        /// <summary>
        /// Modify/records the data form the player (nbLevelsCompleted, nbStepsCompleted, globalActionErrors, globalDiagnosticErrors, gameTime, currentSessionTime and levelRecords).
        /// Save-it in xml file "GameData".
        /// </summary>
        public void GlobalRecordsOnLevelEnd()
        {
            _globalData.nbLevelsCompleted++;

            foreach(var level in _levelRecords)
            {
                LevelRecords levelData = level.Value;
                _globalData.nbStepsCompleted += levelData.nbStepSucced;            
                _globalData.globalActionErrors += levelData.totActionError;
                _globalData.globalDiagnosticErrors += levelData.totDiagnosticError;
            }

            _globalData.gameTime.elapsedTime = Time.time - _globalTimer.startTime + _globalData.gameTime.elapsedTime; // Add the time spend on the game (the global time)
            _globalData.currentSessionTime.elapsedTime = Time.time - _globalTimer.startTime; // Add the time spend on the session
            _globalData.levelRecords = _levelRecords;

            XmlManager.SaveToXml(_globalData, Path.Combine(Application.streamingAssetsPath, path), "GameData");
        }

        /// <summary>
        /// Unity fuction. On start set _globalTimer and try to get Data form xml file "GameData" and set _globalData, _levelRecords and _stepRecords with the loaded data.
        /// </summary>
        void Start()
        {
            _globalTimer = new TimerData(Time.time); // Start the global timer            
            // try to get last session time on web request
            path = Path.Combine(Application.streamingAssetsPath, path);
            if (File.Exists(path))
            {
                _globalData = XmlManager.LoadGameData(Path.Combine(Application.streamingAssetsPath, path));
                _levelRecords = _globalData.levelRecords;
                foreach (var level in _levelRecords)
                {
                    _stepRecords = level.Value.stepRecords;
                }
            }
            else
            {
                _globalData = new GlobalData();
            }
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
