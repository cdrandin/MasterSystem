using UnityEngine;
using System.Collections;

public class HandleInputs : MonoBehaviour {
	private enum InputType{

		TouchInput,
		MouseInput

	};

	private static InputType m_currentInput;
	
	public static Vector2 m_trail_center_position;
	
	// Use this for initialization
	void Awake () {	

		m_trail_center_position = Vector2.zero;
	#if UNITY_STANDALONE || UNITY_EDITOR || UNITY_EDITOR_WIN

		m_currentInput = InputType.MouseInput;
	#else
		m_currentInput = InputType.TouchInput;
	#endif
	}

	public static object CheckIfPressed(){
				
		switch(m_currentInput){
			//Touch Case			
		case InputType.TouchInput:
			
			if( Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began){
				
				//Modify y value since input origin is at the bottom left
				Vector2 coordinates = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);

				m_trail_center_position = coordinates;
				
				return collideWithObject( coordinates );
			}
			break;
			
			//Mouse Case
		case InputType.MouseInput:
						
			if( Input.GetMouseButtonDown(0) ){

				Vector2 coordinates = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				m_trail_center_position = coordinates;

				return collideWithObject( coordinates);
			}
			
			break;
		}
		
		return null;
	}

	private static object collideWithObject(Vector2 point){

		Ray ray = Camera.main.ScreenPointToRay(point);

		if(Input.GetMouseButtonUp(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended))
		{
			RaycastHit[] hits = Physics.RaycastAll(ray, 200.0f);
			foreach(RaycastHit h in hits)
			{
				if(h.transform.gameObject.layer == LayerMask.NameToLayer("Player") ||
				   h.transform.gameObject.layer == LayerMask.NameToLayer("Ally") ||
				   h.transform.gameObject.layer == LayerMask.NameToLayer("Enemy") )
				{
//					Debug.Log(string.Format("HIT: {0}", h.transform.name));
					return h.transform.tag;
					 //break;
				}
			}
		}

		RaycastHit hit;

		if (Physics.Raycast(ray, out hit )){
//			Debug.Log(string.Format("HIT >>: {0}", hit.transform.name));
			return hit.transform.tag;
		}

		return null;

	}

	//Check if Holding
	public static object CheckIfHolding(){


		switch(m_currentInput){
			//Touch case
			case InputType.TouchInput:
				if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Stationary){

					Vector2 coordinates = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
					m_trail_center_position = coordinates;
					return collideWithObject( coordinates );
				}
			break;
			//Mouse Case
			case InputType.MouseInput:
				if( Input.GetMouseButton(0) && Input.GetAxisRaw("Mouse X") == 0 && Input.GetAxisRaw("Mouse Y") == 0 ){
					
					Vector2 coordinates = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
					m_trail_center_position = coordinates;
					return collideWithObject( coordinates );
				}



			break;
		}
		return false;

	}
	//Check if swipping
	public static bool CheckIfSwipping(){

		switch(m_currentInput){

			//Touch case
			case InputType.TouchInput:
				return(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Moved);
			//Mouse Case
			case InputType.MouseInput:
				return(Input.GetMouseButton(0) && (Input.GetAxisRaw("Mouse X")!= 0 || Input.GetAxisRaw("Mouse Y")!=0  ));
			default:
				return false;

			//end switch case
			}
	}


	//Check if release
	public static object CheckIfReleased(){
		switch(m_currentInput){
			
			//Touch Case
			case InputType.TouchInput:
				if(Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended){
					
					Vector2 coordinates = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
					m_trail_center_position = coordinates;
					return collideWithObject( coordinates );
				}
			break;

			//Mouse Case
			case InputType.MouseInput:	
			if( Input.GetMouseButtonUp(0)){
				
				Vector2 coordinates = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
				m_trail_center_position = coordinates;
				return collideWithObject( coordinates );
			}
			
			break;
		}

		return false;
		
	}
}
