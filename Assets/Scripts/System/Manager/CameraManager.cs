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
        player = FindPlayerTransform();
        maincamera = GetComponent<Camera>();
    }

    public void Update()
    {
        if (player == null)
        {
            player = FindPlayerTransform();
        }
        cameraWidth = maincamera.pixelWidth; cameraHeight = maincamera.pixelHeight;
    }

    // Update is called once per frame
    public void LateUpdate()
    {
        if (player != null && background != null)
        {

            Vector3 targetPos = new Vector3(player.position.x, player.position.y, maincamera.transform.position.z);

            float distance = -maincamera.transform.position.z;
            float height = distance * Mathf.Tan(maincamera.fieldOfView * Mathf.Deg2Rad / 2);
            float width = (cameraWidth / cameraHeight) * height;

            float minX = background.bounds.min.x + width;
            float minY = background.bounds.min.y + height;
            float maxX = background.bounds.max.x - width;
            float maxY = background.bounds.max.y - height;
            //Limit camera movement range

            targetPos.x = Mathf.Clamp(targetPos.x, minX, maxX);
            targetPos.y = Mathf.Clamp(targetPos.y, minY, maxY);
            maincamera.transform.position = Vector3.Lerp(maincamera.transform.position, targetPos, Time.deltaTime * 5f);
            // Relatively smooth tracking of playr positions
        }
    }

    public Transform FindPlayerTransform()
    {
        if (GameManager.player != null)
        {
            return GameManager.player.transform;
        }
        return null;
    }


    public void CameraMove(Transform trans)
    {
        player = trans;

    }

    public async Task CameraMoveV2V(Vector3 startPos, Vector3 endPos, float spd = 1f)
    {
        Debug.Log("Move Start");
        GameObject go = new GameObject();
        go.transform.position = startPos;
        player = go.transform;
        float startTime = Time.time; // 시작 시간 기록

        while ((go.transform.position - endPos).magnitude >= 0.05f)
        {
            float distanceCovered = (Time.time - startTime) * spd;
            float fractionOfJourney = distanceCovered / Vector3.Distance(startPos, endPos);
            go.transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
            await Task.Delay(TimeSpan.FromSeconds(0.016f));
        }

        player = GameManager.player.transform;
        // 이동 완료 후 게임 오브젝트 제거
        Destroy(go);
    }
}
