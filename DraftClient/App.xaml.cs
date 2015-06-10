namespace DraftClient
{
    using System;
    using System.IO;
    using System.Text;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private const string _errorLogName = "ErrorLog.txt";

        private void App_OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show(string.Format("An unhandled error has occurred:{0}{1}{2}Please report this issue to Louie :)", Environment.NewLine, e.Exception.Message, Environment.NewLine));

            WriteErrorLog(e.Exception);

            e.Handled = true;
        }

        private void WriteErrorLog(Exception error)
        {
            try
            {
                if (File.Exists(_errorLogName))
                {
                    var fi = new FileInfo(_errorLogName);
                    //5meg
                    if (fi.Length > 1024*1024*5)
                    {
                        File.Delete(_errorLogName);
                        File.WriteAllText(_errorLogName, DateTime.Now + @" - Log was truncated." + Environment.NewLine, Encoding.ASCII);
                    }
                }
                File.AppendAllText(_errorLogName, DateTime.Now + @" - " + error + Environment.NewLine, Encoding.ASCII);
            }
            catch
            {
                //well now you're screwed
            }
        }
    }
}