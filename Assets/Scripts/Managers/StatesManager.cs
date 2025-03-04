using UnityEngine;

namespace Assets.Scripts.Managers
{
    public class StatesManager : MonoBehaviour
    {
        internal enum States
        {
            MAIN_MENU,
            GAME_MENU,
            GAME_E0,
            GAME_E1,
            GAME_E2,
            GAME_E3,
            GAME_E4
        }

        internal States currentState;

        internal bool paused;
        private bool canChangeState = false;

        public void ChangeState()
        {
            if (currentState == States.MAIN_MENU)
            {
                canChangeState = true;
                return;
            }
            if (currentState == States.GAME_MENU)
            {
                canChangeState = true;
                return;
            }
            canChangeState = GameManager.Instance.StepManager.WasCorrectlyAnswered;
        }

        public void ReturnMainMenu()
        {
            paused = true;
            currentState = States.MAIN_MENU;
            DoActionOnChangeState();
        }

        private void UpdateStates()
        {
            switch (currentState)
            {
                case States.MAIN_MENU:
                case States.GAME_MENU:
                    if (canChangeState)
                    {
                        if (currentState != States.MAIN_MENU) GameManager.Instance.AudioManager.PlayBGM("skyline");
                        currentState++;
                        DoActionOnChangeState();
                    }
                    break;
                case States.GAME_E0:
                case States.GAME_E1:
                case States.GAME_E2:
                case States.GAME_E3:
                    if (canChangeState)
                    {
                        if (GameManager.Instance.StepManager.IsLastStep())
                        {
                            GameManager.Instance.AudioManager.PlaySFX("money_up");
                            GameManager.Instance.TelemetryManager.IncrWins();
                            currentState = States.GAME_MENU;
                            GameManager.Instance.AudioManager.PlayBGM("skyline");
                        }
                        else currentState++;
                        DoActionOnChangeState();
                    }
                    break;
                case States.GAME_E4:
                    if (canChangeState)
                    {
                        GameManager.Instance.AudioManager.PlaySFX("money_up");
                        GameManager.Instance.TelemetryManager.IncrWins();
                        currentState = States.GAME_MENU;
                        GameManager.Instance.AudioManager.PlayBGM("skyline");
                        DoActionOnChangeState();
                    }
                    break;
                    //(States.GAME_MENU -> States.MAIN_MENU) see ReturnMainMenu method
            }
            if (currentState == States.MAIN_MENU) paused = true;
        }

        private void DoActionOnChangeState()
        {
            canChangeState = false;
            switch (currentState)
            {
                case States.MAIN_MENU:
                    GameManager.Instance.LoadMainMenu();
                    break;
                case States.GAME_MENU:
                    GameManager.Instance.LoadGameMenu();
                    break;
                case States.GAME_E0:
                    GameManager.Instance.LoadStep(0);
                    break;
                case States.GAME_E1:
                    GameManager.Instance.LoadStep(1);
                    break;
                case States.GAME_E2:
                    GameManager.Instance.LoadStep(2);
                    break;
                case States.GAME_E3:
                    GameManager.Instance.LoadStep(3);
                    break;
                case States.GAME_E4:
                    GameManager.Instance.LoadStep(4);
                    break;
            }
        }

        private void DoActionOnState()
        {
            canChangeState = false;
            switch (currentState)
            {
                case States.MAIN_MENU:
                    break;
                case States.GAME_MENU:
                    break;
                case States.GAME_E0:
                    break;
                case States.GAME_E1:
                    break;
                case States.GAME_E2:
                    break;
                case States.GAME_E3:
                    break;
                case States.GAME_E4:
                    break;
            }
        }

        void FixedUpdate()
        {
            if (!paused || currentState == States.MAIN_MENU)
            {
                UpdateStates();
                DoActionOnState();
            }
        }
    }
}