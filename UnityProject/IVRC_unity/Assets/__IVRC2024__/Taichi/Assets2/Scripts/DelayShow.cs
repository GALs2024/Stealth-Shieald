using System.Collections;
using UnityEngine;

public class DelayedShow : MonoBehaviour
{
    public GameObject targetObject;  // �\��������Ώۂ̃I�u�W�F�N�g
    public float delayTime = 5f;     // �x������ (�b)

    void Start()
    {
        // ���߂ɃI�u�W�F�N�g���\���ɂ���
        targetObject.SetActive(false);

        // �R���[�`�����J�n���āA�w�莞�Ԍ�ɃI�u�W�F�N�g��\��
        StartCoroutine(ShowObjectAfterDelay());
    }

    // �w�莞�Ԍ�ɃI�u�W�F�N�g��\������R���[�`��
    IEnumerator ShowObjectAfterDelay()
    {
        // �w�肳�ꂽ�b���҂�
        yield return new WaitForSeconds(delayTime);

        // �I�u�W�F�N�g��\������
        targetObject.SetActive(true);
    }
}
