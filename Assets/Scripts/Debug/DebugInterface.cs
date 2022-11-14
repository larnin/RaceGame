using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class DebugInterface : MonoBehaviour
{
    enum WindowType
    {
        Window_Quest,
        Window_Inventory,
    }

    class WindowInfo
    {
        public string name;
        public int id;
        public Rect rect;
        public bool enabled;

        public WindowInfo(int _id, string _name)
        {
            name = _name;
            id = _id;
            rect = new Rect(0, 0, 200, 200);
            enabled = true;
        }
    }

    bool m_enabled = false;

    SubscriberList m_subscriberList = new SubscriberList();

    List<WindowInfo> m_windows = new List<WindowInfo>();

    private void Awake()
    {
        m_subscriberList.Add(new Event<IsDebugEnabledEvent>.Subscriber(DebugEnabled));
        m_subscriberList.Subscribe();

        int nbWindow = Enum.GetValues(typeof(WindowType)).Length;
        m_windows.Clear();
        for (int i = 0; i < nbWindow; i++)
            m_windows.Add(new WindowInfo(i, ((WindowType)i).ToString()));
    }

    private void OnDestroy()
    {
        m_subscriberList.Unsubscribe();
    }

    private void Update()
    {
        if (Keyboard.current.f5Key.wasPressedThisFrame)
            m_enabled = !m_enabled;
    }

    private void OnGUI()
    {
        if (!m_enabled)
            return;

        foreach(var w in m_windows)
        {
            if (w.enabled)
            {
                w.rect = GUI.Window(w.id, w.rect, DrawWindow, w.name);
            }
        }
    }

    void DebugEnabled(IsDebugEnabledEvent e)
    {
        e.enabled = m_enabled;
    }

    void DrawWindow(int id)
    {
        GUI.DragWindow(new Rect(0, 0, 10000, 10000));
    }
}