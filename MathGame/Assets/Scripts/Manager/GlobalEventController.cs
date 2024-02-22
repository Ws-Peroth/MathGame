using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalEventController : Singleton<GlobalEventController>
{
    private Dictionary<string, List<object>> eventReceivers = new();

    public void SendEvent(string key, string name, object[] param = null)
    {
        if (eventReceivers.ContainsKey(key))
        {
            var receivers = eventReceivers[key];
            foreach (var receiver in receivers)
            {
                if (receiver is IGlobalEventReceiver Interface)
                {
                    Interface.ReceiveEvent(key, name, param);
                }
                else
                {
                    Debug.LogError("[GlobalEventController] :: receiver is not IGlobalEventReceiver Interface");
                }
            }
        }
    }

    public void Regist(IGlobalEventReceiver eventReceiver, string key)
    {
        if (!eventReceivers.ContainsKey(key))
        {
            List<object> Receivers = new() { eventReceiver.GetOriginObject() };
            eventReceivers.Add(key, Receivers);
            return;
        }
        if (!eventReceivers[key].Contains(eventReceiver.GetOriginObject()))
        {
            eventReceivers[key].Add(eventReceiver.GetOriginObject());
            return;
        }
    }

    public void Unregist(IGlobalEventReceiver observeInstance, string key)
    {
        Debug.Log($"Call Unregist{key}");

        if (eventReceivers.ContainsKey(key))
        {
            var Receivers = eventReceivers[key];
            if (Receivers.Contains(observeInstance.GetOriginObject()))
            {
                var compareObject = observeInstance.GetOriginObject();
                int index = Receivers.FindIndex((match) => match == compareObject);
                Receivers.RemoveAt(index);

                if (Receivers.Count == 0)
                {
                    eventReceivers.Remove(key);
                }
            }
        }
    }

    protected override void OnDestroy()
    {
        Debug.Log("Destroy GlobalEventController");
        base.OnDestroy();
    }

    ~GlobalEventController()
    {
        Debug.Log($"~GlobalEventController:: Receivers Count = {eventReceivers.Count}");
    }
}