using UnityEngine;

namespace DaftAppleGames.MyFirstThunderKitMod
{
    public class RotateObject : MonoBehaviour
    {
        public Vector3 rotationVector = Vector3.up;
        public float rotationSpeed = 90f;

        void Update()
        {
            transform.Rotate(rotationVector * rotationSpeed * Time.deltaTime);
        }
    }    
}

