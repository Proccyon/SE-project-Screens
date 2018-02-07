﻿using Sandbox.ModAPI.Ingame;

public Program()
{
	Runtime.UpdateFrequency = UpdateFrequency.Update1;
}


//---------------------------------//
//-----BeginGeneralDefinitions-----//
//---------------------------------//



public static bool CheckClose(Vector3D A, Vector3D B, double min)
{

	return (Distance(A, B) <= min);

}

public static Double Dot(Vector3D A, Vector3D B)
{
	return VRageMath.Vector3D.Dot(A, B);
}

public static Double Angle(Vector3D A, Vector3D B)
{
	return Math.Acos(Dot(A, B) / (A.Length() * B.Length()));
}



public static Double Distance(Vector3D A, Vector3D B)
{
	return VRageMath.Vector3D.Distance(A, B);
}


public static Double DistanceBlock(IMyTerminalBlock Block1, IMyTerminalBlock Block2)
{

	Vector3D Vector1 = Block1.GetPosition();
	Vector3D Vector2 = Block2.GetPosition();
	return Distance(Vector1, Vector2);

}


public static Vector3D Interpolate(Vector3D Vector1, Vector3D Vector2, double Length2) //Changes the length of a certain line to given value
{

	VRageMath.Vector3D Output = new VRageMath.Vector3D(0.0, 0.0, 0.0);
	double Length1 = Distance(Vector1, Vector2);
	Output.X = Vector2.X + (Vector2.X - Vector1.X) * ((Length2 - Length1) / Length1);
	Output.Y = Vector2.Y + (Vector2.Y - Vector1.Y) * ((Length2 - Length1) / Length1);
	Output.Z = Vector2.Z + (Vector2.Z - Vector1.Z) * ((Length2 - Length1) / Length1);
	return Output;
}

//-------------------------------//
//-----EndGeneralDefinitions-----//
//-------------------------------//

//--------------------------//
//-----BeginPongPhysics-----//
//--------------------------//

public class PongPhysics
{
	public Vector2D BallPosition;
	public Vector2D RightBatPosition;
	public Vector2D LeftBatPosition;
	public double RoomWidth;
	public double RoomHeight;
	public double BallRadius;
	public double BatHeight;
	public double XSpeed;
	public double YSpeed;
	public double StartSpeed;
	public double MaxAngle;
	public int LeftScore = 0;
	public int RightScore = 0;

	public PongPhysics(double BallRadius, double BatHeight, double MaxAngle, double StartSpeed)
	{
		this.BallRadius = BallRadius;
		this.BatHeight = BatHeight;
		this.StartSpeed = StartSpeed;
		this.MaxAngle = 2 * Math.PI * (MaxAngle / 360);
		this.SetBall();

	}

	public void SetBall()
	{

		this.BallPosition.X = this.RoomWidth / 2;
		this.BallPosition.Y = this.RoomHeight / 2;
		this.LeftBatPosition.Y = this.RoomHeight / 2;
		this.RightBatPosition.Y = this.RoomHeight / 2;

		//		double Angle = (this.MaxAngle / 9) * RandInt(); //Generate random angle to shoot ball
		//		this.YSpeed = Math.Sin(Angle) * this.StartSpeed;
		//		this.XSpeed = Math.Cos(Angle) * this.StartSpeed;

	}

	public void Run(double RoomWidth, double RoomHeight)
	{
		this.RoomWidth = RoomWidth;
		this.RoomHeight = RoomHeight;

		this.BallPosition.X += XSpeed / 1000;
		this.BallPosition.Y += YSpeed / 1000;

		if (this.BallPosition.Y + this.BallRadius >= this.RoomHeight)
		{
			this.YSpeed *= -1;
			this.BallPosition.Y = this.RoomHeight - this.BallRadius;
		}

		if (this.BallPosition.Y - this.BallRadius <= 0)
		{
			this.YSpeed *= -1;
			this.BallPosition.Y = this.BallRadius;
		}

		if (this.BallPosition.X + this.BallRadius <= 0)
		{
			this.LeftScore += 1;
			this.SetBall();
		}

		if (this.BallPosition.X - this.BallRadius >= this.RoomWidth)
		{
			this.RightScore += 1;
			this.SetBall();
		}


	}


}

//------------------------//
//-----StartGroupSaver-----//
//------------------------//

public class GroupSaver
{
	public bool Set = false;
	public int A;
	public int B;
	public double Height;
	public double Width;
	public List<int> IndexList;
	public List<Vector2D> PositionList;


	public GroupSaver()
	{


	}

}



