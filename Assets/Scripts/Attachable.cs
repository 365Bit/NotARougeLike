using UnityEngine;

public class Attachable : MonoBehaviour
{
	public Transform attachedTo = null;

	public void OnAttach(AttachedObjects target) {
		attachedTo = target.gameObject.transform;

        if (TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (TryGetComponent<Collider>(out Collider c)) {
            c.enabled = false;
        }
	}

	public void OnDetach() {
		attachedTo = null;

        if (TryGetComponent<Collider>(out Collider c)) {
            c.enabled = true;
        }

        if (TryGetComponent<Rigidbody>(out Rigidbody rb)) {
            rb.isKinematic = false;
        }
	}
}

