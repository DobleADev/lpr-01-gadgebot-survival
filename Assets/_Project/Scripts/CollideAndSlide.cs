using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CollideAndSlide
{
    public static void Slide(
        this Rigidbody2D rigidbody2D
    , Vector2 velocity
    , float skinWidth = 0.08f
    , float maxIterations = 3)
    {
        Vector2 initialPosition = rigidbody2D.position;
        RaycastHit2D[] castHit = new RaycastHit2D[1];
        Vector2 accumulatedVelocity = Vector2.zero;
        float remainingSpeed = velocity.magnitude;
        Vector2 remainingDirection = velocity.normalized;
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = false;

        for (int i = 0; i < maxIterations; i++)
        {
            if (remainingSpeed < Mathf.Epsilon) break;

            if (rigidbody2D.Cast(remainingDirection, contactFilter2D, castHit, remainingSpeed + skinWidth) > 0)
            {
                Vector2 velocityTillContact = Mathf.Max(0, castHit[0].distance - skinWidth) * remainingDirection;
                remainingSpeed = Mathf.Max(0, remainingSpeed - velocityTillContact.magnitude);
                remainingDirection = remainingDirection.ProjectOnPlane(castHit[0].normal);
                accumulatedVelocity += velocityTillContact;
                rigidbody2D.position += velocityTillContact;
                rigidbody2D.SendMessage("OnPhysicsCollision2D", castHit[0].collider);
            }
            else
            {
                accumulatedVelocity += remainingSpeed * remainingDirection;
                break;
            }
        }
        rigidbody2D.MovePosition(initialPosition + accumulatedVelocity);
    }

    public static void Walk(this Rigidbody2D rigidbody2D
    , Vector2 velocity
    , float maxSlopeAngle = 45
    , float skinWidth = 0.08f
    , int maxIterations = 5
    , int accuracy = 3)
    {
        Vector2 initialPosition = rigidbody2D.position;
        RaycastHit2D[] castHit = new RaycastHit2D[accuracy];
        RaycastHit2D[] snapHit = new RaycastHit2D[accuracy];
        Vector2 accumulatedVelocity = Vector2.zero;
        float remainingSpeed = velocity.magnitude;
        Vector2 remainingDirection = velocity.normalized, initialDirection = velocity.normalized;
        bool isProjectedOnIteration = false;
        ContactFilter2D contactFilter2D = new ContactFilter2D();
        contactFilter2D.useTriggers = false;

        for (int i = 0; i < maxIterations; i++)
        {
            // isProjectedOnIteration = false;
            if (remainingSpeed < Mathf.Epsilon) break;

            if (rigidbody2D.Cast(Vector2.down, contactFilter2D, snapHit, remainingSpeed + skinWidth) > 0
            && Vector2.Dot(remainingDirection, Vector2.down) <= 0
            )
            {
                Vector2 snapVector = Mathf.Max(0, snapHit[0].distance - skinWidth) * Vector2.down;
                Vector2 snapNormal = Vector2.zero;
                foreach (var hit in snapHit) { snapNormal += hit.normal; }

                // Vector2 downOrigin = rigidbody2D.ClosestPoint(new Vector2(rigidbody2D.position.x, -Mathf.Infinity)) + new Vector2(0f, -skinWidth);
                Vector2 downOrigin = rigidbody2D.position + new Vector2(0, -0.6f);
                RaycastHit2D[] snapValidationHit = new RaycastHit2D[1];
                bool canSnap = false;
                // Debug.Log("downOrigin: " + downOrigin);

                if (Physics2D.RaycastNonAlloc(downOrigin, Vector2.down, snapValidationHit, 2 * skinWidth) > 0)
                {
                    canSnap = snapValidationHit[0].distance <= skinWidth;
                }
                if (canSnap)
                {
                    if (!isProjectedOnIteration)
                    {
                        remainingDirection = remainingDirection.ProjectOnPlane(snapNormal.normalized).normalized;
                    }

                    // Debug.Log("downOrigin: " + downOrigin + " -  :" + downOrigin);
                    // Debug.DrawRay(downOrigin, snapValidationHit[0].distance * Vector2.down, Color.red);
                    accumulatedVelocity += snapVector;
                    rigidbody2D.position += snapVector;
                }

                Debug.DrawRay(rigidbody2D.position, remainingDirection * 2f);
            }

            isProjectedOnIteration = false;
            if (rigidbody2D.Cast(remainingDirection, contactFilter2D, castHit, remainingSpeed + skinWidth) > 0)
            {
                Vector2 velocityTillContact = Mathf.Max(0, castHit[0].distance - skinWidth) * remainingDirection;
                remainingSpeed = Mathf.Max(0, remainingSpeed - velocityTillContact.magnitude);
                Vector2 castNormal = Vector2.zero;
                float hitAngle;
                foreach (var hit in castHit)
                {
                    hitAngle = Vector2.Dot(hit.normal, Vector2.up);
                    if (hitAngle < Mathf.Cos(maxSlopeAngle * Mathf.Deg2Rad) && hitAngle <= 0) continue;
                    castNormal += hit.normal;
                    rigidbody2D.SendMessage("OnPhysicsCollision2D", hit.collider);
                }
                if (castNormal != Vector2.zero)
                    remainingDirection = remainingDirection.ProjectOnPlane(castNormal.normalized).normalized;
                accumulatedVelocity += velocityTillContact;
                rigidbody2D.position += velocityTillContact;
                isProjectedOnIteration = true;
            }

            if (!isProjectedOnIteration)
            {
                accumulatedVelocity += remainingSpeed * remainingDirection;
                break;
            }
        }
        rigidbody2D.MovePosition(initialPosition + accumulatedVelocity);
    }

    public static void Slide(this Rigidbody rigidbody, Vector3 velocity, float skinWidth = 0.08f, float maxIterations = 3)
    {
        Vector3 initialPosition = rigidbody.position;
        RaycastHit[] castHit;
        Vector3 accumulatedVelocity = Vector2.zero;
        float remainingSpeed = velocity.magnitude;
        Vector3 remainingDirection = velocity.normalized;

        for (int i = 0; i < maxIterations; i++)
        {
            if (remainingSpeed < Mathf.Epsilon) break;

            if ((castHit = rigidbody.SweepTestAll(remainingDirection, remainingSpeed + skinWidth)).Length > 0)
            {
                float nearestDistance = Mathf.Infinity;
                Vector3 castNormal = Vector3.zero;
                foreach (var hit in castHit)
                {
                    castNormal += hit.normal;
                    if (hit.distance < nearestDistance) nearestDistance = hit.distance;
                }
                Vector3 velocityTillContact = Mathf.Max(0, nearestDistance - skinWidth) * remainingDirection;
                remainingSpeed = Mathf.Max(0, remainingSpeed - velocityTillContact.magnitude);
                remainingDirection = Vector3.ProjectOnPlane(remainingDirection, castNormal.normalized).normalized;
                accumulatedVelocity += velocityTillContact;
                rigidbody.position += velocityTillContact;
            }
            else
            {
                accumulatedVelocity += remainingSpeed * remainingDirection;
                break;
            }
        }
        rigidbody.MovePosition(initialPosition + accumulatedVelocity);
    }

    public static void Walk(this Rigidbody rigidbody, Vector3 velocity, float maxSlopeAngle = 45, float skinWidth = 0.08f, int maxIterations = 5)
    {
        Vector3 initialPosition = rigidbody.position;
        RaycastHit[] castHit;
        RaycastHit[] snapHit;
        Vector3 accumulatedVelocity = Vector3.zero;
        float remainingSpeed = velocity.magnitude;
        Vector3 remainingDirection = velocity.normalized, initialDirection = velocity.normalized;
        bool isProjectedOnIteration = false;

        for (int i = 0; i < maxIterations; i++)
        {
            // isProjectedOnIteration = false;
            if (remainingSpeed < Mathf.Epsilon) break;
            if ((snapHit = rigidbody.SweepTestAll(Vector3.down, remainingSpeed + skinWidth)).Length > 0
            && Vector3.Angle(remainingDirection, Vector3.down) <= 90
            )
            {
                float nearestDistance = Mathf.Infinity;
                Vector3 snapNormal = Vector3.zero;
                foreach (var hit in snapHit)
                {
                    snapNormal += hit.normal;
                    if (hit.distance < nearestDistance) nearestDistance = hit.distance;
                }
                Vector3 snapVector = Mathf.Max(0, nearestDistance - skinWidth) * Vector3.down;
                if (!isProjectedOnIteration)
                {
                    remainingDirection = Vector3.ProjectOnPlane(remainingDirection, snapNormal.normalized).normalized;
                }

                accumulatedVelocity += snapVector;
                rigidbody.position += snapVector;

                // Debug.DrawRay(rigidbody2D.position, remainingDirection * 2f);
            }

            if ((castHit = rigidbody.SweepTestAll(remainingDirection, remainingSpeed + skinWidth)).Length > 0)
            {
                float nearestDistance = Mathf.Infinity;
                Vector3 castNormal = Vector3.zero;
                float hitAngle;
                foreach (var hit in castHit)
                {
                    hitAngle = Vector3.Angle(hit.normal, Vector3.up);
                    castNormal +=
                    hitAngle > maxSlopeAngle && hitAngle < 90 ?
                    Vector3.zero :
                    hit.normal;
                    if (hit.distance < nearestDistance) nearestDistance = hit.distance;
                }
                Vector3 velocityTillContact = Mathf.Max(0, nearestDistance - skinWidth) * remainingDirection;
                remainingSpeed = Mathf.Max(0, remainingSpeed - velocityTillContact.magnitude);
                if (castNormal != Vector3.zero)
                    remainingDirection = Vector3.ProjectOnPlane(remainingDirection, castNormal.normalized).normalized;
                accumulatedVelocity += velocityTillContact;
                rigidbody.position += velocityTillContact;
                isProjectedOnIteration = true;
            }
            else
            {
                isProjectedOnIteration = false;
            }

            if (!isProjectedOnIteration)
            {
                accumulatedVelocity += remainingSpeed * remainingDirection;
                break;
            }
        }
        rigidbody.MovePosition(initialPosition + accumulatedVelocity);
    }

    /// <summary>
    /// Proyecta un Vector2 sobre un "plano" definido por una normal Vector2.
    /// En 2D, esto significa encontrar la componente del vector que es perpendicular a la normal.
    /// </summary>
    /// <param name="vector">El vector a proyectar.</param>
    /// <param name="planeNormal">La normal del "plano" (la dirección perpendicular a la línea de proyección).</param>
    /// <returns>El vector proyectado sobre el "plano" (perpendicular a la normal).</returns>
    public static Vector2 ProjectOnPlane(this Vector2 vector, Vector2 planeNormal)
    {
        if (planeNormal == Vector2.zero) return vector;
        return vector - Vector2.Dot(vector, planeNormal) * planeNormal;
    }

    // public static Vector2 ProjectOnPlane(this Vector2 vector, Vector2 planeNormal)
    // {
    //     // Asegúrate de que la normal no sea un vector cero para evitar divisiones por cero.
    //     if (planeNormal == Vector2.zero)
    //     {
    //         Debug.LogWarning("ProjectOnPlane: La normal del plano es un vector cero. No se puede proyectar.");
    //         return vector; // O devuelve Vector2.zero, dependiendo de tu lógica.
    //     }

    //     // Calcula el producto punto entre el vector y la normal.
    //     float dotProduct = Vector2.Dot(vector, planeNormal);

    //     // Calcula la magnitud cuadrada de la normal.
    //     float normalSqrMagnitude = planeNormal.sqrMagnitude;

    //     // Calcula la componente del vector que es paralela a la normal.
    //     // Esto es la proyección de 'vector' sobre 'planeNormal'.
    //     Vector2 projectionOnNormal = (dotProduct / normalSqrMagnitude) * planeNormal;

    //     // El vector proyectado sobre el "plano" (perpendicular a la normal) es
    //     // el vector original menos su componente paralela a la normal.
    //     return vector - projectionOnNormal;
    // }
}
