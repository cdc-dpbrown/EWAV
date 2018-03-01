/*  ----------------------------------------------------------------------------
 *  Emergint Technologies, Inc.
 *  ----------------------------------------------------------------------------
 *  Epi Info™ - Web Analysis & Visualization
 *  ----------------------------------------------------------------------------
 *  File:       MainWindow.xaml.cs
 *  Namespace:  EwavEncryptionUtility    
 *
 *  Author(s):  Daniel Shorter, Mohammad Usman, Ninad Date, Sachin Agnihotri 
 *  Created:    22/08/2013    
 *  Summary:	no summary     
 *  ----------------------------------------------------------------------------
 */
using System;
using System.Windows;
using Ewav.Security;
using Microsoft.Win32;
using System.Configuration;

namespace EpiInfoWebSecurity
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {


        string clipText;
        Nullable<bool> ResultFromDialog ;
        OpenFileDialog _OpenFileDialog;
       private string saltKey;
       private string passPhrase;
       private string saltValue;
       private string generateVector;
       private bool IsEIWSProject;
        /// <summary>
        /// Initializes a new instance of the <see cref="Window1" /> class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            this.Title = "Epi Info Web Security"; //"Ewav Encryption Utility";
            //this.IsEIWSProject =bool.Parse( ConfigurationManager.AppSettings["IsEIWSProject"]);
            //if (IsEIWSProject)
            //{
            //    label3.Visibility = System.Windows.Visibility.Collapsed;
            //    txtSaltKey2.Visibility = System.Windows.Visibility.Collapsed;
            //}
            //labelClipboard.Visibility = System.Windows.Visibility.Hidden;
            //buttonCopy.IsEnabled = false;
            //buttonEncrypt.IsEnabled = CheckButons();
            //buttonDecrypt.IsEnabled = CheckButons();
            
            this.tsslVersion.Text = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
           

        }

        private void buttonExist_Click(object sender, RoutedEventArgs e)
        {


            //    textClip.IsEnabled = false;
            Cryptography c = new Cryptography(txtPassphrase.Text, txtSaltValue.Text, txtGenerateVector.Text);
            //  txtDatabaseConnectionText.Text = c.Encrypt(txtDatabaseConnectionText.Text);        
            textBox1.Text = c.Encrypt(txtDatabaseConnectionText.Text);
            CopyOutputED.IsEnabled = true;
            ClearText.IsEnabled = true;


        }
        private void buttonUpdateKesy_Click(object sender, RoutedEventArgs e)
        {
            string messageBoxText = "Do you want to update Web.config file?";
            string caption = "WARNING!";
            MessageBoxButton button = MessageBoxButton.YesNo;
            MessageBoxImage icon = MessageBoxImage.Warning;
            MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
            string filename = _OpenFileDialog.FileName;
            ConfigReader cfr = new ConfigReader(filename);
            switch (result)
            {
                case MessageBoxResult.Yes:
                    if (!IsEIWSProject)
                    {
                        cfr.UpdateValue("KeyForUserPasswordSalt", txtSaltKey2.Text.ToString(), filename);
                    }
                     cfr.UpdateValue("KeyForConnectionStringPassphrase", txtPassphrase.Text.ToString(), filename);
                     cfr.UpdateValue("KeyForConnectionStringSalt", txtSaltValue.Text.ToString(), filename);
                     cfr.UpdateValue("KeyForConnectionStringVector", txtGenerateVector.Text.ToString(), filename);


                    // CancelButton.IsEnabled = false;
                     buttonUpdateKeys.IsEnabled = false;
                    break;
                case MessageBoxResult.No:
                    // User pressed No button
                    // ...
                    break;
                case MessageBoxResult.Cancel:
                    // User pressed Cancel button
                    // ...
                    break;
            }


         
          
        }
        private void buttonFill_Click(object sender, RoutedEventArgs e)
        {


            //OpenFileDialog ofd;
            //Nullable<bool> result;
            //OpenfileDialog(out ofd, out result);

            // Process open file dialog box results 
            if (this.ResultFromDialog == true)
            {
                // Open document 
                string filename = _OpenFileDialog.FileName;

                ConfigReader cfr = new ConfigReader(filename);

                txtSaltKey2.Text = cfr.ReadKey("KeyForUserPasswordSalt");
                txtPassphrase.Text = cfr.ReadKey("KeyForConnectionStringPassphrase");
                txtSaltValue.Text = cfr.ReadKey("KeyForConnectionStringSalt");
                txtGenerateVector.Text = cfr.ReadKey("KeyForConnectionStringVector");

                buttonEncrypt.IsEnabled = CheckButons();
                buttonDecrypt.IsEnabled = CheckButons();

                textClip.Text = " ";
                //label1.Visibility = System.Windows.Visibility.Collapsed;
                //labelClipboard.Visibility = System.Windows.Visibility.Collapsed;
                //buttonCopy.Visibility = System.Windows.Visibility.Collapsed;
                //textClip.Visibility = System.Windows.Visibility.Collapsed;
                grdKeysOutput.Visibility = System.Windows.Visibility.Collapsed;
                grpEncDec.Visibility = System.Windows.Visibility.Visible;
                grdManageKeys.Visibility = System.Windows.Visibility.Visible;
            }

        }

        private static void OpenfileDialog(out OpenFileDialog ofd, out Nullable<bool> result)
        {
            ofd = new OpenFileDialog();

            ofd.FileName = "Web.config"; // Default file name
            ofd.DefaultExt = ".config"; // Default file extension
            ofd.Filter = "Web.config confiles (.config)|*.config"; // Filter files by extension 

            // Show open file dialog box
            result = ofd.ShowDialog();
        }

        private void ResetTool()
        {



            textClip.Text = " ";
           // labelClipboard.Visibility = System.Windows.Visibility.Hidden;
            txtDatabaseConnectionText.Text = " ";
            buttonCopy.IsEnabled = false;


        }

        private void buttonNew_Click(object sender, RoutedEventArgs e)
        {
           
            grdKeysOutput.Visibility = System.Windows.Visibility.Visible;
            GenerateNewKeys();
            WriteKeysToClipboard();
            buttonUpdateKeys.IsEnabled = true;
            buttonCopy.IsEnabled = true;
        }
       
        /// <summary>
        /// Returns flag for buton activation              
        /// </summary>
        /// <returns></returns>
        private bool CheckButons()
        {
            if (!IsEIWSProject)
            {
                return (txtGenerateVector.Text.Length > 0 &&
                        txtPassphrase.Text.Length > 0 &&
                        txtSaltKey2.Text.Length > 0 &&
                        txtSaltValue.Text.Length > 0);
            }
            else {
                return (txtGenerateVector.Text.Length > 0 &&
                           txtPassphrase.Text.Length > 0 &&
                           txtSaltValue.Text.Length > 0);
            
            }
        }

        private void textClip_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            buttonEncrypt.IsEnabled = CheckButons();
            buttonDecrypt.IsEnabled = CheckButons();
        }

        private void txtDatabaseConnectionText_textChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            buttonEncrypt.IsEnabled = CheckButons();
            buttonDecrypt.IsEnabled = CheckButons();
        }

        /// <summary>
        /// Writes the keys to clipboard.
        /// </summary>
        private void WriteKeysToClipboard()
        {
            
            

            string addTag;
            string _saltKey =txtSaltKey2.Text ;
            string _passPhrase =txtPassphrase.Text ;
            string _saltValue = txtSaltValue.Text;
            string _generateVector =txtGenerateVector.Text ;

            string guid = Guid.NewGuid().ToString();

            addTag = "<!-- Encryption keys -->\n";
            addTag += string.Format("<!-- Created {0} {1}  by " + Environment.UserName + "  -->\n", DateTime.Now.ToShortDateString(), DateTime.Now.ToShortTimeString());
            addTag += string.Format("<!-- GUID:  {0} -->\n", guid.ToUpper());
            addTag += "<!-- WARNING:  -->\n";
            addTag += "<!-- Once users, datasources or connection strings have been added to the database these keys *cannot* be modified. --> \n";
            addTag += "<!-- If these keys are modified all existing encrypted data will be invalid.  -->\n";
            addTag += "<!-- WARNING:  -->\n";
            //addTag += "<add key=\"KeyForUserPasswordSalt\" value=\"{0}\"    />\n";
            //addTag += "<add key=\"KeyForConnectionStringPassphrase\" value=\"{1}\"    />\n";
            //addTag += "<add key=\"KeyForConnectionStringSalt\" value=\"{2}\"    />\n";
            //addTag += "<add key=\"KeyForConnectionStringVector\" value=\"{3}\"    />";

            addTag += "<add key=\"KeyForConnectionStringVector\" value=\"{3}\"    />\n";
            addTag += "<add key=\"KeyForConnectionStringPassphrase\" value=\"{1}\"    />\n";
          
            addTag += "<add key=\"KeyForConnectionStringSalt\" value=\"{2}\"    />\n";
            if (!IsEIWSProject)
            {
                addTag += "<add key=\"KeyForUserPasswordSalt\" value=\"{0}\"    />\n";
            }
            if (txtDatabaseConnectionText.Text.Length > 0)
            {
                addTag += "\n\n";
                addTag += "<add key=\"MetaDataConnectionString\" value=\"{4}\"  />";

                Cryptography cy = new Cryptography(_passPhrase, _saltValue, _generateVector);
                string databaseConnection = cy.Encrypt(txtDatabaseConnectionText.Text);

                clipText = string.Format(addTag, _saltKey, _passPhrase, _saltValue,
                    _generateVector, databaseConnection).Replace("\n", Environment.NewLine);




            }
            else
            {
                clipText = string.Format(addTag, _saltKey, _passPhrase, _saltValue,
                    _generateVector).Replace("\n", Environment.NewLine);
            }

            



            textClip.Text = clipText;
          //  buttonCopy.IsEnabled = true;


        }

        private void GenerateNewKeys( )
        {
            if (!IsEIWSProject)
            {
                txtSaltKey2.Text = PasswordHasher.CreateSalt();
            }
            txtPassphrase.Text = PasswordHasher.CreateSalt();
            txtSaltValue.Text = PasswordHasher.CreateSalt();
            txtGenerateVector.Text = PasswordHasher.CreateSalt().Substring(0, 16);
        
        }

        private void buttonDecrypt_Click(object sender, RoutedEventArgs e)
        {      


            //  textClip.IsEnabled = false;
            Cryptography c = new Cryptography(txtPassphrase.Text, txtSaltValue.Text, txtGenerateVector.Text);
            //  txtDatabaseConnectionText.Text = c.Decrypt(txtDatabaseConnectionText.Text);
            try
            {
                textBox1.Text = c.Decrypt(txtDatabaseConnectionText.Text);
                CopyOutputED.IsEnabled = true;
                ClearText.IsEnabled = true;
                
            }
            catch (Exception ex){
                string messageBoxText = "Error occurred! Text entered can't be decrypted. Please validate entered text and try again.";
                string caption = "WARNING!";
                MessageBoxButton button = MessageBoxButton.OK;
                MessageBoxImage icon = MessageBoxImage.Warning;
                MessageBoxResult result = MessageBox.Show(messageBoxText, caption, button, icon);
            
            }

        }

        private void buttonCopy_Click(object sender, RoutedEventArgs e)
        {



            Clipboard.SetText(textClip.Text);

        //    labelClipboard.Visibility = System.Windows.Visibility.Visible;


        }

        private void textClip_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {




           // buttonCopy.IsEnabled = true;
          //  labelClipboard.Visibility = System.Windows.Visibility.Hidden;



        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(textBox1.Text);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            GetKeyFromWebConfig();
        }

        private void GetKeyFromWebConfig()
        {
            OpenFileDialog ofd;
            Nullable<bool> result;
            OpenfileDialog(out ofd, out result);
            ResultFromDialog = result;
            _OpenFileDialog = ofd;
            if (this.ResultFromDialog == true)
            {
                // Open document 
                string filename = _OpenFileDialog.FileName;

                ConfigReader cfr = new ConfigReader(filename);


                if (cfr.ReadKey("KeyForUserPasswordSalt") == string.Empty)
                {
                    this.IsEIWSProject = true;
                    label3.Visibility = System.Windows.Visibility.Collapsed;
                    txtSaltKey2.Visibility = System.Windows.Visibility.Collapsed;

                }
                else
                {
                    this.IsEIWSProject = false;
                    label3.Visibility = System.Windows.Visibility.Visible;
                    txtSaltKey2.Visibility = System.Windows.Visibility.Visible;

                }


                if (!IsEIWSProject)
                {
                    this.saltKey = txtSaltKey2.Text = cfr.ReadKey("KeyForUserPasswordSalt");
                }
                this.passPhrase = txtPassphrase.Text = cfr.ReadKey("KeyForConnectionStringPassphrase");
                this.saltValue = txtSaltValue.Text = cfr.ReadKey("KeyForConnectionStringSalt");
                this.generateVector = txtGenerateVector.Text = cfr.ReadKey("KeyForConnectionStringVector");

                WriteKeysToClipboard();

                this.path.Text = _OpenFileDialog.FileName.ToString();
                buttonNew.IsEnabled = true;

            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (!IsEIWSProject)
            {
                txtSaltKey2.Text = this.saltKey;
            }
            txtPassphrase.Text = this.passPhrase;
            txtSaltValue.Text = this.saltValue;
            txtGenerateVector.Text = this.generateVector;
            WriteKeysToClipboard();
           // CancelButton.IsEnabled = false;
            buttonUpdateKeys.IsEnabled = false;
        }

        private void ClearText_Click(object sender, RoutedEventArgs e)
        {
            txtDatabaseConnectionText.Text = "";
            textBox1.Text = "";

        }


    }
}