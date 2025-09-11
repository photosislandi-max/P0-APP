using System.Collections;                         // Needed for IEnumerator and Coroutines
using System.Collections.Generic;                 // Generic collections (not used here, but often included by default)
using UnityEngine;                                // Unity engine base classes
using UnityEngine.EventSystems;                   // Gives access to drag events (OnBeginDrag, OnDrag, OnEndDrag)
//using static System.Net.Mime.MediaTypeNames;    // Commented out: would import media type names if needed
using UnityEngine.UI;                             // Needed for UI elements like Image

// This script allows a UI object (like a card with an Image) to be swiped left or right
// If swiped far enough, it animates off-screen and fades away
public class swipingscript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // ---------------- VARIABLES ----------------
    private Vector3 _initialPosition;             // Stores the position of the card before dragging
    private float _distanceMoved;                 // How far the card moved during drag
    private bool _swipeLeft;                      // Tracks swipe direction (true = left, false = right)

    // Called when dragging begins (mouse or finger down)
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save the starting position of the card
        _initialPosition = transform.localPosition;
    }

    // Called continuously while dragging
    public void OnDrag(PointerEventData eventData)
    {
        // Move the card horizontally by how much the pointer moved since last frame (eventData.delta.x)
        // Keep the same Y position so it only moves left/right
        transform.localPosition = new Vector2(
            x: transform.localPosition.x + eventData.delta.x,
            y: transform.localPosition.y
        );
    }

    // Called when the drag ends (mouse/finger released)
    public void OnEndDrag(PointerEventData eventData)
    {
        // Calculate absolute horizontal distance moved from starting position
        _distanceMoved = Mathf.Abs(transform.localPosition.x - _initialPosition.x);

        // If card moved less than 40% of the screen width, snap it back
        if (_distanceMoved < 0.2 * Screen.width)
        {
            transform.localPosition = _initialPosition;
        }
        else
        {
            // Otherwise decide swipe direction
            if (transform.localPosition.x > _initialPosition.x)
            {
                _swipeLeft = false;  // Swiped to the right
            }
            else
            {
                _swipeLeft = true;   // Swiped to the left
            }

            // Start the animation coroutine to move the card away
            StartCoroutine(routine: MovedCard());
        }
    }

    // Coroutine: moves the card off-screen while fading it out
    private IEnumerator MovedCard()
    {
        float time = 0;  // Timer for smooth animation

        // Continue until the Image color equals fully transparent white (alpha = 0)
        while (GetComponent<Image>().color != new Color(r: 1, g: 1, b: 1, a: 0))
        {
            // Increase timer by time passed since last frame
            time += Time.deltaTime;

            if (_swipeLeft)
            {
                // If swiping left → move card smoothly toward left side of screen
                transform.localPosition = new Vector3(
                    Mathf.SmoothStep(transform.localPosition.x,
                                     to: transform.localPosition.x - Screen.width,
                                     t: 2 * time),
                    transform.localPosition.y,
                    z: 0
                );
            }
            else
            {
                // If swiping right → (currently the same code, still subtracts Screen.width)
                transform.localPosition = new Vector3(
                    Mathf.SmoothStep(transform.localPosition.x,
                                     to: transform.localPosition.x + Screen.width,
                                     t: 2 * time),
                    transform.localPosition.y,
                    z: 0
                );
            }

            // Fade alpha from 1 (visible) to 0 (invisible) over time
            GetComponent<Image>().color = new Color(
                r: 1, g: 1, b: 1,
                a: Mathf.SmoothStep(from: 1, to: 0, t: 2 * time)
            );

            yield return null; // Wait until next frame before continuing
        }

        // Once fully faded, destroy the card object
        Destroy(gameObject);
    }

    // Unity lifecycle method: called once before the first Update
    void Start()
    {
        // Nothing here (can be removed if unused)
    }

    // Unity lifecycle method: called once per frame
    void Update()
    {
        // Nothing here (can be removed if unused)
    }
}
