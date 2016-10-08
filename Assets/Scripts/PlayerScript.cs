using UnityEngine;
using System.Collections;

public class PlayerScript : MonoBehaviour 
{
  
  PlatformCharacterController pcc;
  
  float gravity = -25f;
	float runSpeed = 8f;
	float groundDamping = 20f; // how fast do we change direction? higher means faster
	float inAirDamping = 5f;
	float jumpHeight = 3f;
  float normalizedHorizontalSpeed = 0;
  
  RaycastHit _lastCCHit;
  Vector3 _velocity;
  
	void Start () 
  {
    pcc = GetComponent<PlatformCharacterController>();
    pcc.onControllerCollidedEvent += onControllerCollider;
		pcc.onTriggerEnterEvent += onTriggerEnterEvent;
		pcc.onTriggerExitEvent += onTriggerExitEvent;
	}
  
  void Update () 
  {    
    if( pcc.isGrounded )
			_velocity.y = 0;

		if( Input.GetKey( KeyCode.RightArrow ) )
		{
			normalizedHorizontalSpeed = 1;
			if( transform.localScale.x < 0f )
				transform.localScale = new Vector3( -transform.localScale.x, transform.localScale.y, transform.localScale.z );
		}
		else if( Input.GetKey( KeyCode.LeftArrow ) )
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
		if( pcc.isGrounded && Input.GetKeyDown( KeyCode.UpArrow ) )
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
