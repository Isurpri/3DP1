using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static  GameManager m_GameManager;
    PlayerController m_Player;
    public Transform m_DestroyObjects;
    void Start()
    {
        if (m_GameManager != null)
        {
            GameObject.Destroy(gameObject);
            return;
        }
        m_GameManager = this;
        DontDestroyOnLoad(gameObject);
    }
    public static GameManager GetGameManager()
    {
        return m_GameManager;
    }
    public void ReloadLevel()
    {
        for (int i = 0; i < m_DestroyObjects.childCount; ++i)
        {
            GameObject.Destroy(m_DestroyObjects.GetChild(i).gameObject);
        }
    }
    //para cambiar entre escenas
    private void Updat()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SceneManager.LoadSceneAsync("Level2");
        if (Input.GetKeyDown(KeyCode.Alpha2))
            SceneManager.LoadSceneAsync("SampleScene");
    }
    public PlayerController GetPlayer()
    {
        return m_Player;
    }
    public void SetPlayer(PlayerController Player)
    {
        m_Player = Player;
    }
}
