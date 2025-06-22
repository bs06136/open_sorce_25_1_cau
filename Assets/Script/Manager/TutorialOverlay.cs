using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class TutorialOverlay : MonoBehaviour
{
    [System.Serializable]
    public class Step
    {
        public RectTransform target;     // 강조할 UI
        public GameObject tooltipUI;     // 해당 단계의 설명 오브젝트
    }

    public Canvas canvas;
    public Material overlayMaterial;
    public List<Step> steps;

    private int currentStep = 0;

    void Start()
    {

    }

    void OnEnable()
    {
        currentStep = 0;
        ShowStep(0);
    }

    void Update()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
        {
            currentStep++;
            if (currentStep >= steps.Count)
            {
                gameObject.SetActive(false); // 튜토리얼 종료
            }
            else
            {
                ShowStep(currentStep);
            }
        }
    }

    void ShowStep(int index)
    {
        for (int i = 0; i < steps.Count; i++)
            steps[i].tooltipUI.SetActive(i == index);

        RectTransform target = steps[index].target;
        RectTransform tooltipRect = steps[index].tooltipUI.GetComponent<RectTransform>();
        RectTransform canvasRect = canvas.GetComponent<RectTransform>();

        // 타겟의 월드 중심 좌표 구하기
        Vector2 worldPos = target.TransformPoint(target.rect.center);

        // 캔버스 로컬 좌표로 변환
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, worldPos, canvas.worldCamera, out localPoint);

        // if (tooltipRect != null)
        // {
        //     Vector2 offset = Vector2.zero;
        //     if (index == 3 || index == 4 || index == 5)
        //     {
        //         // 오른쪽에 배치: 타겟의 오른쪽 + 툴팁의 절반 + 여유(30)
        //         float offsetX = target.rect.width * 0.5f + tooltipRect.rect.width * 0.5f + 30f;
        //         offset = new Vector2(offsetX, 0);
        //     }
        //     else
        //     {
        //         // 아래에 배치: 타겟의 아래 + 툴팁의 절반 + 여유(30)
        //         float offsetY = -(target.rect.height * 0.5f + tooltipRect.rect.height * 0.5f + 30f);
        //         offset = new Vector2(0, offsetY);
        //     }
        //     tooltipRect.anchoredPosition = localPoint + offset;
        // }

        Vector2 canvasSize = canvasRect.sizeDelta;
        Vector2 holeCenterUV = (localPoint + canvasSize * 0.5f) / canvasSize;
        Vector2 holeSizeUV = target.rect.size / canvasSize;

        overlayMaterial.SetVector("_HoleCenter", new Vector4(holeCenterUV.x, holeCenterUV.y, 0, 0));
        overlayMaterial.SetVector("_HoleSize", new Vector4(holeSizeUV.x, holeSizeUV.y, 0, 0));
    }
}
