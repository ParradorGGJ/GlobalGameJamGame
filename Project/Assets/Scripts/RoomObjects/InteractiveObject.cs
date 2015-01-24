using UnityEngine;


namespace Parrador
{

    interface InteractiveObject
    {
        ObjectType GetObjectType();

        void SetState(bool aState);
        bool GetState();

        void UpdateState();

        Vector3 GetPosition();
        void SetPosition(Vector3 aPosition);

        Quaternion GetRotation();
        void SetRotation(Quaternion aRotation);
    }
}