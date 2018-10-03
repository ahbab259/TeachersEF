using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TeachersEF
{
    public partial class Form1 : Form
    {
        CentralDBEntities dbContext = new CentralDBEntities();
        Teacher model = new Teacher();
        Subject subjectModel = new Subject();
        int SubjectId = 0;
        int TeacherId = 0;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            //this.teacherTableAdapter.Fill(this.teachersDataSet.Teacher);
            load();
            populateSubjectDropdown();
            dropDownSubject.SelectedItem = null;
        }

        void populateSubjectDropdown()
        {
            var subjectList = dbContext.Subjects.Select(p => p.SubjectName).ToArray();
            dropDownSubject.DataSource = subjectList;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (txtFName.Text == "" || txtLName.Text == "" || txtAddress.Text == "")
            {
                MessageBox.Show("Please fill up all the fields");
            }
            else
            {
                model.FirstName = txtFName.Text.Trim();
                model.LastName = txtLName.Text.Trim();
                model.Address = txtAddress.Text.Trim();
                model.SubjectId = dbContext.Subjects.Where(x => 
                                     x.SubjectName == dropDownSubject.SelectedItem)
                                    .SingleOrDefault()?.SubjectId;
                if (pictureBoxImage.ImageLocation != null)
                {
                    model.ImageLocation = pictureBoxImage.ImageLocation;
                }
                dbContext.Teachers.Add(model);
                dbContext.SaveChanges();
                MessageBox.Show("Data added successfully");
                clear();
                load();
            }
        }

        void clear()
        {
            txtFName.Text = "";
            txtLName.Text = "";
            txtAddress.Text = "";
            pictureBoxImage.Image = null;
            dropDownSubject.SelectedItem = null;

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            load();
        }

        void load()
        {
            List<Teacher> datas = dbContext.Teachers.OrderBy(s => s.TeacherId).ToList();
            //datas = dbContext.Teachers.ToList();
            
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.DataSource = datas;
        }

        //private byte[] ConvertFileToByte(string sPath)
        //{
        //    byte[] data = null;
        //    FileInfo fInfo = new FileInfo(sPath);
        //    long numBytes = fInfo.Length;
        //    FileStream fStream = new FileStream(sPath, FileMode.Open, FileAccess.Read);
        //    BinaryReader br = new BinaryReader(fStream);
        //    data = br.ReadBytes((int)numBytes);
        //    return data;
        //}

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Please select a photo";
            ofd.Filter = "JPG|*.jpg|PNG|*.png|GIF|*.gif";
            ofd.Multiselect = false;

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                pictureBoxImage.ImageLocation = ofd.FileName;
            }
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            var item = dbContext.Teachers.Where(a => a.TeacherId == TeacherId).FirstOrDefault();
            dbContext.Teachers.Remove(item);
            dbContext.SaveChanges();
            MessageBox.Show("Data Removed Successfully");
            clear();
            load();
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentRow.Index != -1)
            {
                TeacherId = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                txtFName.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                txtLName.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                txtAddress.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                //pictureBoxImage.Image = Image.FromFile(model.ImageLocation);

                //dropDownSubject.SelectedItem = dbContext.Subjects.Where(x =>
                                     //x.SubjectId == SubjectId).SingleOrDefault()?.SubjectName;
                showImage();
                showSubjectNameInDropdown();
            }
        }
        void showSubjectNameInDropdown()
        {
            var item = dbContext.Teachers.Where
                (a => a.TeacherId == TeacherId).FirstOrDefault();

            if (item.SubjectId != null)
            {

                var subName = dbContext.Subjects.Where
                    (a => a.SubjectId == item.SubjectId).FirstOrDefault();

                dropDownSubject.SelectedItem = subName.SubjectName;
            }
            else
            {
                dropDownSubject.SelectedItem = null;
            }
        }
        void showImage()
        {
            var item = dbContext.Teachers.Where(a => a.TeacherId == TeacherId).FirstOrDefault();

            //Function to convert byte to image
            //if (item.Image != null)
            //{
            //    byte[] arr = item.Image;
            //    MemoryStream ms = new MemoryStream(arr);
            //    pictureBoxImage.Image = Image.FromStream(ms);
            //}
            if (item.ImageLocation != null)
            {
                pictureBoxImage.Image = Image.FromFile(item.ImageLocation);
            }
            else if (item.ImageLocation == null)
            {
                pictureBoxImage.Image = Image.FromFile(@"E:\images\humanAvatar.png");
            }
        }

        private void txtSearch_TextChanged(object sender, EventArgs e)
        {
            List<Teacher> datas = dbContext.Teachers
                .Where(a => a.FirstName.Contains(txtSearch.Text.Trim())
                    || a.LastName.Contains(txtSearch.Text.Trim())
                    || a.Address.Contains(txtSearch.Text.Trim())).ToList();
            dataGridView1.DataSource = datas;
        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)
        {
            if (dataGridView1.CurrentRow.Index != -1)
            {
                TeacherId = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                showImage();
                displayNameOnClick();
            }
        }

        void displayNameOnClick()
        {
            if (dataGridView1.CurrentRow.Index != -1)
            {
                TeacherId = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value.ToString());

                model = dbContext.Teachers.Where(a => a.TeacherId == TeacherId).FirstOrDefault();
                string fName = model.FirstName.ToString();
                string lName = model.LastName.ToString();
                string address = model.Address.ToString();
                string subjectName = dbContext.Subjects
                        .Where(a => a.SubjectId == model.SubjectId).FirstOrDefault()?
                        .SubjectName.ToString();

                showNameOnClick.Text = fName + " " + lName;
                showAddressOnClick.Text = address;
                showSubjectOnClick.Text = subjectName;

                showNameOnClick.Visible = true;
                showAddressOnClick.Visible = true;
                showSubjectOnClick.Visible = true;
                addressLabel.Visible = true;
                subjectLabel.Visible = true;
            }
        }
        
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (txtFName.Text == "" || txtLName.Text == "" || txtAddress.Text == "")
            {
                MessageBox.Show("Please fill up all the fields");
            }
            else
            {
                model = dbContext.Teachers.Where(a => a.TeacherId == TeacherId).FirstOrDefault();

                model.FirstName = txtFName.Text.Trim();
                model.LastName = txtLName.Text.Trim();
                model.Address = txtAddress.Text.Trim();
                model.SubjectId = dbContext.Subjects.Where(x =>
                                    x.SubjectName == dropDownSubject.SelectedItem)
                                   .FirstOrDefault()?.SubjectId;

                if (pictureBoxImage.ImageLocation != null)
                {
                    model.ImageLocation = pictureBoxImage.ImageLocation;
                }

                dbContext.SaveChanges();
                MessageBox.Show("Data Updated successfully");
                clear();
                load();
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            clear();
            load();
            showNameOnClick.Visible = false;
            showAddressOnClick.Visible = false;
            showSubjectOnClick.Visible = false;
            addressLabel.Visible = false;
            subjectLabel.Visible = false;
        }

        private void btnSearchBySubject_Click(object sender, EventArgs e)
        {
            var subId = dbContext.Subjects.Where
                        (a=>a.SubjectName == dropDownSubject.SelectedItem)
                        .FirstOrDefault()?.SubjectId;

            List<Teacher>model = dbContext.Teachers.Where(a => a.SubjectId == subId).ToList();
            dataGridView1.DataSource = model;
        }
        
    }
}
