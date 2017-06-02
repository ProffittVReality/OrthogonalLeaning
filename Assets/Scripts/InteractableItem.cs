using UnityEngine;
using System.Collections;

public class InteractableItem : MonoBehaviour {
    public Rigidbody rigidbody;

    private bool currentlyInteracting;

    private float velocityfactor = 20000f;
    private Vector3 postDelta; 

    private float rotationfactor = 600f;
    private Vector3 posDelta;
    private Quaternion rotationDelta;
    private float angle;
    private Vector3 axis; 

    private Steam_WandController attachedWand;

    private Transform interactionPoint; 

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        interactionPoint = new GameObject().transform;
        // larger the mass of the rigid body, the harder the object is to move
        velocityfactor /= rigidbody.mass;
    }

    // Update is called once per frame
    // Update the velocity of the object
    void Update () {
	    if (attachedWand && currentlyInteracting) {
            //Distance between the hand and the object
            posDelta = attachedWand.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta * velocityfactor * Time.fixedDeltaTime;

            rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if(angle > 180) {
                angle -= 360;
            }

            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationfactor;
         
        }
	}

    public void BeginInteraction(Steam_WandController wand) {
        attachedWand = wand;
        interactionPoint.position = wand.transform.position;
        interactionPoint.rotation = wand.transform.rotation;
        interactionPoint.SetParent(transform, true);

        currentlyInteracting = true;
    }

    public void EndInteraction(Steam_WandController wand)
    {
        if(wand == attachedWand) {
            attachedWand = null;
            currentlyInteracting = false;
        }
    }

    public bool IsInteracting() {
        return currentlyInteracting;
    }
}
