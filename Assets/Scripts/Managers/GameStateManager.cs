using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class GameStateManager : MonoBehaviour
    {
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
        private enum LevelState { LEVEL_0, LEVEL_1, LEVEL_2, LEVEL_3, RANDOMGAME } // Enums levels of the game and random game is load when player finish all the levels 
        private enum AlgoState { NONE, WISPER_TEST, QUESTIONARY, VIDEO_OTOSCOPIE, WEBER_TEST, AUDIOMETRI } // Enums algorithm steps

        //Current states
        private MainState currentMainState;
        private LevelState currentLevelState;
        private AlgoState currentAlgoState;

        // Variables
        private readonly Dictionary<LevelState, bool> levelCompletion = new Dictionary<LevelState, bool>();
        private readonly Dictionary<AlgoState, AlgoStateData> algoStats = new Dictionary<AlgoState, AlgoStateData>();
        private List<AlgoState> testsToDo;
        private int testIndex = 0;
        private bool canGetNextStep = false;

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
        private void SetLevelState(LevelState newState)
        {
            currentLevelState = newState;
            Debug.Log($"Level State: {currentLevelState}");

            switch (currentLevelState)
            {
                case LevelState.LEVEL_0:
                    testsToDo = new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY };
                    break;
                case LevelState.LEVEL_1:
                    testsToDo = new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY, AlgoState.VIDEO_OTOSCOPIE };
                    break;
                case LevelState.LEVEL_2:
                    testsToDo = new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY, AlgoState.VIDEO_OTOSCOPIE, AlgoState.WEBER_TEST };
                    break;
                case LevelState.LEVEL_3:
                    testsToDo = new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY, AlgoState.VIDEO_OTOSCOPIE, AlgoState.WEBER_TEST, AlgoState.AUDIOMETRI };
                    break;
                case LevelState.RANDOMGAME:
                    testsToDo = new List<AlgoState> { AlgoState.WISPER_TEST, AlgoState.QUESTIONARY, AlgoState.VIDEO_OTOSCOPIE, AlgoState.WEBER_TEST, AlgoState.AUDIOMETRI };
                    GetRandomAlgoStateList();
                    break;
            }

            testIndex = 0;
            SetAlgoState(testsToDo[testIndex]);
        }

        // RANDOM GAME MODE
        private void GetRandomAlgoStateList()
        {
            int removeCount = Random.Range(0, testsToDo.Count);
            testsToDo.RemoveRange(testsToDo.Count - removeCount, removeCount);
            Debug.Log(string.Join(", ", testsToDo));
        }

        // LOAD CURRENT ALGO STEP
        private void SetAlgoState(AlgoState newState)
        {

            if (!algoStats.ContainsKey(newState)) algoStats[newState] = new AlgoStateData(0, 0, 0);

            currentAlgoState = newState;
            if (currentAlgoState != AlgoState.NONE) GameManager.Instance.LoadStep((int)currentAlgoState - 1);
            Debug.Log($"Algo Test: {currentAlgoState}");
            //SaveGame();
        }

        // RECORD ATTEMPT OF ALGO STEP - CALL WHEN PLAYER VALIDATE ITS CHOICES
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
        private void CheckLevelCompletion()
        {
            if (testsToDo.All(test => algoStats.ContainsKey(test) && algoStats[test].attempts > 0))
            {
                levelCompletion[currentLevelState] = true;
            }
        }


        /// <summary>
        /// Call by FixedUpdate check if player has correctly answered the question
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
                //return GAME_MENU
                Debug.Log("Level Completed!");
                GameManager.Instance.LoadGameMenu();
                SetNextLevel();
            }
        }

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

        // SAVE PROGRESS - TO CHANGE
        private void SaveGame()
        {
            PlayerPrefs.SetInt("MainSate", (int)currentMainState);
            PlayerPrefs.SetInt("LevelState", (int)currentLevelState);
            PlayerPrefs.SetInt("AlgoState", (int)currentAlgoState);
            PlayerPrefs.Save();
            Debug.Log("Game saved");
        }

        // LOAD PROGRESS -TO CHANGE
        private void LoadGame()
        {
            if (PlayerPrefs.HasKey("MainState"))
            {
                currentMainState = (MainState)PlayerPrefs.GetInt("MainState");
                currentLevelState = (LevelState)PlayerPrefs.GetInt("LevelState");
                currentAlgoState = (AlgoState)PlayerPrefs.GetInt("AlgoState");

                Debug.Log("Game Loaded");
            }
            else
            {
                Debug.Log("No save found");
                SetMainState(MainState.MAIN_MENU);
            }
        }

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

        public void GetNextStep()
        {
            ProgressToNextState();
        }

        public void RegiterError(bool actionError, bool diagnoticsError)
        {
            RecordAttempt(actionError, diagnoticsError);
        }

        public int GetNumberSteps()
        {
            return this.testsToDo.Count - 1;
        }

        //public void ChangeLevelState

        private void Start()
        {
            //LoadGame();
        }
    }
}