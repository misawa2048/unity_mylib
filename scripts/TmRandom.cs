using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TmLib{
    public class TmRandom // M系列ランダム
    {
        readonly System.UInt32 m_seedDef = 338241;
        System.UInt32 m_value = 338241;

        public TmRandom(){
            m_value = m_seedDef;
        }

        public void Reset(){
            m_value = m_seedDef;
        }
        public void SetSeed(System.UInt32 _value){
            m_value = _value;
        }
        public System.UInt32 Update(){
            // https://stackoverflow.com/questions/33716998/why-does-seed-9301-49297-233280-233280-0-generate-a-random-number
            m_value = (m_value * 9301 + 49297) % 233281;
            return m_value;
        }
    }
}
