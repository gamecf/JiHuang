using UnityEngine;

namespace GlowingSwords.Scripts
{
    /// <inheritdoc />
    /// <summary>
    /// This script rotates the sabers.
    /// </summary>
    public class Spin : MonoBehaviour {

        [SerializeField]
        private Vector3 _spinDirection;

        private void Update()
        {
            transform.Rotate(_spinDirection * Time.deltaTime);
        }
    }
}
