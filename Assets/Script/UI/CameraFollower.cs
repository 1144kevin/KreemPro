using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    #region Variables
    
        [SerializeField]private Vector3 _offset;
        [SerializeField] private Transform target;
        [SerializeField] private float smoothTime;
        private Vector3 _currentVelocity = Vector3.zero;
        
    #endregion
    
    #region Unity callbacks 
    
        private void Awake(){
             _offset = target.position - transform.position;}

        private void LateUpdate()
        {   
            Vector3 targetPosition = target.position - _offset;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _currentVelocity, smoothTime);
        }

        public void SetTarget(Transform newTarget)
    {
        target = newTarget;
        if (target != null) {
            _offset = target.position - transform.position;
        }
    }
        
    #endregion
}