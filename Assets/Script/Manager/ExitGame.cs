using UnityEngine;

public class ExitGame : MonoBehaviour
{
    // Exit 버튼에서 호출할 메서드
    public void ExitGameButton()
    {
        Debug.Log("게임 종료 버튼 클릭됨");

#if UNITY_EDITOR
        // 에디터에서 실행 중일 경우 에디터 종료
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 빌드된 실행 파일에서는 애플리케이션 종료
        Application.Quit();
#endif
    }
}
