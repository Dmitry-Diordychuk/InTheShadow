using UnityEngine;

namespace InTheShadow
{
	public class _GameState: MonoBehaviour
	{
		public GameManager.GameState State { get; protected set; }

		protected GameManager gameManager;
		
		public virtual void InitState(GameManager manager)
		{
			this.gameManager = manager;
		}
		
		public virtual void StartState()
		{
			
		}
		
		private bool _isStarted;
		public virtual void UpdateState()
		{
			
		}

		public virtual void FinishState()
		{
			
		}
	}
}