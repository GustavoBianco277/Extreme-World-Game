using UnityEngine;

public class VectDirect  {

	public Vector3 CalcVect(Vector3 mousPosition,float maxAngle){
		var maxX=50F*Mathf.Tan (maxAngle);
		var maxY=maxX;
		var mousPosX=(mousPosition.x*maxX)/(Screen.width/2F);
		var mousPosY=(mousPosition.y*maxY)/(Screen.height/2F);
		var tempVect=new Vector3(mousPosX,mousPosY,50F);
		return tempVect;
	}
	
}
