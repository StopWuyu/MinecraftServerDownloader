using Godot;
using System;

public partial class Menu : ItemList
{
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        var items = GetNode<ItemList>("../ItemList2");

        GetNode<ItemList>(".").ItemSelected += (long index) =>
        {
			switch (index)
			{
				case 0:

                    items.Clear();

                    items.AddItem("我是你爹");

                    break;
            }
		};
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
