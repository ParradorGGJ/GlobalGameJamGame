
namespace Parrador
{

    interface InteractiveObject
    {
        ObjectType GetObjectType();

        void SetState(bool aState);

        bool GetState();

        void UpdateState();

    }
}