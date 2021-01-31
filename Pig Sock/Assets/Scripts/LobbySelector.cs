using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbySelector : MonoBehaviour
{
    public PlayerData data;
    public Button typeToggleButton;
    public TMP_InputField nameInput;
    public TextMeshProUGUI typeLabel;
    public Color[] btnColors;
    public CanvasGroup cg;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
    }

    // Start is called before the first frame update
    void Start()
    {
        //initial playerdata set in inspector.
        typeToggleButton.onClick.AddListener(ToggleType);
        UpdateGraphics();
    }

    public void SetType(PlayerData.Type type)
    {
        data.type = type;
        UpdateGraphics();
    }

    void ToggleType()
    {
        if (data.type == PlayerData.Type.Human)
            data.type = PlayerData.Type.Off;
        else if (data.type == PlayerData.Type.CPU)
            data.type = PlayerData.Type.Human;
        else if (data.type == PlayerData.Type.Off)
            data.type = PlayerData.Type.CPU;

        UpdateGraphics();

    }

    void UpdateGraphics()
    {
        typeLabel.text = data.type.ToString();
        typeToggleButton.image.color = btnColors[(int)data.type];

        if (data.type == PlayerData.Type.Off)
        {
            nameInput.gameObject.SetActive(false);
            cg.alpha = 0.5f;
        }
        else
        {
            nameInput.gameObject.SetActive(true);
            cg.alpha = 1f;
        }
    }
}
