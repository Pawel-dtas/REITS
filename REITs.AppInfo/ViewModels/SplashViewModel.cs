using System.Deployment.Application;

namespace REITs.AppInfoModule.ViewModels
{
	public class SplashViewModel
	{
		#region Private Fields

		public string SystemVersion { get; set; }

		#endregion Private Fields

		#region Public Constructors

		public SplashViewModel()
		{
			try
			{
				SystemVersion = System.Windows.Forms.Application.ProductVersion;
			}
			catch (InvalidDeploymentException)
			{
				SystemVersion = "Dev";
			}
		}

		#endregion Public Constructors
	}
}