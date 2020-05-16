using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public Animator boyAnimator;
    public CharacterController boyCharacter;
    public Transform boyTransform;
    public Rigidbody boyRigidbody;
    
    private float confineTime; //限制每帧执行调用次数

    // Start is called before the first frame update
    void Start()
    {
        confineTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        confineTime += Time.deltaTime;
        if (confineTime < 0.025f) return;
        
        // 遥感操作
        float horizontal = ETCInput.GetAxis("Horizontal");
        float vertical = ETCInput.GetAxis("Vertical");
        bool hasHorizontalInput = !Mathf.Approximately(horizontal, 0f);
        bool hasVerticalInput = !Mathf.Approximately(vertical, 0f);
        bool isWalking = hasHorizontalInput || hasVerticalInput;
        boyAnimator.SetInteger("action", isWalking ? 2 : 0);

        if (isWalking) {
            Vector3 dirA = boyTransform.forward;
            Vector3 dirB = new Vector3(horizontal, 0f, vertical);
            // 计算角度
            float angleY = Vector3.Angle(dirA, dirB);
            // 计算方向
            float dir = Vector3.Dot(Vector3.up, Vector3.Cross(dirA, dirB)) < 0 ? -1 : 1;
            angleY *= dir;
            if (angleY != 0) {
                boyTransform.Rotate(new Vector3(0f, angleY, 0f), Space.World);
            }
            boyTransform.Translate(dirB * 8 * confineTime, Space.World);
            // boyRigidbody.MovePosition(boyRigidbody.position + (dirB * 8 * confineTime));
        }
        
        // 按钮操作
        bool isAttackDown = ETCInput.GetButtonDown("ButtonAttack");
        if (isAttackDown) {
            boyAnimator.SetInteger("action", 3);
        }
        // bool isAttackUp = ETCInput.GetButtonUp("ButtonAttack");
        // if (isAttackUp) {
            // boyAnimator.SetInteger("action", 0);
        // }
        
        confineTime = 0;
    }
}
