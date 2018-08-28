using System.Collections.Generic;
using System;
using System.Collections.ObjectModel;
using CivOne.Tiles;
using CivOne.Enums;
using CivOne;



public class AStarInterface : AStar
{
	// Interface for use in CivOne

	sPosition GoalPosition;

	ICollection<AStar.sPosition> path = new Collection<AStar.sPosition>();

	public sPosition FindPath(sPosition StartPosition, sPosition GoalPosition)
	{
		this.GoalPosition = GoalPosition;
		bool o = FindPath(path, StartPosition, 500);       // Find path
		AStar.sPosition[] Positions = new sPosition[500];        // Get path	
		int iCount = path.Count;
		path.CopyTo( Positions, 0 );
		if (!o)
		{
			Positions[ iCount - 1 ].iX = -1;          // unable to find path
		}
		return Positions[ iCount - 1 ];
	}

	/// Return cost of making a step from Positition to NextPosition (which are neighbors).
	/// Cost equal to float.PositiveInfinity indicates that passage from Positition to NextPosition is impossible.
	/// Currently is cost of current position ignored
	/// 
	protected override float Cost( sPosition Positition, sPosition NextPosition )
	{
		float fCost;

		float fNextCost = Map.Instance.GetMoveCost( NextPosition.iX, NextPosition.iY );
		fCost = Map.Instance.GetMoveCost( Positition.iX, Positition.iY );
		if( fNextCost == 1f && fCost == 1f ) return fNextCost;      // if going along a road/railroad 
		else if( fNextCost == 1f ) return 3f;               // if moving from terrain to road/railroad   (  dont know if this is correct  )
		else return fNextCost;
	}


	/// Return an estimate of cost of moving from Positition to goal.
	/// Return 0 when Positition is goal.
	/// This is an estimate of sum of all costs along the path between Positition and the goal.
	static float fGoalF = 1.0f;
	protected override float Heuristic( sPosition Positition)
	{
		if( Math.Abs( Positition.iX - GoalPosition.iX ) > Math.Abs( Positition.iY - GoalPosition.iY ))
			return Math.Abs(( Positition.iX - GoalPosition.iX ) * fGoalF );
		else
			return Math.Abs(( Positition.iY - GoalPosition.iY ) * fGoalF );

	}


	protected override void Neighbors( Node CurrNode, List<Node> neighbors )
	{
		int[,] aiRelPos = new int[,] { { -1, -1 }, { -1, 0 }, { -1, 1 }, { 0, -1 }, { 0, 1 }, { 1, -1 }, { 1, 0 }, { 1, 1 } };

		for (int i = 0; i < 8; i++)
		{
			sPosition CurPosition = CurrNode.Position;
			sPosition NewPosition;

			NewPosition.iX = CurPosition.iX + aiRelPos[ i, 0 ];
			NewPosition.iY = CurPosition.iY + aiRelPos[ i, 1 ];
			Node NewNode = new Node();
			NewNode.Position = NewPosition;
			NewNode.IsClosed = false;
			NewNode.PreviousPosition = CurPosition;
			neighbors.Add( NewNode );
		}
	}
}


