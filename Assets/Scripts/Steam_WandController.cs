using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class Steam_WandController : MonoBehaviour
{

    // Variables for checking the grip and the trigger buttons

//    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;

    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;

    // Get the correct device based on the index of the tracked object
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    // Reference to the tracked object
    private SteamVR_TrackedObject trackedObj;

    // Collection of the items that the controller is currently colliding with the controller at the same time
    HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();

    private InteractableItem closestItem;
    private InteractableItem interactingItem;

//  private Vector3 point;
//	private Transform pointTransform;
//	public GameObject cube;

    // Use this for initialization
    void Start()
    {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
    }

    // Update is called once per frame
    //Fixed update is based on time, better for rigidbody manipulation
    void Update()
    {
        if (controller == null) {
            Debug.Log("Controller not initialized");
            return;
        }
			

        // If the trigger button is pressed and the controller is touching/colliding with an object, the object moves with the hand. 
        if (controller.GetPressDown(triggerButton))
        {

            float minDistance = float.MaxValue;

            float distance;
            foreach (InteractableItem item in objectsHoveringOver)
            {
                distance = (item.transform.position - transform.position).sqrMagnitude;

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closestItem = item;
                }
            }

            interactingItem = closestItem;
            closestItem = null;		
            if (interactingItem) {
                if (interactingItem.IsInteracting()) {
                    //Ends interaction between the object and the first remote that was interacting
					interactingItem.EndInteraction(this);
                }
					
                interactingItem.BeginInteraction(this);
            }
        }

        // If you let go of the object, it no longer moves with the handset
        if (controller.GetPressUp(triggerButton) && interactingItem != null)
        {
            interactingItem.EndInteraction(this);
        }
			

    }

    private void OnTriggerEnter(Collider collider) {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem) {
            objectsHoveringOver.Add(collidedItem);
        }

    }

    private void OnTriggerExit(Collider collider) {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if(collidedItem) {
            objectsHoveringOver.Remove(collidedItem);
        }

    }
    public SteamVR_Controller.Device getController()
    {
        return controller;
    }
//    public Vector3 getPoint()
//    {
//        return point;
//    }
//	public Transform getPointTransform()
//	{
//		return pointTransform;
//	}
	public InteractableItem getInteractingItem() 
	{
		return interactingItem;
	}

}
