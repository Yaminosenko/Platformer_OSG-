using UnityEngine;

namespace FigmentGames
{
    public class EnhancedPhysics
    {
        [System.Serializable]
        public struct Sphere
        {
            public Vector3 position;
            public float radius;

            public Sphere(Vector3 position, float radius)
            {
                this.position = position;
                this.radius = radius;
            }
        }

        [System.Serializable]
        public class SphereCastHit
        {
            public Sphere sphere;
            public Vector3 direction;
            public RaycastHit raycastHit;

            public bool success;

            // Accessors
            public float hitDot { get { return Vector3.Angle(direction, sphere.position); } }
            public float normalDot { get { return Vector3.Dot(direction, raycastHit.normal); } }
            public float zenithDot { get { return Vector3.Dot(Vector3.up, raycastHit.normal); } }
            public float angle { get { return Vector3.Angle(Vector3.up, raycastHit.normal); } }

            public SphereCastHit()
            {
                this.sphere = new Sphere();
                this.direction = Vector3.zero;
                this.raycastHit = new RaycastHit();

                this.success = false;
            }

            public SphereCastHit(Sphere sphere, Vector3 direction, RaycastHit hit)
            {
                this.sphere = sphere;
                this.direction = direction;
                this.raycastHit = hit;

                this.success = true;
            }


            public Vector3 SphereSnap(float offset = 0f)
            {
                return raycastHit.point + raycastHit.normal * (sphere.radius + offset);
            }
        }

        [System.Serializable]
        public class PhysicsIteration
        {
            public Vector3 position;
            public Quaternion rotation;
            public Vector3 velocity;

            public PhysicsIteration(Vector3 position, Quaternion rotation, Vector3 velocity)
            {
                this.position = position;
                this.rotation = rotation;
                this.velocity = velocity;
            }
        }
    }
}