using UnityEngine;
using System.Collections;

public enum InputType
{
	UNDEFINED,
	TOUCH_INPUT,
	MOUSE_INPUT
};

public class InputWrapper
{
	static private bool _init;
	static private InputType _input_type;  // Don't touch! Super secret
	static private InputType input_type
	{
		get
		{
			// Was overwrriten on start
			if(_input_type != InputType.UNDEFINED)
			{
				_init = true;
			}

			// Figure out which input type to use
			else
			{
				if(!_init)
				{
					_init = true;
					ConfigureInputType();
				}
			}

			return _input_type;
		}

		set { _input_type = value; }
	}

	static public InputType GetInputType()
	{
		return input_type;
	}

	/// <summary>
	/// Overrides the type of the input.
	/// </summary>
	/// <param name="type">Type.</param>
	static public void OverrideInputType(InputType type)
	{
		input_type = type;
	}

	/// <summary>
	/// Configures the type of the input. Meaning it will revert back to which ever input type depending on the device
	/// </summary>
	static public void ConfigureInputType()
	{
		#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN
		_input_type = InputType.MOUSE_INPUT;
		
		#elif UNITY_IPHONE || UNITY_ANDROID
		_input_type = InputType.TOUCH_INPUT;
		#else
		_input_type = InputType.UNDEFINED;
		Debug.LogError("The device input will not work!");
		#endif
	}

	/// <summary>
	// Determine if the left mouse button was lifted up or if a finger was lifted up
	/// </summary>
	/// <returns><c>true</c>, if input up was gotten, <c>false</c> otherwise.</returns>
	static public bool GetInputUp()
	{
		return (input_type == InputType.MOUSE_INPUT)? Input.GetMouseButtonUp(0) : (input_type == InputType.TOUCH_INPUT) ? Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended : false;
	}

	/// <summary>
	/// Gets the input down.
	/// </summary>
	/// <returns><c>true</c>, if input down was gotten, <c>false</c> otherwise.</returns>
	static public bool GetInputDown()
	{
		return (input_type == InputType.MOUSE_INPUT)? Input.GetMouseButtonDown(0) : (input_type == InputType.TOUCH_INPUT) ? Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began : false;
	}

	/// <summary>
	/// Determine if either a mouse was click or if a finger was touched.
	/// This combines Input.GetMouseButton(0)  and Input.touchCount > 0
	/// </summary>
	/// <returns><c>true</c>, if input was anyed, <c>false</c> otherwise.</returns>
	static public bool AnyInput()
	{
		return Input.GetMouseButton(0) || Input.touchCount > 0;
	}

	/// <summary>
	/// Gets the input screen position. For touch based input, when you lift your finger the position where be the last location where a finger was lifted from.
	/// The dimension are the pixels of the resolution of the device screen.
	/// Suggested use: Camera.main.ScreenPointToRay(GetInputScreenPosition());
	/// </summary>
	/// <returns>The input screen position or returns new Vector2(float.NaN, float.NaN) to signified failure.</returns>
	static public Vector2 GetInputScreenPosition()
	{
		return (input_type == InputType.MOUSE_INPUT)? new Vector2(Input.mousePosition.x, Input.mousePosition.y) : (input_type == InputType.TOUCH_INPUT) ? Input.GetTouch(0).position : new Vector2(float.NaN, float.NaN);
	}

	/// <summary>
	/// Gets the input delta position.
	/// </summary>
	/// <returns>The input delta position.</returns>
	static public Vector2 GetInputDeltaPosition()
	{
		return (input_type == InputType.MOUSE_INPUT)? new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y")) : (input_type == InputType.TOUCH_INPUT) ? Input.GetTouch(0).deltaPosition : new Vector2(float.NaN, float.NaN);
	}

	/// <summary>
	/// Gets the input scroll delta. Only works if the current input type is InputType.MOUSE_INPUT.
	/// </summary>
	/// <returns>The input scroll delta.</returns>
	static public float GetInputScrollDelta()
	{
		return Input.GetAxisRaw("Mouse ScrollWheel");
	}

	/// <summary>
	/// Gets the input touch pinch delta. Only works if the current input type is InputType.TOUCH_INPUT.
	/// </summary>
	/// <returns>The input touch pinch delta.</returns>
	static public float GetInputTouchPinchDelta()
	{
		if(Input.touchCount == 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);
			
			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
			
			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;
			
			// Find the difference in the distances between each frame.
			return prevTouchDeltaMag - touchDeltaMag;
		}

		return 0.0f;
	}

	/// <summary>
	/// Gets if input is moving.
	/// </summary>
	/// <returns><c>true</c>, if input is moving was gotten, <c>false</c> otherwise.</returns>
	static public bool GetInputIsMoving()
	{
		return (input_type == InputType.MOUSE_INPUT)? Input.GetMouseButton(0) && (Mathf.Abs(Input.GetAxisRaw("Mouse X")) > 0.1f || Mathf.Abs(Input.GetAxisRaw("Mouse Y")) > 0.1f) : (input_type == InputType.TOUCH_INPUT) ? Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Moved : false;
	}

	static public bool isMouse()
	{
		return input_type == InputType.MOUSE_INPUT;
	}

	static public bool isTouch()
	{
		return input_type == InputType.TOUCH_INPUT;
	}
}
