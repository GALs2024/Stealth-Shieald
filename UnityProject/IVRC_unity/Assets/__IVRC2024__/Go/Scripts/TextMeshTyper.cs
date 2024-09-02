using System.Collections;
using UnityEngine;
using TMPro;

public class TextMeshTyper : MonoBehaviour
{
    // 3D�I�u�W�F�N�g�p��TextMeshPro
    public TextMeshPro textObject;

    // ������\������Ԋu�i�b�j
    public float typingSpeed = 0.05f;

    // �\������e�L�X�g�̓��e
    private string textToDisplay = "����� 3D TextMesh example�ł�!?.�B�A����� 3D TextMesh example�ł������ 3D TextMesh example�ł������ 3D TextMesh example�ł������ 3D TextMesh example�ł������ 3D TextMesh example�ł������ 3D TextMesh example�ł������ 3D TextMesh example�ł������ 3D TextMesh example�ł������ 3D TextMesh example�ł������ 3D TextMesh example�ł�";

    void Start()
    {
        // �R���[�`�����g���ăe�L�X�g���ꕶ�����\��
        if (textObject != null)
        {
            StartCoroutine(TypeText(textObject, textToDisplay));
        }
    }

    // �e�L�X�g���ꕶ�����\������R���[�`��
    IEnumerator TypeText(TextMeshPro textObject, string textToDisplay)
    {
        textObject.text = ""; // �ŏ��Ƀe�L�X�g���N���A
        foreach (char letter in textToDisplay.ToCharArray())
        {
            textObject.text += letter; // �ꕶ�����ǉ�
            yield return new WaitForSeconds(typingSpeed); // �w�肵�����ԑ҂�
        }
    }
}
