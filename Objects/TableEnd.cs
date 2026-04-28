using Godot;
using System;

public partial class TableEnd : Node2D
{
	private void _on_area_2d_area_entered(Area2D area)
	{
		if (area is Beer beer)
		{
			beer.End();
		}

		else if (area is Customer customer)
		{
			customer.GotOut();
		}
	}
}
