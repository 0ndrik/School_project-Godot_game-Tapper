using Godot;
using System;

public partial class TableStart : Node2D
{
	public void _on_glass_end_area_entered(Area2D area)
	{
		if (area is EmptyGlass glass)
		{
			glass.Break();
		}
	}

	public void _on_customer_end_area_entered(Area2D area)
	{
		if (area is Customer customer)
		{
			customer.GotToTheStart();
		}
	}
}
