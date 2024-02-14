using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HeneGames
{
    public class EffectRemover : MonoBehaviour
    {
        public void DestroyEffect(float _timeToDestroy)
        {
            Destroy(gameObject, _timeToDestroy);
        }
    }
}