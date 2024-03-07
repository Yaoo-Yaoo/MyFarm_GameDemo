using UnityEngine;

namespace MyFarm.Inventory
{
    public class ItemBounce : MonoBehaviour
    {
        private Transform spriteTrans;
        private BoxCollider2D coll;

        public float gravity = -3.5f;
        private bool isGround;
        private Vector2 direction;
        private Vector3 targetPos;
        private float distance;

        private void Awake()
        {
            spriteTrans = transform.GetChild(0);
            coll = GetComponent<BoxCollider2D>();
            coll.enabled = false;
        }

        private void Update()
        {
            Bounce();
        }

        public void InitBounceItem(Vector3 target, Vector2 dir)
        {
            coll.enabled = false;
            targetPos = target;
            direction = dir;
            distance = Vector3.Distance(targetPos, transform.position);

            // 物品放在头顶
            spriteTrans.position += Vector3.up * 1.5f;
        }

        private void Bounce()
        {
            // 横向移动
            if (Vector3.Distance(transform.position, targetPos) > 0.1f)
            {
                transform.position += (Vector3)direction * distance * -gravity * Time.deltaTime;
            }

            // 纵向移动
            isGround = spriteTrans.position.y <= transform.position.y;

            if (!isGround)
            {
                spriteTrans.position += Vector3.up * gravity * Time.deltaTime;
            }
            else
            {
                spriteTrans.position = transform.position;
                coll.enabled = true;
            }
        }
    }
}
