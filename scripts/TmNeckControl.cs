using UnityEngine;
using System.Collections.Generic;

namespace TmLib
{
    public class TmNeckControl : MonoBehaviour
    {
        public Transform targetJoint;
        public Transform eyeJoint;
        public Vector3 eyeForward = Vector3.forward;
        public float rotateLimit = 90f;
        [Range(0, 1)]
        public float blendRate = 1f;

        public Transform[] controlJoints;

        private Dictionary<Transform, Quaternion> baseRotDic;

        // Use this for initialization
        void Start()
        {
            baseRotDic = new Dictionary<Transform, Quaternion>();
            for (int i = 0; i < controlJoints.Length; ++i)
            {
                baseRotDic.Add(controlJoints[i], controlJoints[i].localRotation);
            }

            if (targetJoint == null)
            {
                targetJoint = Camera.main.transform;
            }
            if (eyeJoint == null)
            {
                eyeJoint = transform;
            }
            if (controlJoints.Length == 0)
            {
                controlJoints = new Transform[] { eyeJoint };
            }
        }

        // Update is called once per frame
        void Update()
        {
        }

        void LateUpdate()
        {
            if ((controlJoints != null) && (controlJoints.Length > 0))
            {
                for (int i = 0; i < controlJoints.Length; ++i)
                {
                    if (baseRotDic.ContainsKey(controlJoints[i]))
                    {
                        controlJoints[i].localRotation = baseRotDic[controlJoints[i]];
                    }
                }
                if (eyeJoint && targetJoint) { Debug.DrawLine(eyeJoint.position, targetJoint.position, Color.red); }

                Quaternion neckRot = NeckCtrl(controlJoints, targetJoint, eyeJoint, eyeForward.normalized, rotateLimit);
                neckRot = Quaternion.Lerp(Quaternion.identity, neckRot, blendRate);
                Quaternion subRot = Quaternion.Slerp(Quaternion.identity, neckRot, 1f / (float)controlJoints.Length);
                for (int i = 0; i < controlJoints.Length; ++i)
                {
                    controlJoints[i].localRotation = subRot * controlJoints[i].localRotation;
                }
            }
        }

        void OnAnimatorMove()
        {
            for (int i = 0; i < controlJoints.Length; ++i)
            {
                if (baseRotDic.ContainsKey(controlJoints[i]))
                {
                    baseRotDic[controlJoints[i]] = controlJoints[i].localRotation;
                }
            }
        }

        static public Quaternion NeckCtrl(Transform[] _controlJoints, Transform _targetJnt, Transform _eyeJnt, Vector3 _eyeForward, float _limitAng)
        {
            Vector3 dirVec = (_targetJnt.position - _eyeJnt.position).normalized;
            dirVec = Quaternion.Inverse(_eyeJnt.rotation) * dirVec;
            Quaternion neckRot = Quaternion.FromToRotation(_eyeForward, dirVec);
            float ang = Vector3.Angle(_eyeForward, dirVec);
            if (ang > _limitAng)
            {
                float rate = (180f - ang) / (180f - _limitAng);
                neckRot = Quaternion.Slerp(Quaternion.identity, neckRot, rate);
            }
            return neckRot;
        }
    }
}
