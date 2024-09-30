using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ITKombat
{
    [ExecuteInEditMode]
    public class ParallaxLayer : MonoBehaviour
    {
        public float parallaxFactor;
    
        public void Move(float delta)
        {
            Vector3 newPos = transform.localPosition;
            newPos.x -= delta * parallaxFactor;
    
            transform.localPosition = newPos;
        }
    }
}
