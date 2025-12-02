using UnityEngine;

namespace GaussianSplatting.Runtime
{
    public struct RuntimeInputSplatData   // °Á ¿Ã∏ß∏∏ πŸ≤ﬁ
    {
        public Vector3 pos;
        public Vector3 nor;
        public Vector3 dc0;
        public Vector3 sh1, sh2, sh3, sh4, sh5, sh6, sh7, sh8, sh9, shA, shB, shC, shD, shE, shF;
        public float opacity;
        public Vector3 scale;
        public Quaternion rot;
    }
}
