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
    public void RestartLevel()
    {
        for (int i = 0; i < m_DestroyObjects.childCount; ++i)
        {
            GameObject.Destroy(m_DestroyObjects.GetChild(i).gameObject);
        }
        m_Player.Restart();
    }
    //para cambiar entre escenas
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            SceneManager.LoadSceneAsync("VictorScene");
        if (Input.GetKeyDown(KeyCode.O))
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
