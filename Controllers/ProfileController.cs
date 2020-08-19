using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProfileController : MonoBehaviour
{
    [Header("References")]
    public ImageLoader Avatar;
    public Text PatientName;

    public Button LogOut;

    public void Start()
    {
        RequestManager.Patient.Profile(
            response => {
                Avatar.URL = response.data.attributes.photo;
                PatientName.text = response.data.attributes.first_name + " " + response.data.attributes.last_name;
            },
            error => {
                Debug.LogError("Error");
            }
        );

        LogOut.onClick.AddListener(() =>
        {
            FrameSetController.GetInstance().OpenFrame(FrameSet.LOGIN);
        });
    }
}
