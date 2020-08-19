using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum AgreementType { TermsOfUse, PrivacyPolicy};

public class AgreementController : MonoBehaviour
{
    [Header("References")]
    public Button Confirm;
    
    public void Initialize(AgreementType type, Action onComplete) {
        Confirm.onClick.RemoveAllListeners();
        Confirm.onClick.AddListener(() =>
        {
            onComplete.Invoke();
            FrameSetController.GetInstance().LoadPreviousFrame();
        });
    }
}