//-----------------------//
//-----EndGroupSaver-----//
//-----------------------//




//------------------------//
//-----EndPongPhysics-----//
//------------------------//

//--------------------------//
//-----BeginScreenGroup-----//
//--------------------------//

public class screengroup
{
	public List<IMyTextPanel> BlockList = new List<IMyTextPanel>();
	public List<IMyTextPanel> OldList = new List<IMyTextPanel>();
	public List<screen> ScreenList = new List<screen>();
	public List<int> IndexList = new List<int>(); //List of indexes of all screens in screengroup
	public List<Vector2D> PositionList = new List<Vector2D>(); //List of relative Positions of all screens 
	public double Width;
	public double Height;
	public int A;
	public int B;
	public string Name;
	public GroupSaver Save;


	public screengroup(string Name, GroupSaver Save)
	{

		int i;
		this.Save = Save;


		if (this.Save.Set == false) //If Screens already found or not
		{


			program.GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(this.BlockList);

			this.OldList = new List<IMyTextPanel>(this.Blocklist);
			this.Name = Name;
			this.SetScreenList(); //Creates list of screens for each IMyTextPanel in BlockList
			this.FindA();   //Finds the index of the screen with name this.Name
			this.CleanLists();  //Cleans ScreenList of all screens not connected to Screen with index A

			this.SetBlockList();

			this.FindBottomLeft(); //Find index of screen in most bottom left corner
			this.FindSize(); //Finds the amount of screens in the width and height direction
			this.MarkPosition();    //Marks the relative position of each screen (ex. (0,0) for bottom left corner)
			this.SaveVariables();   //Saves all variables in the GroupSaver to use next tick
		}
		else
		{

			this.GetVariables(); //Get all variables saved in GroupSaver

			for (i = 0; i < this.IndexList.Count(); i++) //Put all the variables in usable format
			{
				this.BlockList.Add(blocklist[this.IndexList[i]]);
				this.ScreenList.Add(new screen(this.BlockList[i]));
				this.ScreenList[i].X = this.PositionList[i].X;
				this.ScreenList[i].Y = this.PositionList[i].Y;
			}

		}

	}

	public void SaveVariables() //Saves most variables in the GroupSaver
	{
		this.Save.Width = this.Width;
		this.Save.Height = this.Height;
		this.Save.A = this.A;
		this.Save.B = this.B;
		this.Save.IndexList = this.IndexList;
		this.Save.PositionList = this.PositionList;
		this.Save.Set = true;
	}

	public void GetVariables()
	{

		this.Width = this.Save.Width;
		this.Height = this.Save.Height;
		this.A = this.Save.A;
		this.B = this.Save.B;
		this.IndexList = this.Save.IndexList;
		this.PositionList = this.Save.PositionList;

	}

	public void SetBlockList()
	{
		int i;
		List<IMyTextPanel> NewList = new List<IMyTextPanel>();

		for (i = 0; i < this.IndexList.Count(); i++)
		{
			NewList.Add(this.OldList[IndexList[i]]);
		}

		this.BlockList = NewList;
	}

	public void SetScreenList() //Creates a list of screens based on this.BlockList
	{

		int i;
		for (i = 0; i < this.BlockList.Count(); i++)
		{
			this.ScreenList.Add(new screen(this.BlockList[i]));
			this.ScreenList[i].Index = i;
		}

	}

	public void CleanLists()    //Cleans all screens not connected to screen with index this.A
	{

		int i;
		List<screen> ToDoList = new List<screen>();
		List<screen> FoundList = new List<screen>();
		List<screen> ScreenList = new List<screen>(this.ScreenList);

		ToDoList.Add(ScreenList[this.A]);
		FoundList.Add(ScreenList[this.A]);
		this.IndexList.Add(ScreenList[this.A].Index);
		ScreenList.RemoveAt(this.A);
		this.A = 0;

		while (ToDoList.Count > 0)
		{

			for (i = 0; i < ScreenList.Count(); i++)
			{

				if (VRageMath.Vector3D.Distance(ToDoList[0].Position, ScreenList[i].Position) <= 2.7) //Use standart method!
				{
					ToDoList.Add(ScreenList[i]);
					FoundList.Add(ScreenList[i]);
					this.IndexList.Add(ScreenList[i].Index);
					ScreenList.RemoveAt(i);
					i -= 1;
				}

			}
			ToDoList.RemoveAt(0);
		}

		this.ScreenList = FoundList;
	}

