using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace PP3
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            UpdateUser();
            textBoxFolder.Text = folderBrowserDialog1.SelectedPath.ToString();
            textBoxClient.ReadOnly = true;
            textBoxFolder.ReadOnly = true;
            textBoxSSL.ReadOnly = true;
        }

        public void UpdateUser()//permet de mettre à jour la liste des utilisateur 
        {
            listBoxUsers.Items.Clear();
            MongoClient dbClient = new MongoClient("mongodb+srv://dbUser:LGNYQijN2ZSmiFel@cluster0.hqef6.mongodb.net/client?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("client");
            var collection = database.GetCollection<Profil>("profile");

            var users = collection
            .Find("{}")
            .ToListAsync()
            .Result;

            foreach (var user in users)
            {
                listBoxUsers.Items.Add(user.Name);
            }
        }

        public bool test() //verifie si un profil du meme nom n'existe pas deja 
        {
            bool find = false;
            MongoClient dbClient = new MongoClient("mongodb+srv://dbUser:LGNYQijN2ZSmiFel@cluster0.hqef6.mongodb.net/client?retryWrites=true&w=majority");
            var database = dbClient.GetDatabase("client");
            var collection = database.GetCollection<Profil>("profile");

            var users = collection
           .Find("{}")
           .ToListAsync()
           .Result;

            foreach (var user in users)
            {
                if (user.Name == textBoxCompagny.Text)
                {
                    find = true;
                }
            }
            return find;
        }

        public class Profil
        {
            public ObjectId Id { get; set; }
            public string Name { get; set; }
            public string Function { get; set; }
            public string Country { get; set; }
            public string StateLetter { get; set; }
            public string City { get; set; }
            public string Domain { get; set; }
        }

        //Variables
        public string CompagnyName;
        public string CompagnyFunction;
        public string CompagnyState;
        public string CompagnyStateLetter;
        public string CompagnyStateCity;
        public string CompagnyDomain;

        //Fonctions
        public bool VerfiyCompagnyName() //le nom de la société doit etre superieur à 2 caractères
        {
            CompagnyName = textBoxCompagny.Text;
            if (CompagnyName.Length > 2)
            {
                return true;
            }
            else
            {
                labelDebug.ForeColor = System.Drawing.Color.Red;
                labelDebug.Text = "Le nom de la société doir etre supérieur à 3 lettes";
                return false;
            }
        }

        public bool VerfiyCompagnyFunction()//le nom de la fonction doit etre superieur à 2 caractères
        {
            CompagnyFunction = textBoxFunction.Text;
            if (CompagnyFunction.Length > 2)
            {
                return true;
            }
            else
            {
                labelDebug.ForeColor = System.Drawing.Color.Red;
                labelDebug.Text = "Le nom de votre fonction doit etre supérieur à 3 lettes";
                return false;
            }
        }


        public bool VerfiyCompagnyState()//le nom de l'état doit etre superieur à 2 caractères
        {
            CompagnyState = textBoxCountry.Text;
            if (CompagnyState.Length >= 2)
            {
                CompagnyStateLetter = CompagnyState.Substring(0, 2);
                CompagnyStateLetter = CompagnyStateLetter.ToUpper();
                return true;
            }
            else
            {
                labelDebug.ForeColor = System.Drawing.Color.Red;
                labelDebug.Text = "Le nom de la société doir etre supérieur à 3 lettes";
                return false;
            }
        }

        public bool VerfiyCompagnyCity()//le nom de la ville doit etre superieur à 2 caractères
        {
            var CompagnyCity = textBoxCity.Text;
            if (CompagnyCity.Length > 2)
            {
                return true;
            }
            else
            {
                labelDebug.ForeColor = System.Drawing.Color.Red;
                labelDebug.Text = "Le nom de votre ville doit etre supérieur à 3 lettes";
                return false;
            }
        }

        public bool VerfiyCompagnyDomain()//Permet de verifié que le domaine soit d'une forme valide
        {
            var CompagnyDomaine = textBoxDomain.Text;
            bool isvalide = false;

            bool ipValidation()//verifie si le domaine est une IP
            {
                IPAddress ip;
                bool validdomain = IPAddress.TryParse(CompagnyDomaine, out ip);
                if (validdomain)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            bool IsUrlValid()//verifie si le domaine est un nom de domaine 
            {
                if (CompagnyDomaine == null)
                {
                    return false;
                }

                if (Regex.IsMatch(CompagnyDomaine, @"(([www\.])?|([\da-z-\.]+))\.([a-z\.]{2,3})$") == true)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }


            if (ipValidation() == true || IsUrlValid() == true) //recoupe les résultats du test de domaine
            {
                isvalide = true;
            }
            else
            {
                isvalide = false;
                labelDebug.ForeColor = System.Drawing.Color.Red;
                labelDebug.Text = "L'ip/domaine n'est pas valide";
                return false;
            }

            return isvalide;
        }

        //FRONTEND 






        private void listBox1_SelectedIndexChanged(object sender, EventArgs e) //Permet de séléctionné un profile et d'y lier des champs de manières dynamique 
        {
            if (listBoxUsers.SelectedItem != null)
            {
                MongoClient dbClient = new MongoClient("mongodb+srv://dbUser:LGNYQijN2ZSmiFel@cluster0.hqef6.mongodb.net/client?retryWrites=true&w=majority");
                var database = dbClient.GetDatabase("client");
                var collection = database.GetCollection<Profil>("profile");
                var users = collection
                .Find(u => u.Name == listBoxUsers.SelectedItem.ToString())//trouve le profile sélectionné en base 
                .ToListAsync()
                .Result;

                foreach (var user in users) //met a jour les champs liés
                {
                    textBoxCompagny.Text = user.Name;
                    textBoxClient.Text = user.Name;
                    textBoxFunction.Text = user.Function;
                    textBoxCountry.Text = user.Country;
                    textBoxCity.Text = user.City;
                    textBoxDomain.Text = user.Domain;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e) //Ajouter un profile 
        {
            labelDebug.Text = "";
            if (VerfiyCompagnyName() == true && VerfiyCompagnyFunction() == true && VerfiyCompagnyState() == true && VerfiyCompagnyCity() == true && VerfiyCompagnyDomain() == true) //verifie les champs saisies
            {
                if (test() == true)//verifie que le profile n'existe pas deja 
                {
                    labelDebug.ForeColor = System.Drawing.Color.Red;
                    labelDebug.Text = "Ce profile existe deja";
                }
                else
                {
                    MongoClient dbClient = new MongoClient("mongodb+srv://dbUser:LGNYQijN2ZSmiFel@cluster0.hqef6.mongodb.net/client?retryWrites=true&w=majority");
                    var database = dbClient.GetDatabase("client");
                    var collection = database.GetCollection<Profil>("profile");
                    var entity = new Profil { Name = textBoxCompagny.Text, Function = textBoxFunction.Text, Country = textBoxCountry.Text, StateLetter = CompagnyStateLetter, City = textBoxCity.Text, Domain = textBoxDomain.Text };
                    collection.InsertOne(entity); //Insert le nouveau profile en base
                    UpdateUser();
                    labelDebug.ForeColor = System.Drawing.Color.Green;
                    labelDebug.Text = "Profile ajouter avec succes";
                }
            }
        }

        private void button3_Click(object sender, EventArgs e) //Permet de supprimer l'intégralité des profils en base 
        {
            //demande d'abord confirmation car opération lourde de conséquances 
            string message = "Etes vous sur de vouloir supprimer l'intégralité des clients?";
            string caption = "Suppression clients";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;
            result = MessageBox.Show(message, caption, buttons);
            if (result == System.Windows.Forms.DialogResult.Yes)
            {
                MongoClient dbClient = new MongoClient("mongodb+srv://dbUser:LGNYQijN2ZSmiFel@cluster0.hqef6.mongodb.net/client?retryWrites=true&w=majority");
                var database = dbClient.GetDatabase("client");
                var collection = database.GetCollection<Profil>("profile");
                collection.DeleteMany("{}");
                UpdateUser();
                textBoxClient.Text = "";
            }
        }

        private void button2_Click_1(object sender, EventArgs e)//Permet de séléctionné le dossier de sauvegarde des certificats
        {
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxFolder.Text = folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = folderBrowserDialog1.RootFolder;
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (textBoxClient.Text != "")//verifie que les champs ne soient pas vide 
            {
                string SSL = textBoxSSL.Text;
                if (File.Exists(SSL + "/bin/openssl.exe") == true) //verifie que OpenSSL soit accesible 
                {
                    //démarre un processus hérité a la racine d'Openssl
                    System.Diagnostics.ProcessStartInfo procStartInfo =
                    new System.Diagnostics.ProcessStartInfo("cmd", "/K ");
                    procStartInfo.UseShellExecute = false;
                    procStartInfo.WorkingDirectory = SSL + "/bin";
                    procStartInfo.CreateNoWindow = true;

                    System.Diagnostics.Process proc = new System.Diagnostics.Process();
                    proc.StartInfo = procStartInfo;

                    proc.StartInfo.RedirectStandardOutput = true;
                    proc.StartInfo.RedirectStandardInput = true;
                    proc.Start();

                    string stateletter = textBoxCountry.Text.Substring(0, 2);
                    stateletter = stateletter.ToUpper();
                    string subj = "/C=" + stateletter + "/ST=" + textBoxCountry.Text + "/L=" + textBoxCity.Text + "/O=" + textBoxCompagny.Text + "/OU=" + textBoxFunction.Text + "/CN=" + textBoxDomain.Text;

                    int days = Int32.Parse(comboBoxTime.Text);
                    days = days * 365;
                    //Permet de généré les différants certificat / clée 
                    proc.StandardInput.WriteLine("openssl genrsa -des3 -passout pass:" + textBoxPass.Text + " -out ca.key 2048 ");
                    proc.StandardInput.WriteLine("openssl req -new -x509 -days " + days + " -key ca.key -passin pass:" + textBoxPass.Text + " -out ca.crt -subj \"" + subj + "\"");
                    proc.StandardInput.WriteLine("openssl genrsa -out server.key 2048");
                    proc.StandardInput.WriteLine("openssl req -new -out server.csr -key server.key -subj \"" + subj + "\"");
                    proc.StandardInput.WriteLine("openssl x509 -req -passin pass:" + textBoxPass.Text + " -in server.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out server.crt -days " + days);
                    proc.StandardInput.WriteLine("openssl genrsa -out client.key 2048");
                    proc.StandardInput.WriteLine("openssl req -new -out client.csr -key client.key -subj " + subj);
                    proc.StandardInput.WriteLine("openssl x509 -req -passin pass:" + textBoxPass.Text + " -in client.csr -CA ca.crt -CAkey ca.key -CAcreateserial -out client.crt -days " + days);
                    proc.Close(); //Ferme le processus 

                    //crer un répertoire à l'endroit séléctionné préalablement
                    string path = textBoxFolder.Text + "/" + textBoxCompagny.Text + "/" + DateTime.Now.ToString("MMddyyyyHHmmss");
                    Directory.CreateDirectory(path);
                    //Déplace les certificats dans le répertoire 
                    File.Move(SSL + "/bin/ca.crt", path + "/" + "ca.crt");
                    File.Move(SSL + "/bin/ca.key", path + "/" + "ca.key");
                    File.Move(SSL + "/bin/ca.srl", path + "/" + "ca.srl");
                    File.Move(SSL + "/bin/client.crt", path + "/" + "client.crt");
                    File.Move(SSL + "/bin/client.key", path + "/" + "client.key");
                    File.Move(SSL + "/bin/client.csr", path + "/" + "client.csr");
                    File.Move(SSL + "/bin/server.crt", path + "/" + "server.crt");
                    File.Move(SSL + "/bin/server.key", path + "/" + "server.key");
                    File.Move(SSL + "/bin/server.csr", path + "/" + "server.csr");
                    Thread.Sleep(1000);
                    MessageBox.Show("Les certificats ont été généré : " + path, "Généré avec succès", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    MessageBox.Show("Veuillez verifier le directory de OpenSSL", "Erreur OpenSSL", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            else
            {
                MessageBox.Show("Vous devez remplir l'intégralité des champs afin de pouvoir générer les certificats", "Erreur génération", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void button2_Click_5(object sender, EventArgs e) //modifie le chemin d'acces a OpenSSL
        {

            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                textBoxSSL.Text = folderBrowserDialog1.SelectedPath;
                Environment.SpecialFolder root = folderBrowserDialog1.RootFolder;
            }
        }


    }
}
