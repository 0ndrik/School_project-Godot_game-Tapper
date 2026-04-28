using Godot;
using System;

public partial class TableStart : Node2D
{
	public void _on_area_2d_area_entered(Area2D area)
	{
		if (area is EmptyGlass glass)
		{
			glass.Break();
		}
		else if (area is Customer customer)
		{
			 
		}
	}
}
