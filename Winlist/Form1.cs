using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.DirectoryServices;
using System.Collections;

namespace Winlist
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            AD search = new AD();
            string strUserName = string.Empty;
            ArrayList arGroups = new ArrayList();
            string strOutput = string.Empty;
            tbOutput.Clear();


            strUserName = search.GetObjectDistinguishedName(AD.objectClass.user, AD.returnType.distinguishedName, tbUserName.Text, search.GetDomainString());
            arGroups = search.Groups(strUserName, true);

            foreach (string strGroup in arGroups)
            {
                strOutput += strGroup + ";\r \n";
                
            }

            tbOutput.Text = strOutput;

        }

         
    }
}
