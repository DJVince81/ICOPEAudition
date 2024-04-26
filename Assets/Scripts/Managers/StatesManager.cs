using UnityEngine;

public class StatesManager : MonoBehaviour
{
    private enum States
    {
        MAIN_MENU,
        GAME_E0,
        GAME_E1,
        GAME_E2,
        GAME_E3,
        GAME_E4
    }

    private States State;

    internal bool paused;
    private bool isOk;

    public void ChangeState()
    {
        if (State == States.MAIN_MENU)
        {
            isOk = true;
            return;
        }
        isOk = GameManager.Instance.StepManager.IsStepCorrect((int)State -1);
    }

    public void ReturnMainMenu()
    {
        paused = true;
        State = States.MAIN_MENU;
        DoActionOnChangeState();
    }

    private void UpdateStates()
    {
        switch (State)
        {
            case States.MAIN_MENU:
                State = States.GAME_E0;
                DoActionOnChangeState();
                break;
            case States.GAME_E0:
                if (isOk)
                {
                    State = States.GAME_E1;
                    DoActionOnChangeState();
                }
                break;
            case States.GAME_E1:
                if (isOk)
                {
                    State = States.GAME_E2;
                    DoActionOnChangeState();
                }
                break;
            case States.GAME_E2:
                if (isOk)
                {
                    State = States.GAME_E3;
                    DoActionOnChangeState();
                }
                break;
            case States.GAME_E3:
                if (isOk)
                {
                    State = States.GAME_E4;
                    DoActionOnChangeState();
                }
                break;
            case States.GAME_E4:
                if (isOk)
                {
                    State = States.MAIN_MENU;
                    DoActionOnChangeState();
                }
                break;
        }
        if (State == States.MAIN_MENU) paused = true;
    }

    private void DoActionOnChangeState()
    {
        isOk = false;
        switch (State)
        {
            case States.MAIN_MENU:
                GameManager.Instance.LoadMainMenu();
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
        isOk = false;
        switch (State)
        {
            case States.MAIN_MENU:
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

    void Update()
    {
        if (!paused)
        {
            UpdateStates();
            DoActionOnState();
        }
    }
}
