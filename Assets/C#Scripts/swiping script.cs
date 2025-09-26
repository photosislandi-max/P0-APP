using System.Collections;                         // Needed for IEnumerator and Coroutines
using UnityEngine;                                // Unity engine base classes
using UnityEngine.EventSystems;                   // Gives access to drag events (OnBeginDrag, OnDrag, OnEndDrag)
using UnityEngine.UI;                             // Needed for UI elements like Image
// This script allows a UI object (like a card with an Image) to be swiped left or right
// If swiped far enough, it animates off-screen and fades away
public class swipingscript : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    // ---------------- VARIABLES ----------------
    public GameObject itsAMatch;
    public GameObject notForYou;
    private Vector3 _initialPosition;             // Stores the position of the card before dragging
    private float _distanceMoved;                 // How far the card moved during drag
    private bool _swipeLeft;                      // Tracks swipe direction (true = left, false = right)
    public int _finalCount = 7;                      // Counts if all cards have been swiped using the static counter

    // Called when dragging begins (mouse or finger down)
    public void OnBeginDrag(PointerEventData eventData)
    {
        // Save the starting position of the card
        _initialPosition = transform.localPosition;
    }
    private Vector3 _startPosition;

    public Vector3 GetStartPosition()
    {
        return _startPosition;
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
            /*
             * ------------------ COUNTER LOGIC ------------------
             * We update the Yes/No counters here before the card is destroyed.
             * The counters are stored in a separate static script called SwipeStats.cs
             * 
             * SwipeStats.cs looks like this:
             * 
             * public static class SwipeStats
             * {
             *     public static int YesCount = 0;
             *     public static int NoCount = 0;
             * }
             * 
             * - Because it's static, it lives in memory for the whole session.
             * - It does NOT go on any GameObject or Image in the scene.
             * - Every time a card is swiped, we increment the correct counter.
             * - The card itself will still get destroyed, but the numbers stay stored in SwipeStats.
             * ---------------------------------------------------
             */
            if (_swipeLeft)
            {
                SwipeStats.NoCount++;
                Debug.Log("No count: " + SwipeStats.NoCount);
            }
            else
            {
                SwipeStats.YesCount++;
                Debug.Log("Yes count: " + SwipeStats.YesCount);
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


        if (SwipeStats.YesCount + SwipeStats.NoCount == _finalCount)
        {
            if (SwipeStats.YesCount > SwipeStats.NoCount)
            {
                yesOrNoChanger("Its a Match Panel");
            }
            else
            {
                yesOrNoChanger("NotForYouPanel");
            }
        }
        // Once fully faded, disable 
        gameObject.SetActive(false);
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
    public void yesOrNoChanger(string nameOfPanel)
        {
            itsAMatch.SetActive(nameOfPanel == "Its a Match Panel");
            notForYou.SetActive(nameOfPanel == "NotForYouPanel");  
        }
}
