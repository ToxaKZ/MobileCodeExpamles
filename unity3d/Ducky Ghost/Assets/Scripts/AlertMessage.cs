using UnityEngine;
using System.Collections;

public class AlertMessage : MonoBehaviour
{
    public float DisplaySeconds = 1f;
    public GameObject Title = null;
    private static string message;

    void Awake()
    {
        UtilitiesHelper.SharedInstance().ApplySortingLayer(gameObject, Constants.SORTING_LAYER_NAME_ALERT);
    }

    void Start()
    {
        Title.GetComponent<TextMesh>().text = AlertMessage.message;
        Vector3 titleScale = Title.transform.localScale;

        Transform transformCached = transform;
        transformCached.localScale = new Vector3(Title.renderer.bounds.size.x + 1, transformCached.localScale.y, transformCached.localScale.z);
        Title.transform.localScale = new Vector3(titleScale.x / transformCached.localScale.x, titleScale.y / transformCached.localScale.y,
                                                     titleScale.z / transformCached.localScale.z);
        Invoke("Remove", DisplaySeconds);
    }
    
    void Update()
    {
    }

    void Remove()
    {
        Destroy(gameObject);
    }

    public static void DisplayAlert(string msg)
    {
        AlertMessage.message = msg;
        Instantiate(Camera.main.GetComponent<MainCameraController>().AlertPrefab);
    }
}