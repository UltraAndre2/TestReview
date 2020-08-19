using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class FrameSet {
    public const string LOGIN = "Login";
    public const string PROFILE = "Profile";
    public const string AGREEMENT = "Agreement";
    public const string REGISTRATION = "Registration";
}

public class FrameSetController : MonoBehaviour
{
    private static FrameSetController _Instance = null;
    public void Awake()
    {
        _Instance = this;
    }

    public static FrameSetController GetInstance()
    {
        return _Instance;
    }

    [Header("Prefabs")]
    public List<GameObject> FrameSets;

    [Header("Parameters")]
    public bool FrameCaching;

    [Header("Variables")]
    public Dictionary<string, GameObject> ActiveFrames;

    public void Start()
    {
        ActiveFrames = new Dictionary<string, GameObject>();

        OpenFrame(FrameSet.LOGIN);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            LoadPreviousFrame();
    }

    public void LoadPreviousFrame() {
        // Check if history has enough frames
        if (FrameHistory.Count < 2)
            return;

        OpenFrame(FrameHistory[FrameHistory.Count - 2], false);
        FrameHistory.RemoveAt(FrameHistory.Count - 1);
    }

    private List<string> FrameHistory = new List<string>();
    public GameObject OpenFrame(string alias, bool historical = true) {
        if (!FrameCaching)
            ActiveFrames.Keys.ToList().ForEach(x =>
            {
                Destroy(ActiveFrames[x]);
                ActiveFrames.Remove(x);
            });

        ActiveFrames.Values.ToList().ForEach(x => x.SetActive(false));

        if (ActiveFrames.ContainsKey(alias))
            ActiveFrames[alias].SetActive(true);
        else ActiveFrames.Add(alias, Instantiate(FrameSets.Find(x => x.name == alias), GameObject.Find("Canvas").transform));

        if (historical)
            FrameHistory.Add(alias);

        return ActiveFrames[alias];
    }
}
