using UnityEngine;

public class ExitGame : MonoBehaviour
{
        // Exit ��ư���� ȣ���� �޼���
        public void ExitGameButton()
        {
                Debug.Log("���� ���� ��ư Ŭ����");

#if UNITY_EDITOR
                // �����Ϳ��� ���� ���� ��� ������ ����
                UnityEditor.EditorApplication.isPlaying = false;
#else
                // ����� ���� ���Ͽ����� ���ø����̼� ����
                Application.Quit();
#endif
        }
}
