using SwordGC.AirController;
using UnityEngine.EventSystems;

public class AirControllerInputModule : BaseInputModule
{

    public override void ActivateModule()
    {
        base.ActivateModule();

        var toSelect = eventSystem.currentSelectedGameObject;

        if (toSelect == null)
        {
            toSelect = eventSystem.firstSelectedGameObject;
        }

        eventSystem.SetSelectedGameObject(toSelect, GetBaseEventData());
    }

    public override void DeactivateModule()
    {
        base.DeactivateModule();
    }

    public override void Process()
    {
        ProcessMovement();
        ProcessButtons();
        AirController.Instance.ResetInput();
    }

    void ProcessMovement()
    {
        foreach (Player p in AirController.Instance.Players.Values)
        {
            // Get the axis move event
            var axisEventData = GetAxisEventData(p.Input.GetHorizontalAxis("ui-move"), p.Input.GetVerticalAxis("ui-move"), 0.6f);
            if (axisEventData.moveDir == MoveDirection.None)
            {
                continue; // input vector was not enough to move this cycle, done
            }

            ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, axisEventData, ExecuteEvents.moveHandler);
        }
    }

    void ProcessButtons()
    {
        foreach (Player p in AirController.Instance.Players.Values)
        {
            if (eventSystem.currentSelectedGameObject == null)
            {
                return;
            }

            var data = GetBaseEventData();

            if (p.Input.GetKeyDown("ui-submit"))
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.submitHandler);
            }

            if (p.Input.GetKeyDown("ui-cancel"))
            {
                ExecuteEvents.Execute(eventSystem.currentSelectedGameObject, data, ExecuteEvents.cancelHandler);
            }
        }
    }
}
