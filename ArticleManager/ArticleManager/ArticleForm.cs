using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace ArticleManager
{
    public partial class ArticleForm : Form
    {
        public Article Article { get; private set; }
        private PictureBox picImage;
        private TextBox txtCode, txtNom, txtDescription, txtPrix;
        private ComboBox cmbMarque, cmbCategorie;
        private Button btnSauvegarder, btnAnnuler;
        private Button btnSelectImage, btnDeleteImage;
        private Label lblDragDrop;

        public ArticleForm(Article article = null)
        {
            InitializeComponent();
            InitializeGUI();

            if (article != null)
            {
                Article = article;
                ChargerDonneesArticle();
            }
            else
            {
                Article = new Article();
            }
        }

        private void InitializeGUI()
        {
            this.Text = "Article";
            this.Size = new Size(500, 310);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.FromArgb(243, 244, 246);

            // Image section
            picImage = new PictureBox
            {
                Size = new Size(150, 150),
                Location = new Point(15, 10),
                BorderStyle = BorderStyle.None,
                SizeMode = PictureBoxSizeMode.Zoom,
                AllowDrop = true,
                BackColor = Color.White
            };
            picImage.Paint += (s, e) => {
                using (Pen p = new Pen(Color.LightGray))
                {
                    e.Graphics.DrawRectangle(p, 0, 0, picImage.Width - 1, picImage.Height - 1);
                }
            };

            lblDragDrop = new Label
            {
                Text = "Glissez et déposez une image ici",
                AutoSize = false,
                Size = new Size(150, 30),
                TextAlign = ContentAlignment.MiddleCenter,
                Location = new Point(0, picImage.Height / 2 - 15),
                BackColor = Color.Transparent
            };
            picImage.Controls.Add(lblDragDrop);

            btnSelectImage = new Button
            {
                Text = "📂 Choisir",
                Location = new Point(15, 170),
                Size = new Size(75, 23),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White
            };
            btnSelectImage.FlatAppearance.BorderSize = 0;

            btnDeleteImage = new Button
            {
                Text = "🗑️ Supprimer",
                Location = new Point(90, 170),
                Size = new Size(75, 23),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(79, 70, 229)
            };
            btnDeleteImage.FlatAppearance.BorderColor = Color.FromArgb(79, 70, 229);

            var imagePanel = new Panel
            {
                Location = new Point(10, 10),
                Size = new Size(180, 210),
                BackColor = Color.White
            };
            imagePanel.Paint += (s, e) => {
                using (var p = new Pen(Color.FromArgb(229, 231, 235)))
                {
                    e.Graphics.DrawRectangle(p, 0, 0, imagePanel.Width - 1, imagePanel.Height - 1);
                }
            };

            imagePanel.Controls.AddRange(new Control[] { picImage, btnSelectImage, btnDeleteImage });

            // Form controls
            var formPanel = new Panel
            {
                Location = new Point(200, 10),
                Size = new Size(280, 210),
                BackColor = Color.White
            };
            formPanel.Paint += (s, e) => {
                using (var p = new Pen(Color.FromArgb(229, 231, 235)))
                {
                    e.Graphics.DrawRectangle(p, 0, 0, formPanel.Width - 1, formPanel.Height - 1);
                }
            };

            int y = 10;
            int spacing = 30;
            int labelX = 10;

            var lblCode = new Label { Text = "Code:", Location = new Point(labelX, y), AutoSize = true };
            txtCode = new TextBox { Location = new Point(labelX + 70, y), Size = new Size(190, 23) };
            y += spacing;

            var lblNom = new Label { Text = "Nom:", Location = new Point(labelX, y), AutoSize = true };
            txtNom = new TextBox { Location = new Point(labelX + 70, y), Size = new Size(190, 23) };
            y += spacing;

            var lblDescription = new Label { Text = "Description:", Location = new Point(labelX, y), AutoSize = true };
            txtDescription = new TextBox { Location = new Point(labelX + 70, y), Size = new Size(190, 23) };
            y += spacing;

            var lblMarque = new Label { Text = "Marque:", Location = new Point(labelX, y), AutoSize = true };
            cmbMarque = new ComboBox { Location = new Point(labelX + 70, y), Size = new Size(190, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbMarque.Items.AddRange(new string[] { "Samsung", "Sony", "Apple" });
            y += spacing;

            var lblCategorie = new Label { Text = "Catégorie:", Location = new Point(labelX, y), AutoSize = true };
            cmbCategorie = new ComboBox { Location = new Point(labelX + 70, y), Size = new Size(190, 23), DropDownStyle = ComboBoxStyle.DropDownList };
            cmbCategorie.Items.AddRange(new string[] { "Téléphones", "Média", "Télévisions" });
            y += spacing;

            var lblPrix = new Label { Text = "Prix:", Location = new Point(labelX, y), AutoSize = true };
            txtPrix = new TextBox { Location = new Point(labelX + 70, y), Size = new Size(190, 23) };

            formPanel.Controls.AddRange(new Control[] {
        lblCode, txtCode,
        lblNom, txtNom,
        lblDescription, txtDescription,
        lblMarque, cmbMarque,
        lblCategorie, cmbCategorie,
        lblPrix, txtPrix
    });

            // Bottom buttons
            btnSauvegarder = new Button
            {
                Text = "💾 save",
                DialogResult = DialogResult.OK,
                Location = new Point(390, 230),
                Size = new Size(90, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(79, 70, 229),
                ForeColor = Color.White
            };
            btnSauvegarder.FlatAppearance.BorderSize = 0;

            btnAnnuler = new Button
            {
                Text = "❌ Annuler",
                DialogResult = DialogResult.Cancel,
                Location = new Point(300, 230),
                Size = new Size(90, 30),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.White,
                ForeColor = Color.FromArgb(79, 70, 229)
            };
            btnAnnuler.FlatAppearance.BorderColor = Color.FromArgb(79, 70, 229);

            btnSauvegarder.Click += BtnSauvegarder_Click;
            btnSelectImage.Click += BtnSelectImage_Click;
            btnDeleteImage.Click += BtnDeleteImage_Click;
            picImage.DragEnter += PicImage_DragEnter;
            picImage.DragDrop += PicImage_DragDrop;
            picImage.Paint += PicImage_Paint;

            this.Controls.AddRange(new Control[] { imagePanel, formPanel, btnSauvegarder, btnAnnuler });
        }

        private void PicImage_Paint(object sender, PaintEventArgs e)
        {
            if (picImage.Image == null)
            {
                lblDragDrop.Visible = true;
                using (var pen = new Pen(Color.Gray, 2))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                    e.Graphics.DrawRectangle(pen, 1, 1, picImage.Width - 2, picImage.Height - 2);
                }
            }
            else
            {
                lblDragDrop.Visible = false;
            }
        }

        private void PicImage_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length == 1 && IsImageFile(files[0]))
                {
                    e.Effect = DragDropEffects.Copy;
                    return;
                }
            }
            e.Effect = DragDropEffects.None;
        }

        private void PicImage_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0 && IsImageFile(files[0]))
                {
                    LoadImage(files[0]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'image : {ex.Message}",
                              "Erreur",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void BtnSelectImage_Click(object sender, EventArgs e)
        {
            using (var dlg = new OpenFileDialog())
            {
                dlg.Filter = "Images (*.JPEG, *.PNG, *.BMP)|*.JPEG;*.PNG;*.BMP|Tous les fichiers (*.*)|*.*";
                dlg.Title = "Sélectionner une image";
                dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadImage(dlg.FileName);
                }
            }
        }

        private void BtnDeleteImage_Click(object sender, EventArgs e)
        {
            if (picImage.Image != null)
            {
                picImage.Image.Dispose();
                picImage.Image = null;
                Article.Image = null;
                picImage.Invalidate();
            }
        }

        private bool IsImageFile(string filename)
        {
            string ext = Path.GetExtension(filename).ToLower();
            return ext == ".jpg" || ext == ".jpeg" || ext == ".png" || ext == ".bmp";
        }

        private void LoadImage(string filename)
        {
            try
            {
                if (picImage.Image != null)
                {
                    picImage.Image.Dispose();
                    picImage.Image = null;
                }

                using (var stream = new FileStream(filename, FileMode.Open, FileAccess.Read))
                {
                    var img = Image.FromStream(stream);
                    picImage.Image = new Bitmap(img);
                    img.Dispose();
                }

                picImage.Invalidate();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur lors du chargement de l'image : {ex.Message}",
                              "Erreur",
                              MessageBoxButtons.OK,
                              MessageBoxIcon.Error);
            }
        }

        private void ChargerDonneesArticle()
        {
            txtCode.Text = Article.Code;
            txtNom.Text = Article.Name;
            txtDescription.Text = Article.Description;
            cmbMarque.Text = Article.Brand;
            cmbCategorie.Text = Article.Category;
            txtPrix.Text = Article.Price.ToString();

            if (Article.Image != null)
            {
                using (MemoryStream ms = new MemoryStream(Article.Image))
                {
                    picImage.Image = Image.FromStream(ms);
                }
            }
        }

        private void BtnSauvegarder_Click(object sender, EventArgs e)
        {
            if (ValiderFormulaire())
            {
                Article.Code = txtCode.Text;
                Article.Name = txtNom.Text;
                Article.Description = txtDescription.Text;
                Article.Brand = cmbMarque.Text;
                Article.Category = cmbCategorie.Text;
                Article.Price = decimal.Parse(txtPrix.Text);

                if (picImage.Image != null)
                {
                    using (var image = new Bitmap(picImage.Image))
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                            Article.Image = ms.ToArray();
                        }
                    }
                }
                else
                {
                    Article.Image = null;
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
            }
        }

        private bool ValiderFormulaire()
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
            {
                MessageBox.Show("Veuillez saisir un code.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (string.IsNullOrWhiteSpace(txtNom.Text))
            {
                MessageBox.Show("Veuillez saisir un nom.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            if (!decimal.TryParse(txtPrix.Text, out decimal prix) || prix < 0)
            {
                MessageBox.Show("Veuillez saisir un prix valide.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (picImage.Image != null)
            {
                picImage.Image.Dispose();
            }
        }
    }
}