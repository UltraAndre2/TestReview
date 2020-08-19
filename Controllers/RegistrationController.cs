using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegistrationController : MonoBehaviour
{
    [Header("References")]
    public InputField Email;
    public InputField Password;
    public Toggle TermsOfUse;
    public Toggle PrivacyPolicy;
    public Button TermsOfUseButton;
    public Button PrivacyPolicyButton;


    public Button Submit;
    
    public void Start()
    {
        TermsOfUseButton.onClick.AddListener(() =>
        {
            FrameSetController.GetInstance().OpenFrame(FrameSet.AGREEMENT).GetComponent<AgreementController>().Initialize(AgreementType.TermsOfUse, () => { });
        });

        PrivacyPolicyButton.onClick.AddListener(() =>
        {
            FrameSetController.GetInstance().OpenFrame(FrameSet.AGREEMENT).GetComponent<AgreementController>().Initialize(AgreementType.PrivacyPolicy, () => { });
        });

        new List<Toggle>() { PrivacyPolicy, TermsOfUse }.ForEach(x =>
        {
            x.onValueChanged.AddListener((@checked) =>
            {
                if (!new List<Toggle>() { PrivacyPolicy, TermsOfUse }.Exists(y => y.isOn))
                    Submit.interactable = true;
                else Submit.interactable = false;
            });
        });

        Submit.onClick.AddListener(() =>
        {
            Submit.GetComponent<ButtonLoader>().StartLoading();

            RequestManager.Patient.Registration(new Dictionary<string, string>() {
                { "email", Email.text },
                { "password", Password.text },
                { "confirmation", Password.text },
                { "policy", true.ToString() }
            },
            response =>
            {
                TokenManager.Token = response.data.attributes.token;

                FrameSetController.GetInstance().OpenFrame(FrameSet.PROFILE);
            },
            error => {
                Submit.GetComponent<ButtonLoader>().StopLoading();
            });
        });
    }
}
