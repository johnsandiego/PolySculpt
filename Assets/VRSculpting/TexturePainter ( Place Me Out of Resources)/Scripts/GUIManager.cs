using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace sandiegoJohn.VRsculpting{

	public class GUIManager : MonoBehaviour {
		public Text guiTextMode;
		public Slider sizeSlider;
		public VRTexturePainter painter;

		public void SetBrushMode(int newMode){
			Painter_BrushMode brushMode =newMode==0? Painter_BrushMode.DECAL:Painter_BrushMode.PAINT; //Cant set enums for buttons :(
			string colorText=brushMode==Painter_BrushMode.PAINT?"orange":"purple";	
			guiTextMode.text="<b>Mode:</b><color="+colorText+">"+brushMode.ToString()+"</color>";
			painter.SetBrushMode (brushMode);
		}
		public void UpdateSizeSlider(){
			painter.SetBrushSize (sizeSlider.value);
		}
	}
}