using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

namespace FigmentGames
{
    public partial class PolygonTrigger2D : EditablePolygonCollider
    {
        [Space(10)]
        [Header("EVENTS")]
        [SerializeField] public Transform[] transforms;

        [Space]
        [SerializeField] private UnityEvent onFirstTransformEnter;
        [SerializeField] private UnityEvent onAnyTransformEnter;
        [SerializeField] private UnityEvent onAnyTransformStay;
        [SerializeField] private UnityEvent onAnyTransformExit;
        [SerializeField] private UnityEvent onLastTransformExit;

        // Cache
        private List<bool> transformsIn = new List<bool>();

        private void Awake()
        {
            RecreateBoolArray();
        }

        private void Update()
        {
            for (int i = 0; i < transforms.Length; i++)
            {
                if (polygonCollider.OverlapPoint(transforms[i].position)) // Transform is inside
                {
                    if (!transformsIn[i])
                    {
                        // No transform is within the collider yet
                        if (!AnyTransformIsInside())
                            onFirstTransformEnter.Invoke();

                        onAnyTransformEnter.Invoke();

                        transformsIn[i] = true;
                    }
                }
                else // Transform is outside
                {
                    if (transformsIn[i])
                    {
                        transformsIn[i] = false;

                        onAnyTransformExit.Invoke();

                        // No more transform within the collider
                        if (!AnyTransformIsInside())
                            onLastTransformExit.Invoke();
                    }
                }
            }

            if (AnyTransformIsInside())
                onAnyTransformStay.Invoke();
        }

        private bool AnyTransformIsInside()
        {
            return transformsIn.Contains(true);
        }

        private void SetTransforms(Transform[] transforms)
        {
            this.transforms = transforms;
            RecreateBoolArray();
        }

        private void RecreateBoolArray()
        {
            transformsIn = new bool[transforms.Length].ToList();
        }
    }
}