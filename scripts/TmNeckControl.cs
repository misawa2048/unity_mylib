using UnityEngine;
using System.Collections.Generic;

namespace TmLib
{
    public class TmNeckControl : MonoBehaviour
    {
        public Transform targetJoint;
        public Transform eyeJoint;
        public float rotateLimit = 90f;
        [Range(0, 1)]
        public float blendRate = 1f;

        public Transform[] controlJoints;

        private Dictionary<Transform, Quaternion> baseRotDic;

        // Use this for initialization
        void Start()
        {
            baseRotDic = new Dictionary<Transform, Quaternion>();
            Transform[] childs = transform.GetComponentsInChildren<Transform>();
            for (int i = 0; i < childs.Length; ++i)
            {
                baseRotDic.Add(childs[i], childs[i].localRotation);
            }

            if (gameObject.name.StartsWith(@"^[0-9]{3}_[0-9]{2}"))
            {
                string idStr = gameObject.name.Substring(6, 2);
                int id;
                if (int.TryParse(idStr, out id))
                {
                    Debug.Log("id=" + id);
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (targetJoint)
            {
                Debug.DrawLine(eyeJoint.position, targetJoint.position, Color.red);
            }
        }

        void LateUpdate()
        {
            if ((controlJoints != null) && (controlJoints.Length > 0))
            {
                Animator anm = GetComponent<Animator>();
                for (int i = 0; i < controlJoints.Length; ++i)
                {
                    if (baseRotDic.ContainsKey(controlJoints[i]))
                    {
                        if ((anm != null) && (!anm.isActiveAndEnabled))
                        {
                            controlJoints[i].localRotation = baseRotDic[controlJoints[i]];
                        }
                        else
                        {
                            baseRotDic[controlJoints[i]] = controlJoints[i].localRotation;
                        }
                    }
                }

                Quaternion neckRot = NeckCtrl(controlJoints, targetJoint, eyeJoint, rotateLimit);
                neckRot = Quaternion.Lerp(Quaternion.identity, neckRot, blendRate);
                Quaternion subRot = Quaternion.Slerp(Quaternion.identity, neckRot, 1f / (float)controlJoints.Length);
                for (int i = 0; i < controlJoints.Length; ++i)
                {
                    Quaternion invRot = Quaternion.identity; // Quaternion.Inverse(controlJoints[i].localRotation);
                    controlJoints[i].localRotation = invRot * subRot * controlJoints[i].localRotation;
                }
            }
        }

        void OnAnimatorMove()
        {
        }

        static public Quaternion NeckCtrl(Transform[] _controlJoints, Transform _targetJnt, Transform _eyeJnt, float _limitAng)
        {
            Vector3 dirVec = (_targetJnt.position - _eyeJnt.position).normalized;
            dirVec = Quaternion.Inverse(_eyeJnt.rotation) * dirVec;
            Quaternion neckRot = Quaternion.FromToRotation(Vector3.forward, dirVec);
            float ang = Vector3.Angle(Vector3.forward, dirVec);
            if (ang > _limitAng)
            {
                float rate = (180f - ang) / (180f - _limitAng);
                neckRot = Quaternion.Slerp(Quaternion.identity, neckRot, rate);
            }
            return neckRot;
        }
    }
}
