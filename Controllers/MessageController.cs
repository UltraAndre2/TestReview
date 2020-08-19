using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class MessageController : MonoBehaviour
{
    [Header("References")]
    public Image Icon;
    public Text Header;
    public Text Description;

    public void Start()
    {
        GetComponent<Button>().onClick.AddListener(() => Close());
        TimeUtility.Delay(Message.SHOW_TIME_DELAY, () => Close());
    }

    public void Generate(Message message) {
        Description.text = message.description;
    } 

    public void Close() {
        if (!GetComponent<Animation>().isPlaying)
            GetComponent<Animation>().Play("MessageHide");
    }
}
