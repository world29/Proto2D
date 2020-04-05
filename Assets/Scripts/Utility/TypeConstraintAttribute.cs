using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Proto2D
{
    // http://answers.unity.com/answers/1479775/view.html
    public class TypeConstraintAttribute : PropertyAttribute
    {
        private System.Type m_type;

        public TypeConstraintAttribute(System.Type type)
        {
            m_type = type;
        }

        public System.Type Type
        {
            get { return m_type; }
        }
    }
}
