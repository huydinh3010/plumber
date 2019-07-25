using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public interface IPopup
{
    void Show(Dictionary<PopupButtonEvent, Action> list_actions, Dictionary<PopupSettingType, object> list_settings);
    void OnDisplayed();
    void OnClosed();
}
