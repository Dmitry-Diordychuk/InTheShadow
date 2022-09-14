using UnityEngine;

namespace InTheShadow
{
	public class _GameState: MonoBehaviour
	{
		public GameManager.GameState State { get; protected set; }

		protected GameManager gameManager;

		public virtual void InitState(GameManager gameManager)
		{
			this.gameManager = gameManager;
		}

		public virtual void StartState()
		{
			
		}
		
		public virtual void StateUpdate()
		{
			
		}
	}
}