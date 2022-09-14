using UnityEngine;

namespace InTheShadow
{
	public class GameOverShowMenu : _GameState
	{
		public override void InitState(GameManager gameManager)
		{
			base.InitState(gameManager);
			State = GameManager.GameState.GameOverShowMenu;
		}

		public override void StartState()
		{
			
		}
		
		public override void StateUpdate()
		{
			Debug.Log("GAME OVER MENU");
		}
	}
}