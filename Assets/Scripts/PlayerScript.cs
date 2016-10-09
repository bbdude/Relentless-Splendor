using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
  PlatformCharacterController pcc;
  
  public Transform camera;
  
  float gravity = -25f;
	float runSpeed = 8f;
	float groundDamping = 20f; // how fast do we change direction? higher means faster
	float inAirDamping = 5f;
	float jumpHeight = 3f;
  float normalizedHorizontalSpeed = 0;
  
  int block;
  
  RaycastHit _lastCCHit;
  Vector3 _velocity;
  BlockManager bmgr;
  
	void Start () 
  {
    pcc = GetComponent<PlatformCharacterController>();
    bmgr = camera.GetComponent<BlockManager>();
    pcc.onControllerCollidedEvent += onControllerCollider;
		pcc.onTriggerEnterEvent += onTriggerEnterEvent;
		pcc.onTriggerExitEvent += onTriggerExitEvent;
	}
  
  void Update () 
  {    
    #region Camera
    camera.position = this.transform.position + new Vector3(0, 1.81f, -10);
    #endregion
    
    #region Movement stuff
    if( pcc.isGrounded )
			_velocity.y = 0;

		if( Input.GetKey( KeyCode.RightArrow ) || Input.GetKey( KeyCode.D ))
		{
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		}
		else if( Input.GetKey( KeyCode.LeftArrow ) || Input.GetKey( KeyCode.A ))
		{
			normalizedHorizontalSpeed = -1;
			if( transform.localScale.x > 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		}
		else
		{
			normalizedHorizontalSpeed = 0;
		}

		// we can only jump whilst grounded
		if( pcc.isGrounded && (Input.GetKeyDown( KeyCode.UpArrow ) || Input.GetKey( KeyCode.W )))
		{
			_velocity.y = Mathf.Sqrt( 2f * jumpHeight * -gravity );
		}

		// apply horizontal speed smoothing it. dont really do this with Lerp. Use SmoothDamp or something that provides more control
		var smoothedMovementFactor = pcc.isGrounded ? groundDamping : inAirDamping; // how fast do we change direction?
		_velocity.x = Mathf.Lerp( _velocity.x, normalizedHorizontalSpeed * runSpeed, Time.deltaTime * smoothedMovementFactor );

		// apply gravity before moving
		_velocity.y += gravity * Time.deltaTime;

		// if holding down bump up our movement amount and turn off one way platform detection for a frame.
		// this lets us jump down through one way platforms
		if( pcc.isGrounded && Input.GetKey( KeyCode.DownArrow ) )
		{
			_velocity.y *= 3f;
			pcc.ignoreOneWayPlatformsThisFrame = true;
		}

		pcc.move( _velocity * Time.deltaTime );

		// grab our current _velocity to use as a base for all calculations
		_velocity = pcc.velocity;
    #endregion
    
    #region Mouse stuff
    
    if (Input.GetMouseButton(0)) 
    {
      RaycastHit hit;
      Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
      bool hitSomething = false;
      if(Physics.Raycast (ray, out hit, 100))
      {
        if(hit.transform.tag == "Ground")
        {
          bmgr.OnHold(hit.transform.gameObject, this.transform.position);
          //Debug.Log ("Logged");
          hitSomething = true;
        }
      }
      if (!hitSomething)
      {
        bmgr.OnRelease();
      }
     }
     else if (Input.GetMouseButtonUp(0))
     {
       bmgr.OnRelease();
     }
    #endregion
  }
  
  void onControllerCollider( RaycastHit hit )
	{
		if( hit.normal.y == 1f )
			return;
	}

  void onTriggerEnterEvent( Collider col )
	{
		//Debug.Log( "onTriggerEnterEvent: " + col.gameObject.name );
	}

	void onTriggerExitEvent( Collider col )
	{
		//Debug.Log( "onTriggerExitEvent: " + col.gameObject.name );
	}
  
}
