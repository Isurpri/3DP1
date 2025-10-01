using UnityEngine;

public class Weapon : MonoBehaviour
{

    [SerializeField] private Camera weaponcamera;
    void Start()
    {
        
    }

    void Update()
    {
        Ray middleCameraRay = weaponcamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.5f));
    }
}
