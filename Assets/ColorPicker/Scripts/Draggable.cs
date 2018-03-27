using UnityEngine;

namespace sandiegoJohn.VRsculpting{

	public class Draggable : MonoBehaviour
	{
		PaintMeshDeformer PMD;
		public bool fixX;
		public bool fixY;
		public Transform thumb;	
		bool dragging;
		public Transform raypoint;
		public Transform minBound;

		void Start(){
			PMD = GameObject.FindGameObjectWithTag ("PMD").GetComponent<PaintMeshDeformer> ();
		}

		void FixedUpdate()
		{
			Debug.DrawRay (raypoint.transform.position, raypoint.transform.forward);
			if (PMD._control.touchpadPressed) {
				//Debug.Log ("color picker");
				dragging = false;
				Ray ray = new Ray (raypoint.transform.position, raypoint.transform.forward);
				//var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (GetComponent<BoxCollider>().Raycast(ray, out hit, 100)) {
					dragging = true;
					//Debug.Log ("color picker choosing");
				}
			}
			if (!PMD._control.touchpadPressed) dragging = false;
			if (dragging && PMD._control.touchpadPressed){ //&& PMD._control.triggerTouched) {
				//var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Ray ray = new Ray (raypoint.transform.position, raypoint.transform.forward);
				//var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				RaycastHit hit;
				if (GetComponent<BoxCollider>().Raycast(ray, out hit, 100)) {		
					Vector3 point = hit.point;
					//Debug.Log (hit.collider.name);
					//point = GetComponent<Collider>().ClosestPointOnBounds(point);
					SetThumbPosition(point);
					SendMessage("OnDrag", Vector3.one - (thumb.localPosition - minBound.localPosition) / GetComponent<BoxCollider>().size.x);
				}
			}
		}

		void SetDragPoint(Vector3 point)
		{
			point = (Vector3.one - point) * GetComponent<Collider>().bounds.size.x + GetComponent<Collider>().bounds.min;
			SetThumbPosition(point);
		}

		void SetThumbPosition(Vector3 point)
		{
			Vector3 temp = thumb.localPosition;
			thumb.position = point;
			thumb.localPosition = new Vector3(fixX ? temp.x : thumb.localPosition.x, fixY ? temp.y : thumb.localPosition.y, thumb.position.z - 1);
		}
	}

}