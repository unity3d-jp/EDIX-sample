using UnityEngine;

[RequireComponent (typeof (Collider))]
[RequireComponent (typeof (Renderer))]
public class Button : ActiveObject
{
	private bool on = false;
	
	
	void Start ()
	{
		UpdateVisuals ();
	}
	
	
	public override void Activate (Player player)
	{
		on = !on;
		UpdateVisuals ();
	}
	
	
	void UpdateVisuals ()
	{
		renderer.material.color = on ? Color.green : Color.red;
	}
}