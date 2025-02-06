using UnityEngine;
using Attributes;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace VehicleController {
	
	public sealed class Tire : MonoBehaviour {
		[SerializeField] private VehicleController vehicleController;

		[Space]
		[SerializeField, Range(0.1f, 1.5f)] private float restDistance = 0.8f;
		[SerializeField, Range(25000.0f, 55000.0f)] private float strength = 35000.0f;
		[SerializeField, Range(3000.0f, 15000.0f)] private float damper = 9000.0f;

		[Space]
		[SerializeField] private Transform tireTransform;
		[SerializeField, Range(0.1f, 1.0f)] private float radius = 0.3f;
		[SerializeField, Range(5.0f, 60.0f)] private float mass = 20.0f;
		[SerializeField] private AnimationCurve gripCurve;

		[Space]
		[SerializeField] private bool canProduceSmoke;
		[SerializeField, ConditionalHide(nameof(canProduceSmoke), true)] private ParticleSystem particle;
		[SerializeField, ConditionalHide(nameof(canProduceSmoke), true), Range(3.0f, 9.0f)] private float emitThreshold = 6.0f;
		
		[Space]
		[SerializeField] private bool steeringTires;
		[SerializeField, ConditionalHide(nameof(steeringTires), true)] private float maxSteerAngle = 30.0f;
		
		[Space]
		[SerializeField] private bool connectedToEngine = true;
		
		private RaycastHit _tireRay;
		private bool _grounded;
		private float _offset;
		private Vector3 _tireWorldVel;
		private Vector3 _lastPosition;
		private float _kmph;
		
		private float ContactDistance => restDistance - _offset + ContactDistanceDelta;
		private Vector3 TireCenterPosition => Vector3.down * (ContactDistance - radius);

		private const float ContactDistanceDelta = 0.05f;

#if UNITY_EDITOR
		private void OnValidate() {
			if (tireTransform == null) {
				return;
			}
			
			UpdateTirePosition();
		}
#endif

		private float GetSuspensionForce(float tireRayDistance, float directionalVelocity) {
			_offset = restDistance - tireRayDistance;
			return (_offset * strength) - (directionalVelocity * damper);
		}

		private void UpdateTirePosition() {
			tireTransform.localPosition = TireCenterPosition;
		}

		private float GetDirectionAlignment() {
			Vector3 tireNormalizedLocalForwardVector = tireTransform.localToWorldMatrix.MultiplyVector(Vector3.forward).normalized;
			Vector3 vehicleNormalizedLocalForwardVector = vehicleController.GetChassisVector().normalized;
			float dotProduct = Vector3.Dot(tireNormalizedLocalForwardVector, vehicleNormalizedLocalForwardVector);
			return Mathf.Abs(dotProduct);
		}

		public bool IsTireGrounded() {
			return _grounded = Physics.Raycast(transform.position, -transform.up, out _tireRay, ContactDistance + ContactDistanceDelta);
		}

		public void UpdateSuspension() {
			if (_grounded) {
				Vector3 springDir = tireTransform.up;
				_tireWorldVel = vehicleController.GetPointVelocity(tireTransform.position);
				float vel = Vector3.Dot(springDir, _tireWorldVel);
				float force = GetSuspensionForce(_tireRay.distance, vel);
				Vector3 suspensionForce = springDir * force;
				vehicleController.AddForceAtPosition(suspensionForce, tireTransform.position);
			}
			
			UpdateTirePosition();
		}

		public void UpdateSteering(float horizontalInput) {
			if (steeringTires) {
				float steeringAngle = maxSteerAngle * horizontalInput;
				tireTransform.localRotation = Quaternion.Euler(0f, steeringAngle, 0f);
			}

			Vector3 steeringDir = tireTransform.right;
            _tireWorldVel = vehicleController.GetPointVelocity(tireTransform.position);
            float steeringVel = Vector3.Dot(steeringDir, _tireWorldVel);
            float desiredVelChange = -steeringVel * GetDirectionAlignment();
            float desiredAccel = desiredVelChange / Time.fixedDeltaTime;
            Vector3 steeringForce = steeringDir * (mass * desiredAccel);
            vehicleController.AddForceAtPosition(steeringForce, tireTransform.position);
		}

		public void UpdateAcceleration() {
			if (!_grounded || !connectedToEngine) {
				return;
			}
			
			Vector3 accelDir = tireTransform.forward;
			float power = vehicleController.GetWheelPower();
			Vector3 accelerationForce = accelDir * power;
			vehicleController.AddForceAtPosition(accelerationForce, tireTransform.position);
		}
		
		public void CheckAndControlTireSmoke(float vehicleKmph) {
			if (!canProduceSmoke) {
				return;
			}
			
			float distanceMoved = (transform.position - _lastPosition).magnitude;
			float speedMps = distanceMoved / Time.fixedDeltaTime;
			_kmph = speedMps * 3.6f;
			_lastPosition = transform.position;

			if (Mathf.Abs(vehicleKmph - _kmph) > emitThreshold) {
				particle.Play();
			} else {
				particle.Stop();
			}
		}
		
#if UNITY_EDITOR
		private void OnDrawGizmos() {
			var vehicleLocalMatrix = transform.localToWorldMatrix;
			
			// Draw debug label
			Vector3 tireLocalPosition = tireTransform.localPosition;
			Vector3 textPosition = new Vector3(tireLocalPosition.x + 0.1f, tireLocalPosition.y + 0.5f, tireLocalPosition.z);
			Handles.matrix = vehicleLocalMatrix;
			Handles.Label(textPosition, $"Km/h: {_kmph}");
			
			if (Application.isPlaying) {
				return;
			}
			
			// Draw rest distance
			Gizmos.matrix = vehicleLocalMatrix;
			Gizmos.color = Color.white;
			Gizmos.DrawLine(Vector3.zero, Vector3.down * restDistance);
			
			// Draw wheel position
			Handles.color = Color.black;
			Handles.DrawWireDisc(TireCenterPosition, Vector3.right, radius);
		}
#endif
	}
}