using UnityEngine;

public class AttachedObjects : MonoBehaviour
{
    public int maximumAttachedObjects = 10;
    public SkinnedMeshRenderer mesh;
 
    // 
    private int index = 0;
    private Transform[] bones;
    private Transform[] attachedObjects;
    private Vector3[] offsets;
    private Quaternion[] rotations;
 
    bool visible = true;
 
    void OnBecameInvisible() {
        visible = false;
    }
 
    void OnBecameVisible() {
        visible = true;
    }
 
    void Start() {
        bones = new Transform[maximumAttachedObjects];
        attachedObjects = new Transform[maximumAttachedObjects];
 
        offsets = new Vector3[maximumAttachedObjects];
        rotations = new Quaternion[maximumAttachedObjects];
    }
 
    public void Attach(Transform obj) {
        if (mesh == null) return;

        Transform bone = mesh.bones[0];
        float sqrDist = float.MaxValue;
        foreach (var b in mesh.bones) {
            float d = b.InverseTransformPoint(obj.position).sqrMagnitude;
            if (d < sqrDist) {
                bone = b;
                sqrDist = d;
            }
        }

        // if overwriting, detach first
        if (attachedObjects[index] != null)
            OnDetach(attachedObjects[index]);

        attachedObjects[index] = obj;
        bones[index] = bone;

        offsets[index] = bone.InverseTransformPoint(obj.position);
        rotations[index] =  Quaternion.Inverse(bone.rotation) * obj.rotation;

        // call attachement function
        OnAttach(attachedObjects[index]);

        index = (index + 1) % maximumAttachedObjects;
    }

    void OnAttach(Transform obj) {
	    if (obj.TryGetComponent<Attachable>(out Attachable a)) {
	    	a.OnAttach(this);
	    }
    }

    void OnDetach(Transform obj) {
	    if (obj.TryGetComponent<Attachable>(out Attachable a)) {
	    	a.OnDetach();
	    }
    }

    // Update is called once per frame
    void Update()
    {
        if (!visible) return;

        for (int i = 0; i < maximumAttachedObjects; i++) {
            if (attachedObjects[i] == null || bones[i] == null) continue;

            attachedObjects[i].SetPositionAndRotation(bones[i].TransformPoint(offsets[i]),  bones[i].rotation * rotations[i]);
        }
    }

    void OnDestroy() {
        for (int i = 0; i < maximumAttachedObjects; i++) {
            if (attachedObjects[i] == null || bones[i] == null) continue;

            OnDetach(attachedObjects[i]);
        }
    }
}