	public void FindBottomLeft()    //Finds the index of the screen in the bottom left corner
	{
		int i;
		Vector3D Left = this.BlockList[this.A].WorldMatrix.Left;
		Vector3D Down = this.BlockList[this.A].WorldMatrix.Down;
		Vector3D B = this.BlockList[this.A].GetPosition();
		Vector3D C;

		for (i = 0; i < this.ScreenList.Count(); i++)
		{

			C = this.ScreenList[i].Position - B;
			if (Angle(C, Left) < 0.5 * Math.PI && Angle(C, Down) < 0.5 * Math.PI) //Can be Optimised
			{
				B = this.ScreenList[i].Position;
				this.B = i;
			}
		}
	}


	public void FindA() // Find the index of the screen with Name stored in this.Name
	{

		int i;
		for (i = 0; i < this.BlockList.Count(); i++)
		{

			if (this.BlockList[i].GetPublicText().Contains(this.Name)) //If Name is written on the screen
			{
				this.A = i;
				this.BlockList[i].CustomName = this.Name; //Set the Name of the screen to this.Name
				break;
			}

			if (this.BlockList[i].CustomName.Contains(this.Name))
			{
				this.A = i;
				break;
			}

		}
	}

	public void FindSize()  //Finds the amount of screens in the Height and width direction
	{

		Vector3D Up = this.BlockList[this.A].WorldMatrix.Up;
		Vector3D Right = this.BlockList[this.A].WorldMatrix.Right;

		bool xDone = false;
		bool yDone = false;
		int x = 0;
		int y = 0;
		int i;
		Vector3D Block = this.ScreenList[this.B].Position;

		while (xDone == false)
		{
			xDone = true;
			for (i = 0; i < this.ScreenList.Count(); i++)
			{
				if (CheckClose(Block + 2.5 * Right * x, this.ScreenList[i].Position, 0.2)) //Can be Optimised
				{
					xDone = false;
					x += 1;
					break;
				}

			}
		}

		while (yDone == false)
		{
			yDone = true;
			for (i = 0; i < this.ScreenList.Count(); i++)
			{
				if (CheckClose(Block + 2.5 * Up * y, this.ScreenList[i].Position, 0.2))
				{
					yDone = false;
					y += 1;
					break;
				}

			}

		}

		this.Width = x;
		this.Height = y;

	}

	public void MarkPosition()  //Marks the relative position of all screens (Ex. (0,0) for bottom left screen)
	{
		Vector3D Up = this.BlockList[this.A].WorldMatrix.Up;
		Vector3D Right = this.BlockList[this.A].WorldMatrix.Right;
		Vector3D Position;
		int x;
		int y;
		int i;

		for (x = 0; x < this.Width; x++)
		{
			for (y = 0; y < this.Height; y++)
			{
				Position = x * Right * 2.5 + y * Up * 2.5 + this.ScreenList[this.B].Position;

				for (i = 0; i < this.ScreenList.Count(); i++)
				{
					if (CheckClose(Position, this.ScreenList[i].Position, 0.2))
					{
						this.ScreenList[i].X = x;
						this.ScreenList[i].Y = y;
						if (this.ScreenList[i].Panel.CustomName.Contains("#") == false)
						{
							this.ScreenList[i].Panel.CustomName += " #(" + x.ToString() + "," + y.ToString() + ")";
						}
						break;

					}
				}
			}
		}

		Vector2D NewPosition;

		for (i = 0; i < ScreenList.Count(); i++)
		{
			NewPosition.X = ScreenList[i].X;
			NewPosition.Y = ScreenList[i].Y;
			PositionList.Add(NewPosition);
		}

	}

	public void DrawCircle(circle Circle)   //Draws a circle on the screenGroup
	{

		int i;
		double X = Circle.Centre.X; //The normal position of the circle
		double Y = Circle.Centre.Y;


		for (i = 0; i < this.ScreenList.Count(); i++)
		{

			Circle.Centre.X = X - this.ScreenList[i].X; //Corrected position based on place in Group
			Circle.Centre.Y = Y - this.ScreenList[i].Y;
			ScreenList[i].DrawCircle(Circle);


		}
	}
}


//------------------------//
//-----EndScreenGroup-----//
//------------------------//

//---------------------//
//-----BeginCircle-----//
//---------------------//

public class circle
{

	public Vector2D Centre;
	public double R;
	public string Symbol;

	public circle(double radius, Vector2D centre, string symbol = "I")
	{
		this.R = radius;
		this.Centre = centre;
		this.Symbol = symbol;
	}


	public bool WithinCircle(double X, double Y)
	{

		Vector2D XY = new VRageMath.Vector2D(X, Y);
		return (VRageMath.Vector2D.Distance(XY, Centre) <= R);

	}

}

