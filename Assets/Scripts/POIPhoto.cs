using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class POIPhoto : MonoBehaviour
{
    public Texture2D loading, error;

    private Camera m_MainCamera;

    private MeshRenderer m_Quad;

    private string m_PhotoURL;

    // Start is called before the first frame update
    void Start()
    {
        m_MainCamera = Camera.main;

        m_Quad = GetComponentInChildren<MeshRenderer>();

        // SetPhotoURL("https://hcii-cms.omdat.id/storage/pois/60b9ee6c2e4fc867707516a2/9d13fa8358dab5d4d1017c6a92f2b2d9.jpg");
    }

    // Update is called once per frame
    void Update()
    {
        // TODO temporary while we have not anchored this
        Vector3 lookPosition = transform.position - m_MainCamera.transform.position;
        lookPosition.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPosition);
    }

    public void SetPhotoURL(string url)
    {
        if (url.Equals(m_PhotoURL))
            return;

        m_PhotoURL = url;

        Davinci.get()
            .load(m_PhotoURL)
            .setLoadingPlaceholder(loading)
            .setErrorPlaceholder(error)
            .into(m_Quad)
            .start();
    }
}
