using UnityEngine;
using TMPro; // TextMeshPro���g�p���邽�߂ɕK�v
using System.Collections;

public class TextTyper : MonoBehaviour
{
    // Canvas���TextMeshPro (TMP) �e�L�X�g�t�B�[���h�ւ̎Q�Ƃ�ݒ�
    public TMP_Text textMeshPro1;
    public TMP_Text textMeshPro2;

    // ������\������Ԋu�i�b�j
    public float typingSpeed = 0.05f;

    // �\������e�L�X�g�̓��e
    private string textToDisplay1 = "�����Canvas 1�̃e�L�X�g�ł�";
    private string textToDisplay2 = "�����Canvas 1�̃e�L�X�g�ł�";

    void Start()
    {
        // �R���[�`�����g���ăe�L�X�g���ꕶ�����\��
        if (textMeshPro1 != null)
        {
            StartCoroutine(TypeText(textMeshPro1, textToDisplay1));
        }

        if (textMeshPro2 != null)
        {
            StartCoroutine(TypeText(textMeshPro2, textToDisplay2));
        }
    }

    // �e�L�X�g���ꕶ�����\������R���[�`��
    IEnumerator TypeText(TMP_Text textMeshPro, string textToDisplay)
    {
        textMeshPro.text = ""; // �ŏ��Ƀe�L�X�g���N���A
        foreach (char letter in textToDisplay.ToCharArray())
        {
            textMeshPro.text += letter; // �ꕶ�����ǉ�
            yield return new WaitForSeconds(typingSpeed); // �w�肵�����ԑ҂�
        }
    }
}
