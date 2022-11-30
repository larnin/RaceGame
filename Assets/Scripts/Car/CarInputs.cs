using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CarInputs : MonoBehaviour
{
    const string InputsName = "Player";
    const string ForwardName = "Forward";
    const string SteeringName = "Steering";
    const string BoostName = "Boost";

    PlayerInput m_inputs;

    float m_forward;
    float m_steering;
    bool m_boost;

    SubscriberList m_subscriberList = new SubscriberList();

    void Awake()
    {
        m_inputs = GetComponent<PlayerInput>();
        if (m_inputs)
            m_inputs.onActionTriggered += OnInput;

        m_subscriberList.Add(new Event<GetInputsEvent>.LocalSubscriber(GetInputs, gameObject));
        m_subscriberList.Subscribe();
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    void OnInput(InputAction.CallbackContext e)
    {
        if (e.action == null)
            return;
        if (e.action.actionMap == null)
            return;
        if (e.action.actionMap.name != InputsName)
            return;

        //update current device
        string deviceClass = e.control.device.description.deviceClass;
        InputType type = Settings.instance.inputType;

        if (deviceClass == "Mouse" || deviceClass == "Keyboard")
        {
            if (type != InputType.Keyboard)
                Settings.instance.inputType = InputType.Keyboard;
        }
        else if (type != InputType.Gamepad)
            Settings.instance.inputType = InputType.Gamepad;

        if (e.action.name == ForwardName)
        {
            if (e.phase == InputActionPhase.Started || e.phase == InputActionPhase.Performed)
                m_forward = e.ReadValue<float>();
            else if (e.phase == InputActionPhase.Disabled || e.phase == InputActionPhase.Canceled)
                m_forward = 0;
        }
        else if (e.action.name == SteeringName)
        {
            if (e.phase == InputActionPhase.Started || e.phase == InputActionPhase.Performed)
                m_steering = e.ReadValue<float>();
            else if (e.phase == InputActionPhase.Disabled || e.phase == InputActionPhase.Canceled)
                m_steering = 0;
        }
        else if (e.action.name == BoostName)
        {
            if (e.phase == InputActionPhase.Started)
            {
                m_boost = true;
                Event<StartBoostEvent>.Broadcast(new StartBoostEvent(), gameObject, true);
            }
            else if (e.phase == InputActionPhase.Canceled)
            {
                m_boost = false;
                Event<EndBoostEvent>.Broadcast(new EndBoostEvent(), gameObject, true);
            }
        }
    }

    void GetInputs(GetInputsEvent e)
    {
        e.forward = m_forward;
        e.steering = m_steering;
        e.boost = m_boost;
    }
}