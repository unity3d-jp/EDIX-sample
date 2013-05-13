using UnityEngine;
using System.Collections;

public class Crystal : ActiveObject
{
	public Renderer availableVisuals, spentVisuals;
	public bool spent = false;
	
	
	void Start ()
	{
		UpdateVisuals ();
	}
	
	
	public override void Activate (Player player)
	{
		if (spent || !player.AddCargo ())
		{
			return;
		}
		
		spent = true;
		UpdateVisuals ();
	}
	
	
	void UpdateVisuals ()
	{
		availableVisuals.enabled = !spent;
		spentVisuals.enabled = spent;
	}
}
