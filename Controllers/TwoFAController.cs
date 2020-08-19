using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class TwoFAController : MonoBehaviour
{
    [Header("References")]
    public Button SubmitButton;
    public InputField CodeInputField;

    public void Start()
    {
        SubmitButton.interactable = false;

        CodeInputField.onValueChange.AddListener((text) =>
        {
            SubmitButton.interactable = text.Length == 4;
        });

        SubmitButton.onClick.AddListener(() =>
        {
            SubmitButton.GetComponent<ButtonLoader>().StartLoading();

            RequestManager.User.TwoFA.Verify(new Dictionary<string, string>() {
                { "2fa_code", CodeInputField.text }
            },
            response =>
            {
                onComplete.Invoke();
                Destroy(gameObject);
            },
            error =>
            {
                SubmitButton.GetComponent<ButtonLoader>().StopLoading();
                Message.Debug(error.error_description);
            });
        });
    }

    private Action onComplete;
    public void OnComplete(Action onComplete) {
        this.onComplete = onComplete;
    }
}
