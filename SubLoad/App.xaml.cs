namespace SubLoad
{
    using System.Windows;

    public partial class App : Application
    {
        public string PathArg { get; set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.Length > 0)
            {
                this.PathArg = e.Args[0];
                base.OnStartup(e);
            }
        }
    }
}