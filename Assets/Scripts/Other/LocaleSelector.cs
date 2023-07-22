using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class LocaleSelector : MonoBehaviour {
    bool active = false;
    int index = 1;
    public static event Action onLanguageChanged;

    public void ChangeLocale() {
        if (active == true) {
            return;
        }
        index++;
        int localeID = Mathf.Abs(index) % LocalizationSettings.AvailableLocales.Locales.Count;
        StartCoroutine(SetLocale(localeID));
    }

    IEnumerator SetLocale(int localeID) {
        active = true;
        yield return LocalizationSettings.InitializationOperation;
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[localeID];
        onLanguageChanged?.Invoke();
        active = false;
    }
}
