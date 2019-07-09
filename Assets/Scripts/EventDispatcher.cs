using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventDispatcher : MonoBehaviour
{

    public static EventDispatcher Instance = new EventDispatcher();
    public delegate void Action<in P>(P obj);
    Dictionary<EventID, Action<object>> _listeners = new Dictionary<EventID, Action<object>>();
    private EventDispatcher()
    {

    }


    public void RegisterListener(EventID eventID, Action<object> callback)
    {
        if (_listeners.ContainsKey(eventID))
        {
            _listeners[eventID] += callback;
        }
        else
        {
            _listeners.Add(eventID, null);
            _listeners[eventID] += callback;
        }
    }

    public void PostEvent(EventID eventID, Component sender, object param = null)
    {
        if (_listeners.ContainsKey(eventID))
        {
            var callbacks = _listeners[eventID];
            if(callbacks != null)
            {
                callbacks(param);
            }
        }
    }

    public void RemoveListener(EventID eventID, Action<object> callback)
    {
        if (_listeners.ContainsKey(eventID))
        {
            //Debug.Log("remove: " + eventID.ToString());
            _listeners[eventID] -= callback;
        }
    }

    public void ClearAllListener()
    {
        _listeners.Clear();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
