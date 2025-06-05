using Assets.Scripts.PatientData;
using Assets.Scripts.PatientData.AlgoData;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.Overlays;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameStateManager : MonoBehaviour
    {

        // ENUM
        private enum GameState { MAIN_MENU, GAME_MENU } // Enums for Main_menu and waiting_room

        public enum LevelState { LEVEL_0, LEVEL_1, LEVEL_2, LEVEL_3, LEVEL_4, LEVEL_5 }
        public enum PatientCase { PATIENT_0, PATIENT_1, PATIENT_2 }


        // VARIABLES
        private GameState gameState;
        private LevelState currentLevel;
        private PatientCase currentPatientCase;
        private Step currentStep;

        private LevelsData LevelsData;
        private PatientCaseData patientCaseData;
        private NewPatientData patientData;

        // MAIN MENU / GAME MENU TRANSITIONS
        /// <summary>
        /// Function call by buttons (play and return Menu)
        /// Basicly switch between MAIN_MENU and GAME_MENU
        /// </summary>
        /// <param name="newState">MainState: MAIN_MENU / GAME_MENU</param>
        private void SetMainState(GameState newState)
        {
            var currInstance = GameManager.Instance;
            gameState = newState;
            Debug.Log($"Main State: {gameState}");
            // LOAD SCENE
            if (gameState == GameState.GAME_MENU)
            {
                currInstance.LoadGameMenu();
                // Set level 
                SetLevel(currentLevel, currInstance.LevelsData.patientByLevel[(int)currentLevel]);
                // Set Patient case
                SetPatientCase(currentPatientCase, currInstance.LevelsData.patientByLevel[(int)currentLevel].patientsCase[(int)currentPatientCase]);
            }
            else
            {
                currInstance.LoadMainMenu();
            }
        }

        /// <summary>
        /// Public function change the main state between MAIN_MENU & GAME_MENU.
        /// Its call by buttons "Play" & "return menu"
        /// </summary>
        public void ChangeMainState()
        {
            if (gameState == GameState.MAIN_MENU)
            {
                SetMainState(GameState.GAME_MENU);
            }
            else
            {
                SetMainState(GameState.MAIN_MENU);
            }
        }

        public void SetLevel(LevelState levelState, PatientCaseData patientData)
        {
            currentLevel = levelState;
            patientCaseData = patientData;

            GameManager.Instance.GameData.SetLevelRecords(currentLevel);

            Debug.Log($"Current Level : {currentLevel}, {patientCaseData.levelName}");
        }

        public int GetCurrentLevel() { return (int) currentLevel; }

        public void SetPatientCase(PatientCase patientCase, NewPatientData newPatient)
        {
            currentPatientCase = patientCase;
            patientData = newPatient;

            GameManager.Instance.GameData.SetPatientCaseRecorder(patientData.fisrtName);
            
            Debug.Log($"Current Patient: {currentPatientCase}, {patientData.surname}");
        }

        public int GetCurrentPatientCase() { return (int)currentPatientCase; }

        public void SetStep(Step step)
        {
            currentStep = step;
            Debug.Log($"Algo Test G: {currentStep}");
            
            GameManager.Instance.GameData.SetStepRecords(currentStep);

            GameManager.Instance.LoadStep(currentStep);
        }

        public void NextLevel()
        {           
            if ((int)currentLevel < LevelsData.patientByLevel.Count)
            {
                SetLevel(currentLevel, patientCaseData);
                //Return to game menu
                ReturnToGameMenu();
            }
            else
            {
                Debug.Log("Tout les niveau sont terminer ! Restart du jeu!");
                // TAMPORARY FIX - Restart the game
                currentLevel = LevelState.LEVEL_0;
                currentPatientCase = PatientCase.PATIENT_0;
                
                //SET RANDOM MOD (load patient in random make list of all patient)
                //Return to game menu
                ReturnToGameMenu();
            }    
        }

        public void NextPatientCase()
        {
            currentPatientCase++;
            if ((int)currentPatientCase < patientCaseData.patientsCase.Count )
            {
                SetPatientCase(currentPatientCase, patientCaseData.patientsCase[(int)currentPatientCase]);
                ReturnToGameMenu();
            }
            else
            {
                Debug.Log("Tous les cas patient sont terminer! Next Level !");
                currentLevel++;
                NextLevel();
            }
        }

        public void NextStep(AlgoStep step)
        {
            currentStep = step.type;
            Debug.Log("Next Level: " + currentStep);
            SetStep(currentStep);
        }

        public void SaveShowScores()
        {
            Debug.Log("Level completed ! ");
            // Saving player data
            SavePlayerData();
            // Load resume screen - Same
            GameManager.Instance.LoadScore(patientData.fisrtName);
        }


        private void SavePlayerData()
        {
            GameManager.Instance.GameData.RecordsPatientCase(patientData.fisrtName);
            GameManager.Instance.GameData.RecordsLevel(currentLevel);
            GameManager.Instance.GameData.UpdateMainRecordsOnLevelEnd();
        }

        // CALL FORM 'PatientScoreManager' BY 'GoToMenu' FUNCTION
        private static void ReturnToGameMenu()
        {
            //return Game menu selection patient
            GameManager.Instance.LoadGameMenu();
            GameManager.Instance.AudioManager.PlayBGM("skyline");
        }

        public void LoadPlayerSaveStates(LevelState savedLevelState, PatientCase savedPatientCase)
        {
            if ((int)savedLevelState <= GameManager.Instance.LevelsData.patientByLevel.Count)
            {
                currentLevel = savedLevelState;

                // WARNING : if cond not good
                if ((int) savedPatientCase < GameManager.Instance.LevelsData.patientByLevel[(int)currentLevel].patientsCase.Count - 1)
                {
                    currentPatientCase = savedPatientCase + 1;
                }
                else
                {
                    // TO CHANGE : Load next level & patientCase = 0
                    currentPatientCase = savedPatientCase;
                }
            }

            // WARNING
            //GameManager.Instance._defaultLoadLevel = (int)currentLevel;
            //GameManager.Instance._defaultPatientCase = (int)currentPatientCase;

            Debug.Log($"Last level played : {currentLevel}, last patient played: {currentPatientCase}");
        } 

        private void Start()
        {
            LevelsData = GameManager.Instance.LevelsData;

            // Set by default current level and current patient case (change later if player has a save)
            currentLevel = LevelState.LEVEL_0;
            currentPatientCase = PatientCase.PATIENT_0;
        }

        /*
        //MAIN_MENU -> PLAY -> CHANGE MAIN STATE -> GAME_MENU
        //GAME_MENU -> ESC -> CHANGE MAIN STATE -> MAIN_MENU

        //GAME_MENU -> GrandMa -> LOAD CURRENT_LEVEL -> LOAD STEP

        //Structure
        private struct AlgoStateData
        {
            public int attempts;
            public int actionsErrors;
            public int diagnoticsErrors;

            public AlgoStateData(int attempts, int actionsErrors, int diagnoticsErrors)
            {
                this.attempts = attempts;
                this.actionsErrors = actionsErrors;
                this.diagnoticsErrors = diagnoticsErrors;
            }
        }

        //Enums for states
        private enum MainState { MAIN_MENU, GAME_MENU } // Enums for Main_menu and waiting_room
        public enum LevelState { LEVEL_0, LEVEL_1, LEVEL_2, LEVEL_3, LEVEL_4, RANDOMGAME } // Enums levels of the game and random game is load when player finish all the levels 
        public enum AlgoState { NONE, WISPER_TEST, QUESTIONARY, VIDEO_OTOSCOPIE, WEBER_TEST, AUDIOMETRI } // Enums algorithm steps

        //Current states
        private MainState currentMainState;
        private LevelState currentLevelState;
        private AlgoState currentAlgoState;

        // New variables 
        private Step currentStep;
        
        private Dictionary<String, List<AlgoStep>> dataLevels2;
        private string currentLevelStateG;
        private List<AlgoStep> stepsToDo;

        // Variables
        private readonly Dictionary<LevelState, bool> levelCompletion = new Dictionary<LevelState, bool>();
        private readonly Dictionary<AlgoState, AlgoStateData> algoStats = new Dictionary<AlgoState, AlgoStateData>();
        private Dictionary<LevelState, List<AlgoState>> dataLevels;
        private List<AlgoState> testsToDo;
        private int testIndex = 0;
        private bool canGetNextStep = false;


        // NEW VARIABLES
        [SerializeField] private List<LevelsData> levelsDatas;
        private NewPatientData patientCaseData;
        private int _currentLevel;
        private int _currentPatientCase;

        // MAIN MENU / GAME MENU TRANSITIONS
        /// <summary>
        /// Function call by buttons (play and return Menu)
        /// Basicly switch between MAIN_MENU and GAME_MENU
        /// </summary>
        /// <param name="newState">MainState: MAIN_MENU / GAME_MENU</param>
        private void SetMainState(MainState newState)
        {
            currentMainState = newState;
            Debug.Log($"Main State: {currentMainState}");
            // LOAD SCENE
            if (currentMainState == MainState.GAME_MENU)
            {
                GameManager.Instance.LoadGameMenu();
            }
            else
            {
                GameManager.Instance.LoadMainMenu();
            }
        }

        // LEVEL SELECTION
        /// <summary>
        /// Select a scenario (AlgoState) linked to the currentLevelState.
        /// </summary>
        /// <param name="newState">currentLevelState</param>
        private void SetLevelState(LevelState newState)
        {
            currentLevelState = newState;
            Debug.Log($"Level State: {currentLevelState}");

            dataLevels = new Dictionary<LevelState, List<AlgoState>>() 
            {
                { LevelState.LEVEL_0, new List<AlgoState> { AlgoState.WISPER_TEST } },
                { LevelState.LEVEL_1, new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY } },
                { LevelState.LEVEL_2, new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY, AlgoState.VIDEO_OTOSCOPIE } },
                { LevelState.LEVEL_3, new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY, AlgoState.VIDEO_OTOSCOPIE, AlgoState.WEBER_TEST } },
                { LevelState.LEVEL_4, new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY, AlgoState.VIDEO_OTOSCOPIE, AlgoState.WEBER_TEST, AlgoState.AUDIOMETRI } },
                { LevelState.RANDOMGAME, new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY, AlgoState.VIDEO_OTOSCOPIE, AlgoState.WEBER_TEST, AlgoState.AUDIOMETRI } }
            };

            testsToDo = dataLevels[newState];
            if (newState == LevelState.RANDOMGAME) GetRandomAlgoStateList();

            GameManager.Instance.GameData.SetLevelRecords(currentLevelState);

            testIndex = 0;
            SetAlgoState(testsToDo[testIndex]);
        }

        private void SetLevelG(string newLevelG)
        {
            currentLevelStateG = newLevelG;
            Debug.Log($"Level State G: {currentLevelStateG}");
            stepsToDo = dataLevels2[currentLevelStateG];
            // add random case in the future
            // GameManager.Instance.GameData.SetLevelRecordsG(currentLevelStateG); // TODO save records
            testIndex = 0;
            //Debug.Log(stepsToDo[testIndex].type);
            SetStepState(stepsToDo[testIndex].type);
        }

        // RANDOM GAME MODE
        /// <summary>
        /// Create a senario based on list of AlgoState which conatins the full algorithm Audiocop (Wisper_test, questionary go /no-go, video otoscopie, weber_test and audiometri) to parcour.
        /// Its remove AlgoState from the right to the left.
        /// </summary>
        /// <remarks>Its change private variable testsToDo which contains the AlgoStates that player have to do.</remarks>
        private void GetRandomAlgoStateList()
        {
            int removeCount = UnityEngine.Random.Range(0, testsToDo.Count);
            testsToDo.RemoveRange(testsToDo.Count - removeCount, removeCount);
            Debug.Log(string.Join(", ", testsToDo));
        }

        // LOAD CURRENT ALGO STEP
        /// <summary>
        /// Load the current algo state (currentAlgoState) scene.
        /// </summary>
        /// <remarks>
        /// For each steps of Audicop algorithm, its attach AlgoStateData strucuture with contain attempts, actionError and diagnoticsErrors.
        /// </remarks>
        /// <param name="newState">AlgoSate</param>
        private void SetAlgoState(AlgoState newState)
        {

            //if (!algoStats.ContainsKey(newState)) algoStats[newState] = new AlgoStateData(0, 0, 0);
            currentAlgoState = newState;

            GameManager.Instance.GameData.SetStepRecords(currentAlgoState);

            if (currentAlgoState != AlgoState.NONE) GameManager.Instance.LoadStep((int)currentAlgoState - 1);
            Debug.Log($"Algo Test: {currentAlgoState}");
            //SaveGame();
        }

        private void SetStepState(Step newStep)
        {
            currentStep = newStep;
            //Todo : set records Step            
            GameManager.Instance.LoadStep((int) currentStep);
            Debug.Log($"Algo Test G: {currentStep}, {(int) currentStep}");
        }

        // RECORD ATTEMPT OF ALGO STEP - CALL WHEN PLAYER VALIDATE ITS CHOICES
        /// <summary>
        /// Record attemps player during his parcours of Audicop algorithm.
        /// </summary>
        /// <remarks>
        /// Take in enter if the player have succed his choice about its actions and diagnotics choice.
        /// </remarks>
        /// <param name="actionsSucces">Boolean</param>
        /// <param name="diagnoticsSucces">Boolean</param>
        private void RecordAttempt(bool actionsSucces, bool diagnoticsSucces)
        {
            if (!algoStats.ContainsKey(currentAlgoState)) return;

            AlgoStateData data = algoStats[currentAlgoState];
            data.attempts++;

            if (!actionsSucces) data.actionsErrors++;
            if (!diagnoticsSucces) data.diagnoticsErrors++;
            algoStats[currentAlgoState] = data;
            Debug.Log("Record : "+ currentAlgoState);

            CheckLevelCompletion();
        }

        // CHECK IF LEVEL IS COMPLETE
        /// <summary>
        /// Chech if player have complete all the steps of AlgoState for each levels.
        /// </summary>
        private void CheckLevelCompletion()
        {
            if (testsToDo.All(test => GameManager.Instance.GameData.PlayerHasAttemptStep(test)))
            {
                levelCompletion[currentLevelState] = true;
            }
        }


        /// <summary>
        /// Call by when player press "Next" button. Load next steps of AlgoState or if the level is ended return (load) to waiting_room.
        /// </summary>
        private void ProgressToNextState()
        {
            canGetNextStep = GameManager.Instance.StepManager.WasCorrectlyAnswered;

            if (testIndex < testsToDo.Count - 1 && canGetNextStep)
            {
                testIndex++;
                SetAlgoState(testsToDo[testIndex]);
            }
            else if (levelCompletion[currentLevelState])
            {
                
                Debug.Log("Level Completed!");
                // Save data
                GameManager.Instance.AudioManager.PlaySFX("money_up");
                GameManager.Instance.GameData.RecordsLevels(currentLevelState);
                GameManager.Instance.GameData.GlobalRecordsOnLevelEnd();
                // return WAITING_ROOM
                GameManager.Instance.LoadGameMenu();
                GameManager.Instance.AudioManager.PlayBGM("skyline");
                SetNextLevel();
            }
        }

        private void ProgressToNextStateG()
        {
            if (testIndex < stepsToDo.Count - 1)
            {
                testIndex++;
                SetStepState(stepsToDo[testIndex].type);
            }
            else if (stepsToDo[testIndex].isTerminatingStep)
            {
                Debug.Log("Level Completed !");
                //save data
                //return selection level
                GameManager.Instance.LoadGameMenu();
                GameManager.Instance.AudioManager.PlayBGM("skyline");
            }
        }

        /// <summary>
        /// Call when player have ended the current level and feed next level. When the player ended all the level we load randomgame level.
        /// </summary>
        private void SetNextLevel()
        {
            if (currentLevelState == LevelState.LEVEL_0)
            {
                currentLevelState = LevelState.LEVEL_1;
            }
            else if (currentLevelState == LevelState.LEVEL_1)
            {
                currentLevelState = LevelState.LEVEL_2;
            }
            else if (currentLevelState == LevelState.LEVEL_2)
            {
                currentLevelState = LevelState.LEVEL_3;
            }
            else
            {
                Debug.Log("All levels completed");
                currentLevelState = LevelState.RANDOMGAME;
            }
        }

        // GET STATE PROGRESS
        public LevelState GetCurrentLevelState() => this.currentLevelState;
        public AlgoState GetCurrentAlgoState() => this.currentAlgoState;    

        /// <summary>
        /// Public function change the main state between MAIN_MENU & GAME_MENU.
        /// Its call by buttons "Play" & "return menu"
        /// </summary>
        public void ChangeMainState()
        {
            if (currentMainState == MainState.MAIN_MENU)
            {
                SetMainState(MainState.GAME_MENU);
            }
            else
            {
                SetMainState(MainState.MAIN_MENU);
            }
        }

        /// <summary>
        /// Public function that load the current level when GrandMa or GrandPa Click
        /// </summary>
        public void LoadLevelState()
        {
            if (currentMainState == MainState.GAME_MENU) SetLevelState(currentLevelState);
        }

        public void LoadLevelStateG(string nameLevel)
        {
            if (currentMainState == MainState.GAME_MENU) SetLevelG(nameLevel);
        }

        /// <summary>
        /// Public function that load the level when player click on the button form level selection panel.
        /// </summary>
        /// <param name="level"></param>
        public void LoadLevelFromPanel(int level)
        {
            if (level > Enum.GetValues(typeof(LevelState)).Length)
            {
                Debug.LogError("Level not found. Add new level to levels state.");
                return;
            }
            SetLevelState((LevelState)level);
        }

        /// <summary>
        /// Public fonction call when player hit "next" button. Goes the next steps of AlgoState.
        /// </summary>
        public void GetNextStep()
        {
            //ProgressToNextState();
            ProgressToNextStateG();
        }

        /// <summary>
        /// Public fonction call when player hit "validate" button. Record attempt's player.
        /// </summary>
        /// <param name="actionError"></param>
        /// <param name="diagnoticsError"></param>
        public void RegiterError(String actionError, String diagnoticsError)
        {
            //RecordAttempt(actionError, diagnoticsError);
            GameManager.Instance.GameData.RecordsSteps(currentAlgoState, actionError, diagnoticsError);
            CheckLevelCompletion();
        }

        /// <summary>
        /// Public fonction call when the game intialize the patient in the StepsManager.cs
        /// </summary>
        /// <returns>int: Number of steps</returns>
        public int GetNumberSteps()
        {
            return this.testsToDo.Count - 1;
        }

        /// <summary>
        /// Set the currentLevelState on loading game with a saved LevelState from GameData.
        /// </summary>
        /// <param name="levelState"></param>
        public void LoadSavedLevel(LevelState levelState)
        {
            if (levelState < LevelState.RANDOMGAME)
            {
                currentLevelState = levelState + 1;
            }
            else
            {
                currentLevelState = LevelState.RANDOMGAME;
            }
        }

        private void Start()
        {
            //LoadGame();
            dataLevels2 = new Dictionary<string, List<AlgoStep>>();

            // Load player progression

        }*/
    }
}