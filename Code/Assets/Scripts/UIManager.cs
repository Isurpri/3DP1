using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public TextMeshProUGUI m_AmmoText;

    private void Awake()
    {
        Instance = this;
    }

    public void AmmoUI(float chargerAmmo, float totalAmmo)
    {
        if (m_AmmoText != null)
        {
            m_AmmoText.text = $"{chargerAmmo} / {totalAmmo}";
        }
    }
}