//-------------------//
//-----EndCircle-----//
//-------------------//

//--------------------------//
//-----BeginScreenSaver-----//
//--------------------------//


public class ScreenSaver
{

	public string Text;
	public int ImageNumber; //How much images are drawn
	public bool IsDrawing = false;
	public double YStart;   //Where to start drawing
	public double XStart;   //idem

}



//------------------------//
//-----EndScreenSaver-----//
//------------------------//




//---------------------//
//-----BeginScreen-----//
//---------------------//

public class screen
{

	public ScreenSaver Save;
	public double Font;
	public int Height;
	public int Width;
	public string Text;
	public IMyTextPanel Panel;
	public double X; //Position for screengroup
	public double Y; //Position for Screengroup
	public double XStart = 0;
	public double YStart = 0;
	public Vector3D Position;
	public int Index;   //The index in the list of IMyTextPanels
	public int ImageNumber; //How much images are drawn

	public screen(IMyTextPanel panel, ScreenSaver Save)
	{



		this.Save = Save;
		this.Font = panel.FontSize;
		this.Height = (int)(Math.Round(18 / panel.FontSize, 0));
		this.Width = (int)(Math.Round(73 / panel.FontSize, 0));
		this.Panel = panel;
		this.Position = panel.GetPosition();

		if (this.Save.IsDrawing)
		{
			this.Text = this.Save.Text;
			this.XStart = this.Save.XStart;
			this.YStart = this.Save.Ystart;
		}
		else
		{
			this.FillScreen(" ");
			this.XStart = 0.5 / (float)this.Width;
            this.YStart = 1 - 0.5 / (float)this.Height;
		}

		panel.ShowPublicTextOnScreen();
	}


	public void Drawtext()
	{
		this.Panel.WritePublicText(this.Text, false);
	}


	public void FillScreen(string symbol)
	{
		int y;
		this.Text = "";
		for (y = 0; y < this.Height; y++)
		{
			this.Text += new String(symbol[0], this.Width) + "\n";
		}
	}


	public void DrawCircle(circle Circle)
	{

		//Optimisation
		if (Circle.Centre.X - Circle.R <= 1 && Circle.Centre.X + Circle.R >= 0 && Circle.Centre.Y - Circle.R <= 1 && Circle.Centre.Y + Circle.R >= 0)
		{
			string NewText = "";
			int i = 0;
			double LetterWidth = 1 / (float)this.Width; //Width of a letter
			double LetterHeight = 1 / (float)this.Height; //Height of a letter
			double x;
			double y;

			for (y = this.YStart; y > 0; y -= LetterHeight)
			{

				for (x = this.XStart; x < 1; x += LetterWidth)
				{

					//Optimisation
					if ((y > Circle.Centre.Y + Circle.R || y < Circle.Centre.Y - Circle.R) || (x > Circle.Centre.X + Circle.R || x < Circle.Centre.X - Circle.R))
					{
						NewText += this.Text[i];
					}
					else
					{
						if (Circle.WithinCircle(x, y))
						{
							NewText += Circle.Symbol;
						}
						else
						{
							NewText += this.Text[i];
						}
					}

					i += 1;

				}
				NewText += "\n";
				i += 1;
			}

			this.Text = NewText;
			this.Drawtext();
		}
		else
		{
			this.Text = "";
			this.Drawtext();
		}
	}

}

//-------------------//
//-----EndScreen-----//
//-------------------//

//------------------------------//
//-----BeginGlobalVariables-----//
//------------------------------//

public static int ActionCount = 24999; // SE only allows 25000 method calls per tick


PongPhysics Pong = new PongPhysics(0.2, 1, 45, 30);

Vector2D Zero = new VRageMath.Vector2D(0.0, 0.0);
GroupSaver Save = new GroupSaver();
static Program program;


//----------------------------//
//-----EndGlobalVariables-----//
//----------------------------//

//-------------------//
//-----BeginMain-----//
//-------------------//

void Main(string argument, UpdateType updateSource)
{


	program = this;


	List<IMyTextPanel> Iets = new List<IMyTextPanel>();
	GridTerminalSystem.GetBlocksOfType<IMyTextPanel>(Iets);

	screengroup Group = new screengroup("ScreenA", Save);

	//Pong.Run(Group.Width, Group.Height);
	circle Circle = new circle(1, Zero);
	Group.DrawCircle(Circle);


	//screen LCD = new screen(Iets[6]);
	//LCD.DrawCircle(MiddleCircle);
	//LCD.Drawtext();


}

//-------------------//
//-----EndMain-----//
//-------------------//




