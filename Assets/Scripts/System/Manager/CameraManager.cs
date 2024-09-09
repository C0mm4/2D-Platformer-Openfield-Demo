using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CameraManager : MonoBehaviour
{

    public Transform player;
    public Camera maincamera;

    private float cameraWidth, cameraHeight;

    public SpriteRenderer background;



    public void Awake()
    {
        // Player and Camera Component Set
        player = FindPlayerTransform();
        maincamera = GetComponent<Camera>();
        // Set Camera Resolution
        cameraWidth = maincamera.pixelWidth; cameraHeight = maincamera.pixelHeight;
    }

    public void Update()
    {
        // Player is not exists, Refind player object
        if (player == null)
        {
            player = FindPlayerTransform();
        }
    }

    // Update is called once per frame
    public void LateUpdate()
    {
        // Player object and background object is not null
        if (player != null && background != null)
        {
            // target transform position
            Vector3 targetPos = new Vector3(player.position.x, player.position.y, maincamera.transform.position.z);

            // Set height width by camera distance and FoV
            float distance = -maincamera.transform.position.z;
            float height = distance * Mathf.Tan(maincamera.fieldOfView * Mathf.Deg2Rad / 2);
            float width = (cameraWidth / cameraHeight) * height;

            //Limit camera movement range
            float minX = background.bounds.min.x + width;
            float minY = background.bounds.min.y + height;
            float maxX = background.bounds.max.x - width;
            float maxY = background.bounds.max.y - height;

            // Relatively smooth tracking of playr positions
            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            maincamera.transform.position = Vector3.Lerp(maincamera.transform.position, targetPos, Time.deltaTime * 5f);
        }
    }

    // Find Player Transform from GameManager
    public Transform FindPlayerTransform()
    {
        if (GameManager.player != null)
        {
            return GameManager.player.transform;
        }
        return null;
    }

    // Camera Move to target Transform
    public void CameraMove(Transform trans)
    {
        player = trans;

    }

    // Camera Move Point to point
    public async Task CameraMoveV2V(Vector3 startPos, Vector3 endPos, float spd = 1f)
    {
        // Generate temp object
        GameObject go = new GameObject();
        // Set Position temp object
        go.transform.position = startPos;
        // temp object is target transform
        player = go.transform;

        // Recording moving Time
        float startTime = Time.time; 

        // loop temp object leach target Position
        while ((go.transform.position - endPos).magnitude >= 0.05f)
        {
            float distanceCovered = (Time.time - startTime) * spd;
            float fractionOfJourney = distanceCovered / Vector3.Distance(startPos, endPos);
            go.transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
            await Task.Yield();
        }

        // Camera target reset to player object
        player = GameManager.player.transform;
        // Destroy temp object
        Destroy(go);
    }
}
