using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TmLib
{
    public class TmPhysicsPlCtrl : MonoBehaviour
    {
        [SerializeField] float JumpSpeed = 50.0f;
        [SerializeField] float RotationSpeed = 90.0f;
        [SerializeField] float MovementSpeed = 2.0f;
        [SerializeField] float MaxSpeed = 0.2f;

        Rigidbody rb;
        private float rot = 0.0f;
        private float acc = 0.0f;
        private bool isJumping = false;

        // Start is called before the first frame update
        void Start()
        {
            rb = GetComponent<Rigidbody>();
        }

        // Update is called once per frame
        void Update()
        {
            rot = Input.GetAxis("Horizontal");
            acc = Input.GetAxis("Vertical");
            if (Input.GetButton("Jump") && !isJumping)
            {
                StartCoroutine(jumpCo());
            }
        }
        void FixedUpdate()
        {
            Quaternion tmpRot = rb.rotation * Quaternion.AngleAxis(rot * RotationSpeed * Time.fixedDeltaTime, transform.up);
            rb.MoveRotation(tmpRot);

            Vector3 force = transform.forward * acc * 1000.0f * MovementSpeed * Time.fixedDeltaTime;
            rb.AddForce(force);
            if (rb.velocity.magnitude > (MaxSpeed * 1000.0f))
            {
                rb.velocity = rb.velocity.normalized * MaxSpeed * 1000.0f;
            }
        }
        IEnumerator jumpCo()
        {
            isJumping = true;
            rb.AddForce(transform.up * JumpSpeed, ForceMode.Impulse);
            yield return new WaitForSeconds(0.5f);
            isJumping = false;
        }
    }
}
