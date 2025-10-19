using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameManager m_GameManager;
    PlayerController m_Player;
    public Transform m_DestroyObjects;
    public Fade m_fade;
    public Transform m_PersistentParticlesParent;

    private Vector3? m_LastCheckpointPosition = null;
    private Quaternion? m_LastCheckpointRotation = null;

    void Awake()
    {
        if (m_GameManager != null)
        {
            Destroy(gameObject);
            return;
        }

        m_GameManager = this;
        DontDestroyOnLoad(gameObject);

        if (m_PersistentParticlesParent == null)
        {
            GameObject particlesContainer = GameObject.Find("PersistentParticles");
            if (particlesContainer == null)
                particlesContainer = new GameObject("PersistentParticles");

            particlesContainer.transform.SetParent(transform);
            m_PersistentParticlesParent = particlesContainer.transform;
        }
    }

    public static GameManager GetGameManager()
    {
        return m_GameManager;
    }
    /*public void RestartLevel() 
    { 
        for (int i = 0; i < m_DestroyObjects.childCount; ++i) 
        { 
            GameObject.Destroy(m_DestroyObjects.GetChild(i).gameObject); 
        } 
        m_Player.Restart(); 
    }*/

    public void SetCheckpoint(Vector3 position, Quaternion rotation)
    {
        m_LastCheckpointPosition = position;
        m_LastCheckpointRotation = rotation;
        Debug.Log("Checkpoint guardado en: " + position);
    }
    public void RestartLevel()
    {
        for (int i = 0; i < m_DestroyObjects.childCount; ++i)
        {
            GameObject.Destroy(m_DestroyObjects.GetChild(i).gameObject);
        }

        if (m_Player != null)
        {
            if (m_LastCheckpointPosition.HasValue)
            {
                m_Player.transform.position = m_LastCheckpointPosition.Value;
                m_Player.transform.rotation = m_LastCheckpointRotation.Value;
            }
            else
            {
                GameObject spawn = GameObject.FindWithTag("SpawnPoint");
                m_Player.m_Health = m_Player.m_initialHealth;
                m_Player.m_Shield = m_Player.m_initialShield;
                m_Player.m_ChargerAmmoCount = m_Player.m_initialAmmo;
                m_Player.m_CharacterController.enabled = false;

                if (spawn != null)
                {
                    m_Player.transform.position = spawn.transform.position;
                    m_Player.transform.rotation = spawn.transform.rotation;
                }
                else
                {
                    m_Player.transform.position = Vector3.zero;
                    m_Player.transform.rotation = Quaternion.identity;
                }
            }

            m_Player.m_CharacterController.enabled = true;
        }
        m_fade.FadeOut(() =>
        {
            m_fade.gameObject.SetActive(false);
        });
    }
    //para cambiar entre escenas
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
            LoadScene();
            //SceneManager.LoadSceneAsync("VictorScene");
        if (Input.GetKeyDown(KeyCode.O))
            SceneManager.LoadSceneAsync("SampleScene");
    }
    public void LoadScene()
    {
        SceneManager.LoadSceneAsync("Level2");
    }
    public PlayerController GetPlayer()
    {
        return m_Player;
    }
    public void SetPlayer(PlayerController Player)
    {
        m_Player = Player;
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (m_Player != null)
        {
            
            GameObject spawn = GameObject.FindWithTag("SpawnPoint");

            m_Player.m_CharacterController.enabled = false;

            if (spawn != null)
            {
                m_Player.transform.position = spawn.transform.position;
                //m_Player.transform.rotation = spawn.transform.rotation;
            }
            else
            {
                m_Player.transform.position = Vector3.zero;
               // m_Player.transform.rotation = Quaternion.identity;
            }

            m_Player.m_CharacterController.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
