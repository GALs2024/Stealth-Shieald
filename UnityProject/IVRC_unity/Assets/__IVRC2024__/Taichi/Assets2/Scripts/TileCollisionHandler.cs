using UnityEngine;

public class TileCollisionHandler : MonoBehaviour
{
    // �Փˎ��̏���
    private void OnCollisionEnter(Collision collision)
    {
        // 2�b��Ƀ^�C�����폜
        Debug.Log("Collision detected with: " + collision.gameObject.name); // �f�o�b�O�p
        Destroy(gameObject, 2f);
    }
}
