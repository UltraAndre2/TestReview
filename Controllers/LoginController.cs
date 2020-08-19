using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginController : MonoBehaviour
{
    [Header("References")]
    public FormController Form;
    public InputField Email;
    public InputField Password;
    public Button Submit;
    public Button RegistrationButton;
    
    public void Start()
    {
        Submit.onClick.AddListener(() =>

        {
            Submit.GetComponent<ButtonLoader>().StartLoading();

            RequestManager.Patient.Authorize(new Dictionary<string, string>() {
            { "email", Email.text },
            { "password", Password.text }
            },
            response =>
            {
                TokenManager.Token = response.data.attributes.token;

                Debug.Log(response.rawData);

                RequestManager.Patient.Profile(_response =>
                {
                    RequestManager.User.TwoFA.Request(
                        __response =>
                        {
                            PrefabsController.OpenPrefabUI("2FA").GetComponent<TwoFAController>().OnComplete(() =>
                            {
                                Submit.GetComponent<ButtonLoader>().StopLoading();
                                FrameSetController.GetInstance().OpenFrame(FrameSet.PROFILE);
                            });
                        },
                        error =>
                        {
                            Message.Debug("2FA Failed");
                        }
                    );

                },
                error => {
                    Message.CREDENTIALS_WRONG.Show();
                    Form.Clear();

                    Submit.GetComponent<ButtonLoader>().StopLoading();
                });
            },
            error => {
                Message.CREDENTIALS_WRONG.Show();
                Form.Clear();

                Submit.GetComponent<ButtonLoader>().StopLoading();
            });
        });

        RegistrationButton.onClick.AddListener(() =>
        {
            FrameSetController.GetInstance().OpenFrame(FrameSet.REGISTRATION);
        });
    }
}
