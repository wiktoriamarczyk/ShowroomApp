using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour {
    private bool active = false;
    int index = 1;

    public void ChangeLocale() {
        if (active == true)
            return;

        index++;
        int localeID = Mathf.Abs(index) % LocalizationSettings.AvailableLocales.Locales.Count;
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int localeID) {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        active = false;
    }
}
