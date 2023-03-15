using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        int position = -1, IDEtu;
        bool Check = false;
        public Form1()
        {
            InitializeComponent();
            
        }
        private bool SiVide()
        {
            if (this.tbID.Text.Trim().Equals(string.Empty) || this.tbNom.Text.Trim().Equals(string.Empty)
                || this.tbPrenom.Text.Trim().Equals(string.Empty) || this.tbAge.Text.Trim().Equals(string.Empty))
                return false;
            return true;
        }

        private void EmptyTb()
        {
            tbNom.Text = string.Empty;
            tbPrenom.Text = string.Empty;
            tbAge.Text = string.Empty;

            int nbrLigne = DV.Rows.Count - 1;
            tbID.Text = (Convert.ToInt32(DV.Rows[nbrLigne].Cells[0].Value.ToString()) + 1).ToString();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.DV.AllowUserToAddRows = false;
            bt_AjouterList_Click(sender, e);
            EmptyTb();
            tbNom.Focus();
        }

        string connaixionstring = ConfigurationManager.ConnectionStrings["CnxSqlServer"].ConnectionString;
        private void bt_AjouterList_Click(object sender, EventArgs e)
        {
            SqlConnection cnx = new SqlConnection();
            cnx.ConnectionString = connaixionstring;


            string query = "Select * from Etudients;";
            SqlCommand cmd = new SqlCommand(query, cnx);

            cnx.Open();
            SqlDataReader rd = cmd.ExecuteReader();


            if (rd.HasRows)
            {
                this.DV.Rows.Clear();
                while (rd.Read())
                {
                    this.DV.Rows.Add(rd[0], rd[1], rd[2], rd[3]);
                }
                cnx.Close();
            }
            else
            {
                MessageBox.Show("la table etudiants est vide !!");
            }



            
        }

        private void Ajouter_Click(object sender, EventArgs e)
        {
            try
            {
                if (Check == true)
                    return;

                if (!SiVide())
                {
                    MessageBox.Show("Veuillez saisir tout les info");
                    return;
                }

                using(SqlConnection cnx = new SqlConnection(connaixionstring)) 
                {
                    string Query = $"INSERT INTO Etudients VALUES({this.tbID.Text.Trim()},'{this.tbNom.Text.Trim()}','{this.tbPrenom.Text.Trim()}',{this.tbAge.Text.Trim()});";
                    SqlCommand cmd = new SqlCommand(Query, cnx);
                    if (cnx.State == ConnectionState.Open)
                        cnx.Close() ;
                    cnx.Open();

                    int a = cmd.ExecuteNonQuery();
                    bt_AjouterList_Click(sender, e);

                    EmptyTb();

                    MessageBox.Show(a + " ligne(s) affectee");
                    cnx.Close();
                    
                    
                }
            }
            catch (Exception ex) 
            {
                MessageBox.Show(ex.Message, "Error",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            //
        }

        

        
        

        private void DV_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            position = this.DV.CurrentRow.Index;
            IDEtu = int.Parse(this.DV.Rows[position].Cells[0].Value.ToString());

            tbID.Text = this.DV.Rows[position].Cells[0].Value.ToString();
            tbNom.Text = this.DV.Rows[position].Cells[1].Value.ToString();
            tbPrenom.Text = this.DV.Rows[position].Cells[2].Value.ToString();
            tbAge.Text = this.DV.Rows[position].Cells[3].Value.ToString();

            tbNom.Enabled = false;
            tbPrenom.Enabled = false;
            tbAge.Enabled = false;
            Check = true;
        }

        private void bt_Nouveau_Click(object sender, EventArgs e)
        {
            EmptyTb();
            tbNom.Enabled = true;
            tbPrenom.Enabled = true;
            tbAge.Enabled = true;
            tbNom.Focus();


        }

        int valid = -1;
        private void button1_Click(object sender, EventArgs e)
        {

            if (position == -1)
            {
                MessageBox.Show("Selectionnez un ligne");
                return;
            }
            else if (valid == -1)
            {
                bt_Nouveau.Enabled = false;
                btAjouter.Enabled = false;
                tbAge.Enabled=true;
                tbNom.Enabled = true;
                tbPrenom.Enabled = true;
                bt_Modifier.Text = "Valider";
                valid = 1;
                
            }
            else if(valid == 1)
            {

                if (!SiVide())
                {
                    MessageBox.Show("Completez-Vous les Information S'il vous plait");
                    return;
                }
                    
                
                string Query = "UPDATE Etudients SET Nom = @p1 , Prenom = @p2 , Age = @p3 WHERE ID = @p4;";
                using(SqlConnection cnx = new SqlConnection(connaixionstring))
                {
                    SqlCommand command = new SqlCommand(Query, cnx);
                    command.Parameters.AddWithValue("@p1", tbNom.Text.Trim());
                    command.Parameters.AddWithValue("@p2", tbPrenom.Text.Trim());
                    command.Parameters.AddWithValue("@p3", tbAge.Text.Trim());
                    command.Parameters.AddWithValue("@p4", tbID.Text.Trim());
                    cnx.Open();
                    command.ExecuteNonQuery();

                    cnx.Close();

                    bt_AjouterList_Click(sender,e);
                    MessageBox.Show("Le livre est bien Modifier");

                }
                valid = -1;
                position = -1;
                EmptyTb();
                bt_Modifier.Text = "Modifier";
                bt_Nouveau.Enabled = true;
                btAjouter.Enabled = true;

                Check = false;


            }

            
        }

        private void bt_Supprimer_Click(object sender, EventArgs e)
        {
            if (position == -1)
            {
                MessageBox.Show("Selectionnez un ligne");
                return;
            }
                
            using (SqlConnection cnx = new SqlConnection(connaixionstring))
            {
                if (MessageBox.Show("Avez Vous vraiment Supprimer l'etudiant !", "Supprimer", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;

                SqlCommand cmd = new SqlCommand("DELETE FROM Etudients WHERE ID = @id");
                cmd.Connection = cnx;
                cmd.CommandText = "DELETE FROM Etudients WHERE ID = @id";
                cmd.Parameters.AddWithValue("@id", IDEtu);

                cnx.Open();
                cmd.ExecuteNonQuery();

                this.DV.Rows.RemoveAt(position);

                EmptyTb();
                tbNom.Enabled = true;
                tbPrenom.Enabled = true;
                tbAge.Enabled = true;


                MessageBox.Show("Le livre est bien supprimer");
                cnx.Close();
            }

            position = -1;
            Check = false;
            
            
        }

       
    }
}
